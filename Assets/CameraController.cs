using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Focus Variables
    [Header("Focus")]
    [SerializeField] private Transform _focus;
    [SerializeField] private Vector3 _focusOffset;

    [SerializeField, Min(0f)] private float _focusRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float _focusCentering = 0.5f;

    private Vector3 _focusPoint, _previousFocusPoint;
    #endregion


    #region Orbit Variables
    private Vector2 _orbitAngles = new Vector2(45, 0);

    [Header("Orbit")]
    [SerializeField] private float _rotationSpeed = 90f;
    [SerializeField] private float _orbitDistance = 5f;

    [SerializeField] private float _minVertAngle = -30f;
    [SerializeField] private float _maxVertAngle = 60f;

    private bool _orbitEnabled = true;
    [SerializeField] private float _zoomSpeed = 0.1f;
    #endregion

    #region Align Variables
    [Header("Align")]
    [SerializeField, Min(0f)] private float alignDelay = 2f;
    private float lastManualRotationTime = 0f;

    #endregion

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (_orbitEnabled)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                _orbitEnabled = false;

            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _orbitEnabled = true;
            }
        }

        //scroll speed below
        _orbitDistance -= Input.mouseScrollDelta.y * _zoomSpeed;
        //stops the camera from getting too close to the camera
        _orbitDistance = Mathf.Clamp( _orbitDistance, 3f, 30f);
    }
    /*private float _lastDistance = -1f;

    private void FixedUpdate()
    {
        Vector3 direction = transform.position- _focusPoint;
        RaycastHit hit;
        if (Physics.Raycast(_focusPoint, transform.position,out hit, _orbitDistance))
        {
            _orbitDistance = hit.distance;
        }
        else
        {
            if(_lastDistance >= 0f)
            {
                _orbitDistance = _lastDistance;
                _lastDistance = -1f;
            }
        }
    }*/

    private void Awake()
    {
        _focusPoint = _focus.position + _focusOffset;
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void LateUpdate()
    {
        Quaternion lookRotation = transform.localRotation;

        UpdateFocusPoint();

        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }



        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * _orbitDistance;

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private bool ManualRotation()
    {
        if(!_orbitEnabled) return false;

        Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        float e = 0.001f; //Deadzone
        if (Mathf.Abs(input.x) > e || input.y < -e || input.y > e)
        {
            _orbitAngles += _rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }

        return false;
    }

    private bool AutomaticRotation()
    {
        if(Time.unscaledTime - lastManualRotationTime < alignDelay) return false;

        Vector2 moveDelta = new Vector2(_focusPoint.x - _previousFocusPoint.x, _focusPoint.z - _previousFocusPoint.z);

        if (moveDelta.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        float headingAngle = GetAngle(moveDelta.normalized);

        _orbitAngles.y = Mathf.MoveTowardsAngle(_orbitAngles.y, headingAngle, _rotationSpeed * Time.unscaledDeltaTime);

        return true;// return true
    }
    private void UpdateFocusPoint()
    {
        _previousFocusPoint = _focusPoint;
        Vector3 targetFocusPoint = _focus.position + _focusOffset;

        if (_focusRadius > 0f)
        {
            float moveDistance = Vector3.Distance(targetFocusPoint, _previousFocusPoint);
            float t = 0.1f;

            if (moveDistance > 0.01f && _focusCentering > 0f)
            {
                t = Mathf.Pow(1f - _focusCentering, Time.unscaledDeltaTime);

            }

            if (moveDistance > _focusRadius)
            {
                t = Mathf.Min(t, _focusRadius / moveDistance);
            }

            _focusPoint = Vector3.Lerp(targetFocusPoint, _previousFocusPoint, t);
        }
        else
        {
            _focusPoint = targetFocusPoint;
        }
    }

    float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;

        if (direction.x < 0f)
        {
            return 360f - angle;
        }
        else
        {
            return angle;
        }
    }

    void ConstrainAngles()
    {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, _minVertAngle, _maxVertAngle);

        if(_orbitAngles.y < 0f)
        {
            _orbitAngles.y += 360f;
        }
        else if(_orbitAngles.y > 360f)
        {
            _orbitAngles.y -= 360f;
        }
    }

    private void OnValidate()
    {
        if(_maxVertAngle < _minVertAngle)
        {
            _maxVertAngle = _minVertAngle;
        }
    }
}
