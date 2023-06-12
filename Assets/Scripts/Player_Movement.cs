using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("References")]
    CharacterController controller;
    [SerializeField] Animator weaponSocketAnimator;

    [Header("Ground Movement")]
    [SerializeField] float groundMoveSpeed = 13f;
    [SerializeField] float movementSmoothTime = 0.1f;

    [Header("Air Movement")]
    [SerializeField] float airAcceleration = 0.5f;
    [SerializeField] float airDeceleration = 0.3f;
    [SerializeField] float maxAirMoveSpeed = 6f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 20f;

    [Header("Gravity")]
    [SerializeField] float gravityInAir = -9.81f;

    [Header("Audio")]
    [SerializeField] AudioClip[] footstepSounds;

    // Input
    float horizontalInput;
    float verticalInput;
    Vector3 moveInput;
    Vector3 currentDirection;
    Vector3 currentDirVelocity;

    // Movement
    Vector3 velocity;
    float gravity;
    bool hasJumped;
    bool resetVelocityInAir = false;

    // Audio
    AudioSource audioSource;

    // FUNCTIONS

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        GetMoveInput();
        GetJumpInput();

        CalcVelocity();
        
        ApplyMovement();

        // TODO: add to own file
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void GetMoveInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveInput = transform.forward * verticalInput + transform.right * horizontalInput;
        moveInput.Normalize();

        // set weapon animation
        weaponSocketAnimator.SetBool("IsMoving", moveInput != Vector3.zero);

        // smooth movement on ground
        currentDirection = Vector3.SmoothDamp(currentDirection, moveInput, ref currentDirVelocity, movementSmoothTime);
    }

    void GetJumpInput()
    {
        hasJumped = Input.GetButtonDown("Jump");
    }

    void CalcVelocity()
    {
        if (controller.isGrounded) // movement on ground
        {
            resetVelocityInAir = false;

            gravity = 0f;

            // instant acceleration (setting movement instead of adding it)
            velocity = currentDirection * groundMoveSpeed;

            // jump velocity
            if (hasJumped)
            {
                // add upwards jump force
                velocity.y = jumpForce;

                hasJumped = false;
            }
            else
            {
                // keeps player on slopes when moving down them
                velocity.y = -9f;
            }
        }
        else // movement in air
        {
            // reset slope velocity to 0 as soon as player is in air (only done once so velocity isn't overridden)
            // don't reset y velocity if y velcoity is more than 0 (jumping)
            if (!resetVelocityInAir && velocity.y < 0) velocity.y = 0;
            resetVelocityInAir = true;

            gravity = gravityInAir;

            if (moveInput == Vector3.zero) // no player input
            {
                // horizontal velocity should slowly decelerate when input is stopped in air
                velocity -= currentDirection * airDeceleration;
            }
            else
            {
                // accelerate movement in air
                velocity += currentDirection * airAcceleration;
            }

            // limit horizontal air speed
            float verticalVelocity = velocity.y;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxAirMoveSpeed);
            velocity = horizontalVelocity + (Vector3.up * verticalVelocity);
        }

        // apply gravity to velocity
        velocity.y += gravity * Time.deltaTime;
    }

    void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);
    }

    public void PlayFootstepSound(int foot)
    {
        if (controller.isGrounded && moveInput != Vector3.zero)
        {
            audioSource.pitch = Random.Range(0.8f, 1.1f);
            if (foot == 0) audioSource.PlayOneShot(footstepSounds[0]);
            else audioSource.PlayOneShot(footstepSounds[1]); ;
        }
    }
}
