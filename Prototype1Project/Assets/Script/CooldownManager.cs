using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;
using System.Collections;
using Unity.VisualScripting;
using System.Drawing;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] RectTransform CoolDownPos1;
    [SerializeField] RectTransform CoolDownPos2;
    [SerializeField] RectTransform CoolDownPos3;

    Vector2 OriginalSize = new Vector2(380, 80);

    public void AddCoolDown(String Text, float CDTimer)
    {
        if (CoolDownPos1.gameObject.activeSelf == false)
        {
            CoolDownPos1.gameObject.SetActive(true);
            CoolDownPos1.GetComponentInChildren<TMP_Text>().text = Text;
            StartCoroutine(CDUI(CoolDownPos1, CDTimer));
        }
        else if (CoolDownPos1.gameObject.activeSelf == true && CoolDownPos2.gameObject.activeSelf == false)
        {
            CoolDownPos2.gameObject.SetActive(true);
            CoolDownPos2.GetComponentInChildren<TMP_Text>().text = Text;
            StartCoroutine(CDUI(CoolDownPos2, CDTimer));
        }
        else if (CoolDownPos1.gameObject.activeSelf == true && CoolDownPos2.gameObject.activeSelf == true && CoolDownPos3.gameObject.activeSelf == false)
        {
            CoolDownPos3.gameObject.SetActive(true);
            CoolDownPos3.GetComponentInChildren<TMP_Text>().text = Text;
            StartCoroutine(CDUI(CoolDownPos3, CDTimer));
        }

    }

    IEnumerator CDUI(RectTransform UIChange, float Timer)
    {
        float curTime = 0;
        Vector2 StartSize = UIChange.sizeDelta;
        float Width = UIChange.sizeDelta.x;

        while (curTime < Timer)
        {
            curTime += Time.deltaTime;
            float Tweenpercent = curTime / Timer;

            StartSize.x = Mathf.Lerp(Width, 0f, Tweenpercent);
            UIChange.sizeDelta = StartSize;
            yield return null;
        }

        StartSize.x = 0f;
        UIChange.sizeDelta = StartSize;
        UIChange.gameObject.SetActive(false);
        UIChange.sizeDelta = OriginalSize;
        

    }
}
