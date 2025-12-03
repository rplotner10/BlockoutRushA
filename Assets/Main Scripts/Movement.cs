using UnityEngine;
using UnityEngine.ProBuilder;

public class Movement : MonoBehaviour

{
//Movement + Jump//
private Rigidbody rb;
public float MoveSpeed = 15f;
private float moveHorizontal;
private float moveForward;
public float jumpForce = 20f;
public float fallMultiplier = 2.5f;
public float upFloatMultiplier = 2f;
//Ground Checks
public LayerMask groundLayer;
private float playerHeight;
private float raycastDistance;



void Start()
{
    rb = GetComponent<Rigidbody>();
    rb.freezeRotation = true;
    playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
    raycastDistance = (playerHeight / 2) + 0.2f;
}

void Update()
{
    moveHorizontal = Input.GetAxisRaw("Horizontal");
    moveForward = Input.GetAxisRaw("Vertical");

    Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
    bool isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);
    
    if (Input.GetButtonDown("Jump"))
    {
        Jump();
    }
}

void FixedUpdate()
{
    MoveYourPlayer();
    ApplyJumpPhysics();
}

void MoveYourPlayer()
{
    Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
    Vector3 targetVelocity = movement * MoveSpeed;
    
    Vector3 velocity = rb.linearVelocity;
    velocity.x = targetVelocity.x;
    velocity.z = targetVelocity.z;
    rb.linearVelocity = velocity;
}

void Jump()
{
    rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
}

void ApplyJumpPhysics()
{
    if (rb.linearVelocity.y < 0)
    {
        rb.linearVelocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.fixedDeltaTime;
    }
    else if (rb.linearVelocity.y > 0)
    {
        rb.linearVelocity += Vector3.up * Physics.gravity.y * upFloatMultiplier * Time.fixedDeltaTime;
    }
}

}

