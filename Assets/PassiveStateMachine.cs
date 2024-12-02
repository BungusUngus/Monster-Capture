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
            case State.Scanning:
                StartCoroutine(ScanningState());
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
        }
    }

    IEnumerator WanderingState()
    {
        while (state == State.Wandering)
        {
            //If distance for navmesh agent is closer than two units away, changes destination
            if (agent.remainingDistance < 2)
            {
                agent.destination = new Vector3(Random.Range(PatrolPoints[0].position.x, PatrolPoints[1].position.x), Random.Range(PatrolPoints[0].position.y, PatrolPoints[1].position.y), Random.Range(PatrolPoints[0].position.z, PatrolPoints[1].position.z));


            }
        }
        yield return null;
    }

    IEnumerator ScanningState()
    {
        while (state == State.Scanning)
        {

        }
        yield return null;
    }

    IEnumerator AttentiveState()
    {
        while (state == State.Attentive)
        {

        }
        yield return null;
    }

    IEnumerator FleeingState()
    {
        while (state == State.Fleeing)
        {

        }
        yield return null;
    }

    IEnumerator CapturedState()
    {
        while (state == State.Captured)
        {

        }
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
