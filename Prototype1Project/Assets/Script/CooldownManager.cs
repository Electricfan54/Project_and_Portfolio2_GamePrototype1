using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] GameObject CoolDownPos1;
    [SerializeField] GameObject CoolDownPos2;
    [SerializeField] GameObject CoolDownPos3;

    public void AddCoolDown(string Text)
    {
        if (CoolDownPos1.activeSelf == false)
        {
            CoolDownPos1.SetActive(true);
            
        }
    }
}
