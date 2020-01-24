using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nollie : MonoBehaviour
{
    float jumpTimer;
    public float tiltSpeed = 10.0f;
    public bool canJump = false;
    public bool crouched = false;
    public bool jump;
    public bool turning = false;
    [SerializeField] private float slopeForce = 15;
    [SerializeField] private float slopeForceRayLength = 1;

    public float speed = 6.0f;
    public float jumpSpeed = 6.0f;
    public float gravity = 10.0f;
    RaycastHit hit;

    CharacterController mover;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        mover = GetComponent<CharacterController>();
    }

    void Update()
    {
        //print(moveDirection.y);
        //print(turning);
        if (mover.isGrounded)
        {
            gravity = 30f;
            //turning = false;
            if (crouched == false && canJump == false)
            {
                jumpSpeed = 10.0f;
            }

            float leftRight = Input.GetAxis("Horizontal");

            //moveDirection = transform.forward * 6;
            if (turning == true)
            {
                transform.Rotate(0, Mathf.Lerp(0, leftRight * 4, 1.0f), 0, Space.Self);
            }
            

            moveDirection = transform.forward * 4;



            if (Input.GetKeyDown("left") || Input.GetKeyDown("right"))
            {
                turning = true;
            }

            if (Input.GetKeyUp("left") || Input.GetKeyUp("right"))
            {
                turning = false;
            }

            moveDirection *= speed;

            if (jump == true)
            {
                moveDirection.y += jumpSpeed;
            }
            if (Input.GetKey("down"))
            {
                if (jumpSpeed < 13)
                {
                    jumpSpeed += Time.deltaTime * 6;
                }

                crouched = true;
            }
            if (Input.GetKeyUp("down"))
            {
                crouched = false;
                canJump = true;
                jumpTimer = 3;
            }
        }


        if (turning == false)
        {
            if (transform.rotation.z > 0)
            {
                transform.Rotate(0, 0, -tiltSpeed, Space.Self);
            }
            if (transform.rotation.z < 0)
            {
                transform.Rotate(0, 0, tiltSpeed, Space.Self);
            }
        }

        if (!mover.isGrounded)
        {
            if (moveDirection.y <= 3)
            {
                gravity = 37;
            }
            if (moveDirection.y <= 0)
            {
                gravity = 40;
            }
            float airLeftRight = Input.GetAxis("Horizontal");
            moveDirection.x += airLeftRight / 2;

        }
        print(OnSlope());
        print(hit.normal.y);
        if (OnSlope())
        {
            moveDirection.y -= slopeForce;
            //speed = hit.normal.y;
        }
 

        if (canJump == true)
        {
            JumpTimer();

        }
        moveDirection.y -= gravity * Time.deltaTime;
        mover.Move(moveDirection * Time.deltaTime);
    }

    private bool OnSlope()
    {
        if(jump == true)
        {
            return false;
        }

        
        Debug.DrawRay(transform.position, Vector3.down * slopeForceRayLength, Color.green);

        if (Physics.Raycast(transform.position, Vector3.down, out hit, mover.height / 2 * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }



    void JumpTimer()
    {
        jumpTimer -= Time.deltaTime * 50;

        //print(jumpTimer);

        if (jumpTimer > 0 && Input.GetKey("up"))
        {
            jump = true;
            //print(jumpTimer);
        }

        if (jumpTimer <= 0)
        {
            //print("cant");
            canJump = false;
            jump = false;
        }
    }
}
