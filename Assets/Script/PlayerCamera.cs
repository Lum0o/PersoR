using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private InputActionReference camMovement;

    [Header("parameters")] 
    [SerializeField] private float frontOffset;
    [SerializeField] private float rightOffset;
    
    [SerializeField] private float collisionThreshold;
    
    [SerializeField] private Vector2 cameraSpeed;
    
    //Min and Max angle are for the pitch. Avoiding putting the camera upside down.
    [SerializeField, Range(-89,0)] private float minAngle;
    [SerializeField, Range(0,89)] private float maxAngle;
    [SerializeField] private Vector3 aimOffset;

    //contain yaw and pitch, roll is ignored
    private Vector2 _currentAngle;
    private Tween _currentTween;
    private float _currentFrontOffset;
    
    //private Vector3 AimPoint => playerTransform.position + aimOffset;

    void Start()
    {
        //Hide and lock cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        CatchInputs();
    }

    private void FixedUpdate()
    {
        CatchCollision();
    }


    private void CatchInputs()
    {
        Vector2 input = camMovement.action.ReadValue<Vector2>();
        Vector2 newAngle = _currentAngle;
        
        
        newAngle.x += input.x * cameraSpeed.x;
        newAngle.y += input.y * cameraSpeed.y;

        newAngle.x %= 360;
        newAngle.y = Mathf.Clamp(newAngle.y, minAngle, maxAngle);
        
        MoveToPosition(GetGoalPosition(newAngle), 0.1f);

        //look at the right point
        Vector3 forward = transform.position - playerTransform.position;
        forward.y = 0;
        float angle = Vector3.Angle(Vector3.forward, forward.normalized);
        bool isRight = Vector3.Dot(forward, Vector3.right) > 0;
        transform.LookAt(Quaternion.AngleAxis(isRight ? angle : -angle, Vector3.up) * aimOffset + playerTransform.position);
        
        _currentAngle = newAngle;
    }

    private void CatchCollision()
    {
        Vector3 origin = playerTransform.position;
        Vector3 direction = (transform.position - origin).normalized;
        float maxDistance = -frontOffset;

        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default");

        if (Physics.Raycast(origin, direction, out hit, maxDistance, layerMask))
        {
            _currentFrontOffset = (-hit.distance + collisionThreshold);
        }
        else
        {
            _currentFrontOffset = frontOffset;
        }
    }

    
    private Vector3 GetGoalPosition(Vector2 goalCameraAngles)
    {
        Vector3 playerPos = playerTransform.position;
        goalCameraAngles.y = Mathf.Clamp(goalCameraAngles.y, minAngle, maxAngle);

        Vector3 forward = Quaternion.AngleAxis(goalCameraAngles.x, Vector3.up) * Vector3.forward;
        
        Vector3 right = Vector3.Cross(forward.normalized, Vector3.up);
        
        forward = Quaternion.AngleAxis(goalCameraAngles.y, right) * forward;

        Vector3 goalPos = forward.normalized * _currentFrontOffset + right.normalized * rightOffset;
        
        goalPos += playerPos;
        
        return goalPos;
    }

    private void MoveToPosition(Vector3 position, float timeToGo)
    {
        transform.position = position;
    }
}
