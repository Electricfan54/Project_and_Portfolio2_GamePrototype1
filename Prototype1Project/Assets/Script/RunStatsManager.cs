using UnityEngine;
using TMPro;

public class RunStatsManager : MonoBehaviour
{

    public static RunStatsManager instance;

    [Header("Stat Menu Labels")]
    [SerializeField] public TMP_Text elimLabel;
    [SerializeField] public TMP_Text hitLabel;
    [SerializeField] public TMP_Text timeLabel;
    [SerializeField] public TMP_Text healthLabel;
    [SerializeField] public TMP_Text damageLabel;
    [SerializeField] public TMP_Text speedLabel;
    [SerializeField] public TMP_Text siphonLabel;

    [Header("Run Stat Values")]
    public int currElims;
    public int currHit;
    public int currHealth;
    public int currDamage;
    public int currSpeed;
    public int currSiphon;

    [Header("Timer Values")]
    int hours, minutes, seconds;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        UpdateTimer();
    }

    void UpdateTimer()
    {
        seconds++;

        if (seconds >= 60)
        {
            seconds = 0;
            minutes++;

            if (minutes >= 60)
            {
                minutes = 0;
                hours++;
            }
        }
    }

    public void UpdateUI()
    {
        elimLabel.text = currElims.ToString("F0");
        hitLabel.text = currHit.ToString("F0");

        healthLabel.text = currHealth.ToString("F0");
        damageLabel.text = currDamage.ToString("+F0");
        speedLabel.text = currSpeed.ToString("+F0");
        siphonLabel.text = currSiphon.ToString("F0");

        timeLabel.text = hours.ToString("F0") + ":" + minutes.ToString("F0") + seconds.ToString("F0");

    }

}
