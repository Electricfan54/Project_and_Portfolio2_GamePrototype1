using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamage, IStatuseffect
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
    [SerializeField] float InvincTimer;

    [Tooltip("audio things")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] jumpsounds;
    [Range(0, 1)][SerializeField] float jumpvolume;

    [SerializeField] AudioClip[] damagesounds;
    [Range(0, 1)][SerializeField] float damagevolume;

    [SerializeField] AudioClip[] walkingsounds;
    [Range(0, 1)][SerializeField] float walkingvolume;

    [SerializeField] float timetostep;
    [SerializeField] float timetorun;
    bool isplayingsteps;
    float steptimer;



    Vector3 playerDirection;
    Vector3 playerVel;

    int jumpCount;
    bool isLauched = false;

    bool isInvis = false;
    bool isrunning;
    int origHP;
    [Tooltip("Effect variables")]

    float durationtimer;
    float ticktimer;
    public bool hasStatusEffect;
    int dur;
    int tic;
    int effdam;
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
        if (hasStatusEffect)
        {
            ticktimer += Time.deltaTime;
            durationtimer += Time.deltaTime;
            ApplyEffect(effdam, dur, tic);
        }
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            isLauched = false;
            jumpCount = 0;
            playerVel = Vector3.zero;
            if (playerDirection.normalized.magnitude > 0.3f && !isplayingsteps)
            {
                StartCoroutine(playstep());
            }
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
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            aud.PlayOneShot(jumpsounds[Random.Range(0, jumpsounds.Length)], jumpvolume);
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            isrunning = true;

            playerSpeed *= SprintMod;

        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= SprintMod;
            isrunning = false;
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
        gameManager.instance.playerHPBar.fillAmount = (float)PlayerHP / origHP;
    }

    void TestLauch()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {

            LauchPlayer(new Vector3(0, 1, 1));
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvis == false)
        {
            PlayerHP -= damageAmount;
            aud.PlayOneShot(damagesounds[Random.Range(0, damagesounds.Length)], damagevolume);
            UpdatePlayerHPUI();
            StartCoroutine(playerFlashDamage());
            StartCoroutine(IFrames());

            if (PlayerHP <= 0)
            {
                gameManager.instance.GameOver();
            }
        }
    }

    IEnumerator playerFlashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }

    IEnumerator IFrames()
    {
        isInvis = true;
        yield return new WaitForSeconds(InvincTimer);
        isInvis = false;
    }
    
    public void HealPlayerOnKill()
    {
        PlayerHP += 2;
        if (PlayerHP >= origHP)
        {
            PlayerHP = origHP;
        }
        UpdatePlayerHPUI();

    }

    public void ApplyEffect(int damage, int durration, int tickspeed)
    {
        effdam = damage;
        dur = durration;
        tic = tickspeed;

        if (hasStatusEffect)
        {
            if (ticktimer >= tic && durationtimer <= dur)
            {
                TakeDamage(effdam);
                ticktimer = 0;
            }
            if (durationtimer >= dur)
            {
                hasStatusEffect = false;
                durationtimer = 0;
                ticktimer = 0;
            }
        }
    }

    IEnumerator playstep()
    {
        isplayingsteps = true;
        aud.PlayOneShot(walkingsounds[Random.Range(0, walkingsounds.Length)], walkingvolume);
        if (isrunning)
        {
            yield return new WaitForSeconds(timetorun);
        }
        else
        {
            yield return new WaitForSeconds(timetostep);
        }
        isplayingsteps = false;
    }
}
