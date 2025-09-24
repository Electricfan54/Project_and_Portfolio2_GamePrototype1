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
    [HideInInspector] public float value;

    public delegate void PowerupDelegate(Powerup powerup);
    public PowerupDelegate function;
}

public class Powerups : MonoBehaviour
{
    [Header("Required fields")]
    [SerializeField] Image powerupImage1;
    [SerializeField] Image powerupImage2;
    [SerializeField] Image powerupImage3;

    [SerializeField] TMP_Text powerupText1;
    [SerializeField] TMP_Text powerupText2;
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

        healthPowerup.function = HealthPowerup;
        damagePowerup.function = HealthPowerup;
        speedPowerup.function = SpeedPowerup;
        lifestealPowerup.function = LifestealPowerup;

        //SetRandomPowerups();
        //GivePowerup1();

    }

    void Update()
    {
        
    }

    //Randomizes and sets the visuals for the powerup screen
    //Call this when diplaying the ability select screen
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
        returnVal.value = Random.Range(returnVal.minVal, returnVal.maxVal);
        //round to the nearest hundredth place
        returnVal.value = Mathf.Round(returnVal.value * 100.0f) / 100.0f;
        powerups.RemoveAt(val);
        return returnVal;
    }

    void ShowPowerups()
    {
        //Update image and text
        powerupImage1.sprite = powerup1.sprite;
        powerupImage2.sprite = powerup2.sprite;
        powerupImage3.sprite = powerup3.sprite;

        powerupText1.text = powerup1.description + " " + powerup1.value.ToString();
        powerupText2.text = powerup2.description + " " + powerup2.value.ToString();
        powerupText3.text = powerup3.description + " " + powerup3.value.ToString();
    }

    void HealthPowerup(Powerup powerup)
    {
        Debug.Log(powerup.description);
        gameManager.instance.playerScript.AddMaxHealthMult(powerup.value);
    }

    void DamagePowerup(Powerup powerup)
    {
        //gameManager.instance.playerScript
    }

    void SpeedPowerup(Powerup powerup)
    {
        gameManager.instance.playerScript.AddSpeedBuff(powerup.value);
    }

    void LifestealPowerup(Powerup powerup)
    {
        gameManager.instance.playerScript.AddHealingBuff(powerup.value);
    }

    void TestPowerup(Powerup powerup)
    {
        Debug.Log(powerup.description);
    }

    //Call this to give the player the first powerup
    public void GivePowerup1()
    {
        powerup1.function(powerup1);
    }

    //Call this to give the player the second powerup
    public void GivePowerup2()
    {
        powerup2.function(powerup2);
    }

    //Call this to give the player the third powerup
    public void GivePowerup3()
    {
        powerup3.function(powerup3);
    }

}
