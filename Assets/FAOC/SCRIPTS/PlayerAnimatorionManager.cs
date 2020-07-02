using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerAnimatorionManager : MonoBehaviourPun
{

    public CharacterController controller;
    public float gravityScale = 1;
    public float moveSpeed = 1;
    public float rotateSpeed = 5;
    Animator animator;
    Vector3 inputVec;
    private Vector3 moveDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Prevent control is connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Check to see if the right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X") * rotateSpeed;
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, x, 0));
            transform.Rotate(deltaRotation.eulerAngles);
        }

        moveDirection = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        moveDirection = moveDirection.normalized * moveSpeed;

        moveDirection.y = moveDirection.y + (Physics.gravity.y * gravityScale * Time.deltaTime);
        controller.Move(moveDirection * Time.deltaTime);

        animator.SetBool("forward", false);
        animator.SetBool("backward", false);
        animator.SetBool("strafeRight", false);
        animator.SetBool("strafeLeft", false);

        if (v > 0)
        {
            animator.SetBool("forward", true);
        }
        else if (v < 0)
        {
            animator.SetBool("backward", true);
        }
        else if (h > 0)
        {
            animator.SetBool("strafeRight", true);
        }
        else if (h < 0)
        {
            animator.SetBool("strafeLeft", true);
        }
    }
}
