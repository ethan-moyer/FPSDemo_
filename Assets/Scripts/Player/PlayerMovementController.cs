using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform groundCheck = null;
    [Header("Ground & Slopes")]
    [SerializeField] private float groundRadius = 0.5f;
    [SerializeField] private float slopeForce = 10f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask slideMask;
    [Header("Movement Attributes")]
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float decceleration = 1f;
    [SerializeField] private float jump = 10f;
    [SerializeField] private float gravity = 10f;
    [Header("Sounds")]
    [SerializeField] private AudioClip footsteps = null;
    [SerializeField] private AudioClip landingLight = null;
    [SerializeField] private AudioClip landingHeavy = null;

    private PlayerInputReader controls;
    private CharacterController cc;
    private PlayerState currentState;
    private VirtualAudioSource audioSource;
    private Vector3 moveDirection;

    public float WalkSpeed => walkSpeed;
    public float Acceleration => acceleration;
    public float Decceleration => decceleration;
    public float Jump => jump;
    public float Gravity => gravity;
    public float SlopeForce => slopeForce;
    public AudioClip Footsteps => footsteps;
    public AudioClip LandingLight => landingLight;
    public AudioClip LandingHeavy => landingHeavy;
    public float SlopeAngle { get; set; }
    public Vector3 SlopeTangent { get; set; }
    public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }


    private void Awake()
    {
        controls = GetComponent<PlayerInputReader>();
        cc = GetComponent<CharacterController>();
        audioSource = GetComponents<VirtualAudioSource>()[0]; // { movement_sounds, weapon_sounds }
        SwitchState(new AirState(this, cc, cam, controls));
    }

    private void Update()
    {
        //Update State & Apply Motion
        currentState.Update();
        cc.Move(MoveDirection * Time.deltaTime);
        
        //Debug.Log($"CC: {cc.velocity}. MoveDir: {moveDirection}. CC: {cc.isGrounded}. {IsGrounded()}.");
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

    public void PlayClip(AudioClip clip)
    {
        audioSource.Play(clip);
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
            Vector3 u = Vector3.Cross(hit.normal, Vector3.down).normalized;
            SlopeTangent = Vector3.Cross(u, hit.normal);
            SlopeAngle = Vector3.Angle(Vector3.up, hit.normal);
        }
        gameObject.layer = 9;

        //Grounded Detection
        bool grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask, QueryTriggerInteraction.Ignore);
        return grounded;
    }

    public bool CurrentStateIs(Type type)
    {
        return currentState.GetType() == type;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Start falling if head is hit.
        gameObject.layer = 2;
        if (Physics.CheckSphere(transform.position + Vector3.up * (0.8f), 0.5f, groundMask, QueryTriggerInteraction.Ignore))
        {
            moveDirection.y = -6f;
        }
        gameObject.layer = 9;

        //Check for edge hanging and push player back.
        if (cc.isGrounded == true && IsGrounded() == false)
        {
            cc.Move(hit.normal * 0.1f);
        }

        //Push Props
        if (hit.gameObject.layer == 12 && hit.gameObject.GetComponent<Prop>() != null)
        {
            hit.gameObject.GetComponent<Prop>().Hit(hit.point, cc.velocity * 0.3f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (.8f), 0.5f);
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Ray(transform.position, SlopeTangent));
    }
}
