using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] GameObject CoolDownPos1;
    [SerializeField] GameObject CoolDownPos2;
    [SerializeField] GameObject CoolDownPos3;

    public void AddCoolDown(String Text)
    {
        if (CoolDownPos1.activeSelf == false)
        {
            CoolDownPos1.SetActive(true);
            CoolDownPos1.GetComponentInChildren<TMP_Text>().text = Text;
            
        }
    }
}
