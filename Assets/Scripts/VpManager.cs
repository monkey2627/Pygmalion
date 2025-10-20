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
    public GameObject[] vpGameObjects;
    private void Start()
    {
        vps = new Dictionary<string, GameObject>();
        print(vpGameObjects.Length);
        vps["elpis"] = vpGameObjects[0];
        vps["ym"] = vpGameObjects[1];
    }

    public void Fade(string obj, float fadeTime, float target)
    {
        vps[obj].GetComponent<SpriteRenderer>().DOFade(target, fadeTime);
    }

    public void Move(string obj, float moveTime, Vector3 target)
    {
        vps[obj].transform.DOMove(target, moveTime);
    }

    public void Scale(string obj, float scaleTime, Vector3 vector3)
    {
        vps[obj].transform.DOScale(vector3, scaleTime); 
    }

    public void Move2O(string obj)
    {
        switch (obj)
        {
            case "elpis":
                BlinkForSeconds(vps["elpis"].GetComponent<SpriteRenderer>(),1,0.3f,new Vector3(-556.5f,-540.1f,0));
                vps["elpis"].GetComponent<SpriteRenderer>().DOFade(1, 0.5f);
                break;
        }
    }
    public void BlinkForSeconds(SpriteRenderer sr,float seconds,float fadeDuration,Vector3 target)
    {
        // 1. 无限循环闪烁
        var tween = sr.DOFade(0, fadeDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // 2. 到点停止
        DOVirtual.DelayedCall(seconds, () =>
        {
            tween.Kill();                                      // 立即停
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1); // 复位不透明
            sr.gameObject.transform.position = target;
            sr.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        });
    }
}
