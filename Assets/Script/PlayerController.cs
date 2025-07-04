using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform groundCheckStartPoint;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference run;
    [SerializeField] private InputActionReference crouch;
    [SerializeField] private InputActionReference jump;

    [Header("Parameters")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float jumpStrength;
    
    public bool IsGrounded { get; private set; }
    private Vector3 _velocity;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        CatchInputs();
    }

    private void GroundCheck()
    {
        float maxDistance = 0.2f;

        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default");
        if (Physics.Raycast(groundCheckStartPoint.position, -transform.up, out hit, maxDistance, layerMask))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    #region InputHandling

    private void CatchInputs()
    {
        _velocity = rigidbody.linearVelocity;
        
        ApplyMovement(movement.action.ReadValue<Vector2>());
        if (jump.action.IsPressed()) ApplySpaceBar();
        if (jump.action.IsPressed()) ApplySpaceBar();
    }
    
    private void ApplyMovement(Vector2 input)
    {
        Vector3 cameraForward = camera.transform.forward;
        
        cameraForward.y = 0;

        float angle = Vector3.Angle(Vector3.forward, cameraForward);
        bool isRight = Vector3.Dot(cameraForward, Vector3.right) > 0;

        if(IsGrounded)
            _velocity = (Quaternion.AngleAxis(isRight ? angle : -angle, Vector3.up) * new Vector3(input.x, 0, input.y)).normalized * movementSpeed;
        else
            _velocity += (Quaternion.AngleAxis(isRight ? angle : -angle, Vector3.up) * new Vector3(input.x, 0, input.y)).normalized * (movementSpeed * 3f * Time.deltaTime);
        
    }

    private void ApplySpaceBar()
    {
        if (IsGrounded)
        {
            _velocity.y = jumpStrength;
        }
    }

    private void ApplySprint()
    {
        
    }

    #endregion
    

    private void FixedUpdate()
    {
        Vector3 rbVelocity = rigidbody.linearVelocity;
        GroundCheck();


        if (IsGrounded)
        {
            rbVelocity.x = _velocity.x;
            rbVelocity.z = _velocity.z;
            if(_velocity.y != 0) rbVelocity.y = _velocity.y;
        }
        else
        {
            if(_velocity.x != 0) rbVelocity.x = _velocity.x;
            if(_velocity.z != 0) rbVelocity.z = _velocity.z;
            if(_velocity.y != 0) rbVelocity.y = _velocity.y;
        }
        
        rigidbody.linearVelocity = rbVelocity;

    }
}
