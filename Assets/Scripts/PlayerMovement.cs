using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private Rigidbody rb;

    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
        var position = rb.position;

        position = rb.position + transform.forward * controller.moveFB * moveSpeed * Time.deltaTime;
        rb.MovePosition(position);
    }

    private void Rotate()
    {
        var rotation = rb.rotation;

        rotation *= Quaternion.Euler(Vector3.up * controller.moveLR * rotateSpeed * Time.deltaTime);
        rb.MoveRotation(rotation);
    }
}
