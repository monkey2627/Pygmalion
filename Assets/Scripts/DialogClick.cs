using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogClick : MonoBehaviour,IPointerClickHandler
{
    public TW_MultiStrings_Regular regular;
    public bool enable = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (enable)
        {
            if (!regular.finishOneText)
            {
                regular.SkipTypewriter();
            }
            else
            {
                GameManager.Instance.ReadLine();
            }
        }
    }
}