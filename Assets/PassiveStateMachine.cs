using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PassiveStateMachine : MonoBehaviour
{
    NavMeshAgent agent;
    Rigidbody rb;
    PlayerController player;
    [SerializeField] Material moodDisplay;
    [SerializeField] Texture[] moods;
    private float timer;
    public State state;
    private int pIndex;
    [SerializeField] Transform[] PatrolPoints;

    public enum State
    {
        Wandering,
        Scanning,
        Attentive,
        Fleeing,
        Captured
    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        state = State.Wandering;
        NextState();
    }

    void NextState()
    {
        switch (state)
        {
            case State.Wandering:
                StartCoroutine(WanderingState());
                break;
            case State.Attentive:
                StartCoroutine(AttentiveState());
                break;
            case State.Fleeing:
                StartCoroutine(FleeingState());
                break;
            case State.Captured:
                StartCoroutine(CapturedState());
                break;
            default:
                break;
        }
    }

    IEnumerator WanderingState()
    {
        moodDisplay.SetTexture("_MainTex", moods[0]);
        while (state == State.Wandering)
        {
            transform.rotation *= Quaternion.Euler(0f, 50f * Time.deltaTime, 0f);
            //Direction from A to B
            //B - A
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.Normalize();
            //Dot product parameters should be "normalised"
            float result = Vector3.Dot(transform.forward, directionToPlayer);

            //If distance for navmesh agent is closer than two units away, changes destination
            if (agent.remainingDistance < 2)
            {
                agent.destination = new Vector3(Random.Range(PatrolPoints[0].position.x, PatrolPoints[1].position.x), 0, Random.Range(PatrolPoints[0].position.z, PatrolPoints[1].position.z));
            }
            if (result >= 0.95 && Vector3.Distance(transform.position, player.transform.position) < 18)
            {
                agent.destination = transform.position;
                state = State.Attentive;
            }
            yield return null;
        }

        Debug.Log("Exiting Wandering State");
        NextState();
    }

    IEnumerator AttentiveState()
    {
        moodDisplay.SetTexture("_MainTex", moods[1]);
        Debug.Log("Entered Scanning State");

        while (state == State.Attentive)
        {
            //if player position is within 8 units of monster 
            if (Vector3.Distance(transform.position, player.transform.position) < 8)
            {
                agent.speed = 8;
                //switches to fleeing 
                state = State.Fleeing;
            }
            transform.LookAt(player.transform.position);
            yield return null;
        }

        Debug.Log("Exiting Attentive State");
        NextState();
    }


    IEnumerator FleeingState()
    {
        moodDisplay.SetTexture("_MainTex", moods[2]);
        Debug.Log("Entering Fleeing State");
        while (state == State.Fleeing)
        {
            //moves away from player when player gets closer 
            Vector3 runTo = transform.position + ((transform.position - player.transform.position) * 1.5f);
            agent.SetDestination(runTo);
            if (Vector3.Distance(transform.position, player.transform.position)< 2f)
            {
                state = State.Captured;
            }
            yield return null;
        }
        Debug.Log("Exiting Fleeing State");
        NextState();
    }

    IEnumerator CapturedState()
    {
        Debug.Log("Entering Captured State");
        moodDisplay.SetTexture("_MainTex", moods[3]);
        while (state == State.Captured)
        {
            agent.SetDestination(Vector3.back + player.transform.position);
            yield return null;
        }
        Debug.Log("Exiting Captured State");
        NextState();
    }
    
}
