using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform groundCheck = null;
    [SerializeField] private float groundRadius = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask slideMask;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float jump = 10f;
    [SerializeField] private float gravity = 10f;
    private PlayerInputManager controls;
    private CharacterController cc;
    private PlayerState currentState;
    private float slopeAngle;
    private Vector3 slopeTangent;
    private Vector3 moveDirection;

    public float WalkSpeed => walkSpeed;
    public float Acceleration => acceleration;
    public float Jump => jump;
    public float Gravity => gravity;
    public float SlopeAngle { get { return slopeAngle; } set { slopeAngle = value; } }
    public Vector3 SlopeTangent { get { return slopeTangent; } set { slopeTangent = value; } }
    public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

    private void Awake()
    {
        controls = GetComponent<PlayerInputManager>();
        cc = GetComponent<CharacterController>();
        SwitchState(new AirState(this, cc, cam, controls));
    }

    private void Update()
    {
        //Update State & Apply Motion
        currentState.Update();
        cc.Move(MoveDirection * Time.deltaTime);

        Debug.Log($"CC: {cc.velocity}. MoveDir: {moveDirection}. CC: {cc.isGrounded}. {IsGrounded()}.");
    }

    public void SwitchState(PlayerState state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }
        currentState = state;
        gameObject.name = "Player - " + state.GetType().Name;
        if (currentState != null)
        {
            currentState.OnStateEnter();
        }
    }

    public void SetY(float newY)
    {
        moveDirection.y = newY;
    }

    public bool IsGrounded()
    {
        //Slope Detection
        RaycastHit hit;
        gameObject.layer = 2;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, slideMask))
        {
            Vector3 normal = hit.normal.normalized;
            Vector3 u = Vector3.Cross(normal, Vector3.down).normalized;
            SlopeTangent = Vector3.Cross(u, normal);
            slopeAngle = Vector3.Angle(Vector3.up, normal);
        }
        gameObject.layer = 9;

        //Grounded Detection
        bool grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        return grounded;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Start falling if head is hit.
        gameObject.layer = 2;
        if (Physics.CheckSphere(transform.position + Vector3.up * (cc.height / 2), 0.5f, groundMask))
        {
            moveDirection.y = -1.5f;
        }
        gameObject.layer = 9;

        //Check for edge hanging and push player back.
        if (cc.isGrounded == true && IsGrounded() == false)
        {
            cc.Move(hit.normal * 0.1f);
        }

        //Push Props
        if (hit.gameObject.layer == 12 && hit.rigidbody != null)
        {
            hit.rigidbody.AddForceAtPosition(cc.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (1), 0.5f);
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, slopeTangent));
    }
}
