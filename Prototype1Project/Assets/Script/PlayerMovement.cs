using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int playerSpeed;
    [SerializeField] int SprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int Gravity;
    
    Vector3 playerDirection;
    Vector3 playerVel;

    int jumpCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Sprint();
    }

    void Movement()
    {
        if(controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else
        {
            playerVel.y -= Gravity * Time.deltaTime;

        }

        playerDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(playerDirection * playerSpeed * Time.deltaTime);

        Jump();

        controller.Move(playerVel * Time.deltaTime);

    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount <  jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= SprintMod;

        }
        else if(Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= SprintMod;
        }
    }
}
