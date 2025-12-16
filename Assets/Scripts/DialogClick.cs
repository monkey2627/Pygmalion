using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogClick : MonoBehaviour
{
    public TW_MultiStrings_Regular regular;
    public bool enable = true;
    public void Click()
    {
        if (enable)
        {
            if (!regular.finishOneText)
            {
                regular.SkipTypewriter();
            }
            else
            {
                PygmalionGameManager.instance.ReadLine();
            }
        }
    }
}