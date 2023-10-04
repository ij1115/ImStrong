using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody rb;

    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;

    public CinemachineVirtualCamera vCamera;
    private Vector3 moveVec;
    private Quaternion cameraRot;

    
    // Start is called before the first frame update
    public void Setup()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        cameraRot = vCamera.transform.rotation;
        cameraRot.x = 0;
        cameraRot.z = 0;
    }

    private void FixedUpdate()
    {
        if (controller ==null)
            return;

        Vector3 dir = new Vector3(controller.moveLR, 0, controller.moveFB);

        moveVec = cameraRot * dir;

        if (moveVec.magnitude > 1f)
        {
            moveVec.Normalize();
        }

        Move();
        Rotate();
    }

    private void Move()
    {
        var position = rb.position;
        position += moveVec * moveSpeed * Time.deltaTime;
        rb.MovePosition(position);
    }

    private void Rotate()
    {
        if (moveVec.sqrMagnitude == 0)
            return;

        var dirQuat = Quaternion.LookRotation(moveVec * moveSpeed * Time.deltaTime);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, rotateSpeed);
        rb.MoveRotation(moveQuat);
    }
}
