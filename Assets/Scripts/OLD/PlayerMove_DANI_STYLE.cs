using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] CharacterController controller;

    [Header("Movement")]
    [SerializeField] float groundMoveSpeed = 9f;
    [SerializeField] float airMoveSpeed = 4f;
    [SerializeField] float gravityInAir = -9.81f;
    [SerializeField] float moveSmoothTime = 0.1f;

    [SerializeField] Transform vfx;

    // Input
    float horizontalInput;
    float verticalInput;
    Vector3 targetDirection;
    Vector3 currentDirection;
    Vector3 currentDirVelocity;

    // Movement
    float moveSpeed;
    Vector3 velocity;
    float gravity;

    void Update()
    {
        GetInput();
        GroundDetection();
        CalcVelocity();

        ApplyMovement();
        vfx.position = transform.position;
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        targetDirection = vfx.forward * verticalInput + vfx.right * horizontalInput;
        targetDirection.Normalize();

        currentDirection = Vector3.SmoothDamp(currentDirection, targetDirection, ref currentDirVelocity, moveSmoothTime);
    }

    void GroundDetection()
    {
        if (controller.isGrounded) {
            gravity = -0.1f;
            moveSpeed = groundMoveSpeed;
        } else {
            gravity = gravityInAir;
            moveSpeed = airMoveSpeed;
        }
    }

    void CalcVelocity()
    {
        // apply movement speed
        velocity = currentDirection * moveSpeed;

        // apply gravity to velocity
        velocity.y += gravity;
    }

    void ApplyMovement()
    {
        controller.Move(velocity * Time.deltaTime);
    }
}
