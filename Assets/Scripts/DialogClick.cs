using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogClick : MonoBehaviour,IPointerClickHandler
{
    public TW_MultiStrings_Regular regular;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!regular.finishOneText)
        {
            regular.SkipTypewriter();
        }
        else {
            print(GameManager.Instance);
            GameManager.Instance.ReadLine(); 
        }
       
    }
}