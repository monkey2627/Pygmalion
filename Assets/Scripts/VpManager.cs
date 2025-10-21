using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SaveVPData
{
    public List<Vp> snapshots;
}
[Serializable]     
public class Vp
{   //为了持久化
    public string name;       
    public Vector3 localPos;
    public Vector3 localScale;
    public Color color;
}
public class VpManager : MonoBehaviour
{
    public static VpManager Instance;
    private static readonly string SaveFile = "SaveVPData.json";
    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<string, GameObject> vps = new Dictionary<string, GameObject>();
    public GameObject[] vpGameObjects;
    private void Start()
    {
        vps = new Dictionary<string, GameObject>
        {
            ["elpis"] = vpGameObjects[0],
            ["ym"]   = vpGameObjects[1]
        };

    }

    /// <summary>
    /// 开始新游戏就将其全部变成透明再说,然后覆盖一下存档
    /// </summary>
    public void StartNewGame()
    {
        foreach (var (key, value) in vps)
        {
            value.GetComponent<SpriteRenderer>().color = new Color(value.GetComponent<SpriteRenderer>().color.r, value.GetComponent<SpriteRenderer>().color.g, value.GetComponent<SpriteRenderer>().color.b, 0f);
        }
        SaveAllVp(0);
    }
    public void Fade(string obj, float fadeTime, float target)
    {
        vps[obj].GetComponent<SpriteRenderer>().DOFade(target, fadeTime);
    }

    public void Move(string obj, float moveTime, Vector3 target)
    {
        print(target);
        if (moveTime == 0)
        {
            vps[obj].transform.localPosition = target;
            return;
        }
        vps[obj].transform.DOLocalMove(target, moveTime);
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
                BlinkForSeconds(vps["elpis"].GetComponent<SpriteRenderer>(),2,0.7f,new Vector3(-556.5f,-540.1f,0));
                break;
        }
    }

    public GameObject DelayGameObject;
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
            //到海边
            sr.gameObject.transform.localPosition = target;
            sr.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1);
            //播放转场动画G=
            DelayGameObject.transform.DOMove(new(0, 0, 0), 0.1f).OnComplete(() =>
            {
                sr.DOFade(0, 4);
                GameManager.Instance.ReadLine();
            });
        });
    }
    public void SaveAllVp(int type)
    {
        if (type == 0)
        {
            List<Vp> list = new();
            foreach (var kvp in vps)
            {
                var tr = kvp.Value.transform;
                var sr = kvp.Value.GetComponent<SpriteRenderer>();
                list.Add(new Vp
                {
                    name = kvp.Key,
                    localPos = tr.localPosition,
                    localScale = tr.localScale,
                    color = sr.color
                });
            }

            SaveVPData data = new() { snapshots = list };
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFile), json);
            Debug.Log($"[VpManager] 已保存 {list.Count} 条 vp 数据");
        }
    }

    // 从磁盘读出并覆盖当前 vp 的状态
    public void LoadAllVp()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFile);
        if (!File.Exists(path))
        {
            Debug.Log("[VpManager] 没有找到存档文件，跳过读档");
            return;
        }

        string json = File.ReadAllText(path);
        SaveVPData data = JsonUtility.FromJson<SaveVPData>(json);

        foreach (var snap in data.snapshots)
        {
            if (!vps.ContainsKey(snap.name)) continue;

            var tr = vps[snap.name].transform;
            var sr = vps[snap.name].GetComponent<SpriteRenderer>();

            tr.localPosition = snap.localPos;
            tr.localScale    = snap.localScale;
            sr.color         = snap.color;
        }
        Debug.Log($"[VpManager] 已载入 {data.snapshots.Count} 条 vp 数据");
    }
}
