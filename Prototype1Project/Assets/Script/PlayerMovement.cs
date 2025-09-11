using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;

    [SerializeField] int PlayerHP;
    [SerializeField] int playerSpeed;
    [SerializeField] int SprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int Gravity;

    [Tooltip("SidewaysDownTime is how much you want to subtract from the sideways vector (x,z) until it reaches 0")]
    [SerializeField] int SidewaysDownTime;
    [SerializeField] float LauchForceMult;
    
    Vector3 playerDirection;
    Vector3 playerVel;

    int jumpCount;
    bool isLauched = false;

    int origHP;
    void Start()
    {
        origHP = PlayerHP;
        UpdatePlayerHPUI();
    }

    void Update()
    {
        Movement();
        Sprint();
        CheckLauch();
    }

    void Movement()
    {
        if(controller.isGrounded)
        {
            isLauched = false;
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
        TestLauch();
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
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= SprintMod;

        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= SprintMod;
        }
    }

    public void LauchPlayer(Vector3 Lauchdirection)
    {
        playerVel = Lauchdirection * LauchForceMult;
        isLauched = true;
       
        
    }

    void CheckLauch()
    {
        if (!isLauched)
        {
            return;
        }

        playerVel.x -= SidewaysDownTime * Time.deltaTime;
        playerVel.z -= SidewaysDownTime * Time.deltaTime;

        if (playerVel.x <= 0 || playerVel.z <= 0)
        {
            isLauched = false;
            return;
        }
    }

 
    public void UpdatePlayerHPUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)PlayerHP/origHP;
    }

    void TestLauch()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            
            LauchPlayer(new Vector3(0,1,1));
        }
    }

    void IDamage.TakeDamage(int damageAmount)
    {
        PlayerHP -= damageAmount;
        UpdatePlayerHPUI();
        StartCoroutine(playerFlashDamage());

        if (PlayerHP <= 0)
        {
            gameManager.instance.GameOver();
        }

    }

    IEnumerator playerFlashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }


    public void HealPlayerOnKill()
    {
        PlayerHP += 2;
        if (PlayerHP >= origHP)
        { 
            PlayerHP = origHP;
        }

    }
}
