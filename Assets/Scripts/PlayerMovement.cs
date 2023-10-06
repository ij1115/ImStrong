using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController controller;
    private PlayerInfo playerInfo;
    private PlayerWeapons weapons;
    private Animator ani;
    private Rigidbody rb;

    public float rotateSpeed = 180f;

    public CinemachineVirtualCamera vCamera;
    private Camera worldCam;

    private Vector3 moveVec;

    public void Setup()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        weapons = GetComponent<PlayerWeapons>();
        ani = GetComponent<Animator>(); 
        playerInfo = GetComponent<PlayerInfo>();
        worldCam = Camera.main;
        ani.runtimeAnimatorController = weapons.GetAni();
        //cameraRot = vCamera.transform.rotation;
        //cameraRot.x = 0;
        //cameraRot.z = 0;
    }

    private void FixedUpdate()
    {
        if (controller ==null)
            return;


        //Vector3 dir = new Vector3(controller.moveLR, 0, controller.moveFB);

        //moveVec = cameraRot * dir;
        //Debug.Log($"Àü : {moveVec}");
        //if (moveVec.magnitude > 1f)
        //{
        //    moveVec.Normalize();
        //}
        //Debug.Log($"ÈÄ{moveVec}");

        Move();
        Rotate();

        ani.SetFloat("Move", moveVec.magnitude);
    }

    private void Update()
    {
        if (playerInfo.unitState == UnitState.NIdle || playerInfo.unitState == UnitState.Idle)
        {
            var forward = worldCam.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = worldCam.transform.right;
            right.y = 0f;
            right.Normalize();

            moveVec = forward * controller.moveFB;
            moveVec += right * controller.moveLR;

            if (moveVec.magnitude > 1f)
            {
                moveVec.Normalize();
            }
        }
    }

    private void Move()
    {
        var position = rb.position;
        position += moveVec * StateManager.Instance.current.movSp * Time.deltaTime;
        rb.MovePosition(position);
    }

    private void Rotate()
    {
        //if (moveVec.sqrMagnitude == 0)
        //    return;

        //var dirQuat = Quaternion.LookRotation(moveVec * moveSpeed * Time.deltaTime);
        //Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, rotateSpeed);
        //rb.MoveRotation(moveQuat);


        if (moveVec == Vector3.zero)
            return;

        var rotation = rb.rotation;
        var targetRotateion = Quaternion.LookRotation(moveVec, Vector3.up);
        rotation = Quaternion.RotateTowards(rotation, targetRotateion, rotateSpeed * Time.deltaTime);
        rb.MoveRotation(rotation);

    }
}
