using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody rb;
    

    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    private Vector3 moveVec;
    
    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        moveVec = new Vector3(-controller.moveFB, 0, controller.moveLR) * moveSpeed * Time.deltaTime;
        Move();
        Rotate();
    }

    private void Move()
    {
        var position = rb.position;
        position += moveVec;
        rb.MovePosition(position);
    }

    private void Rotate()
    {
        if (moveVec.sqrMagnitude == 0)
            return;

        var rotation = rb.rotation;
        var dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, 180f);
        rb.MoveRotation(moveQuat);
    }
}
