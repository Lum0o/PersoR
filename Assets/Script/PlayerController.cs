using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Camera camera;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference run;
    [SerializeField] private InputActionReference crouch;
    [SerializeField] private InputActionReference jump;

    [Header("Parameters")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float runningSpeed;

    public EPlayerMovementState PlayerMovementState { get; private set; }
    private Vector3 _velocity;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        CatchInputs();
    }

    #region InputHandling

    private void CatchInputs()
    {
        ApplyMovement(movement.action.ReadValue<Vector2>());
    }
    
    private void ApplyMovement(Vector2 input)
    {
        Vector3 cameraForward = camera.transform.forward;
        
        cameraForward.y = 0;

        float angle = Vector3.Angle(Vector3.forward, cameraForward);
        bool isRight = Vector3.Dot(cameraForward, Vector3.right) > 0;
        
        _velocity = (Quaternion.AngleAxis(isRight ? angle : -angle, Vector3.up) * new Vector3(input.x, 0, input.y)).normalized * movementSpeed;
    }

    #endregion
    

    private void FixedUpdate()
    {
        Vector3 rbVelocity = rigidbody.linearVelocity;
        rbVelocity.x = _velocity.x;
        rbVelocity.z = _velocity.z;
        
        rigidbody.linearVelocity = rbVelocity;
    }
    
    public enum EPlayerMovementState
    {
        OnGround,
        InAir,
        InWater,
        UnderWater,
        OnWall
    }
}
