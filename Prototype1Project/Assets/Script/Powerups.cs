using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public struct Powerup
{
    public Sprite sprite;
    public string description;
    public float minVal, maxVal;
    public delegate void PowerupDelegate(Powerup powerup);
    public PowerupDelegate function;
}

public class Powerups : MonoBehaviour
{
    [Header("Required fields")]
    [SerializeField] Image powerupImage1;
    [SerializeField] TMP_Text powerupText1;
    [SerializeField] Image powerupImage2;
    [SerializeField] TMP_Text powerupText2;
    [SerializeField] Image powerupImage3;
    [SerializeField] TMP_Text powerupText3;

    [SerializeField] Powerup healthPowerup;
    [SerializeField] Powerup damagePowerup;
    [SerializeField] Powerup speedPowerup;
    [SerializeField] Powerup lifestealPowerup;

    //Variables for the current powerups on screen
    Powerup powerup1;
    Powerup powerup2;
    Powerup powerup3;

    void Start()
    {

        healthPowerup.function = TestPowerup;
        healthPowerup.function(healthPowerup);
    }

    void Update()
    {
        
    }

    public void SetRandomPowerups()
    {
        List<Powerup> powerups = new List<Powerup>{ healthPowerup, damagePowerup, speedPowerup, lifestealPowerup };

        powerup1 = GetRandPowerup(powerups);
        powerup2 = GetRandPowerup(powerups);
        powerup3 = GetRandPowerup(powerups);

        ShowPowerups();
    }

    Powerup GetRandPowerup(List<Powerup> powerups)
    {
        int val = Random.Range(0, powerups.Count);
        Powerup returnVal = powerups[val];
        powerups.RemoveAt(val);
        return returnVal;
    }

    void ShowPowerups()
    {
        //Update image and text
        powerupImage1.sprite = powerup1.sprite;
        powerupImage2.sprite = powerup2.sprite;
        powerupImage3.sprite = powerup3.sprite;

        powerupText1.text = powerup1.description;
        powerupText2.text = powerup2.description;
        powerupText3.text = powerup3.description;
    }

    void HealthPowerUp(Powerup powerup)
    {
        //gameManager.instance.playerScript.
    }

    void TestPowerup(Powerup powerup)
    {
        Debug.Log(powerup.description);
    }

    public void GivePowerup1()
    {
        powerup1.function(powerup1);
    }

    public void GivePowerup2()
    {
        powerup2.function(powerup2);
    }

    public void GivePowerup3()
    {
        powerup3.function(powerup3);
    }

}
