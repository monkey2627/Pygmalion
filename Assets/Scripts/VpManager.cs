using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class VpManager : MonoBehaviour
{
    public static VpManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, GameObject> vps = new Dictionary<string, GameObject>();
    private GameObject[] vpGameObjects;
    private void Start()
    {
        vps["Elpis"] = vpGameObjects[0];
    }

    public void Fade(string obj, float fadeTime, float target)
    {
        vps[obj].GetComponent<SpriteRenderer>().DOFade(target, fadeTime);
    }

    public void Move(string obj, float moveTime, Vector3 target)
    {
        vps[obj].transform.DOMove(target, moveTime);
    }
}
