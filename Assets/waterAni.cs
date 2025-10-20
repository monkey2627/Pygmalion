using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class waterAni : MonoBehaviour
{
    public string target;
    public static waterAni Instance;
    public Dictionary<string,GameObject> roles;
    public GameObject[] roleArray;

    private void Awake()
    {
        Instance  = this;
    }

    public void showTarget()
    {
        switch (target)
        {
            case "elpis":
                roles["elpis"].SetActive(true);
                roles["elpis"].GetComponent<SpriteRenderer>().DOFade(0.5f,0);
                roles["elpis"].GetComponent<SpriteRenderer>().DOFade(1, 0.167f);
                break;
        }
    }

    public void showJellyfish()
    {
        
        
    }
}
