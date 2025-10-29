using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SaveVPData
{
    public string enow;
    public string ymnow;
    public string elpisnow;
    public Vector3 ympose;
    public Vector3 epose;
    public Vector3 elpispose;
    public Vector3 ymScale;
    public Vector3 eScale;
    public Vector3 elpisScale;
    public List<Vp> snapshots;
}
[Serializable]     
public class Vp
{   //为了持久化
    public string role;
    public string name;       
    public Color color;
}

public struct RoleVp
{
    public string VpNow;//现在vp的名字
    public GameObject PosGameObject;//代表位置移动的空物体
    public Dictionary<String, GameObject> VpsDictionary;//vp字典
}
public class VpManager : MonoBehaviour
{
    public static VpManager instance;
    private static readonly string AutoSaveFile = "AutoSaveVPData.json";
    private static readonly string PersonSaveFile = "PersonSaveVPData.json";
    public GameObject ymGameObject;
    public GameObject eGameObject;
    public GameObject elpisGameObject;
    public Dictionary<String, GameObject> YmDictionary;
    public GameObject[] ymGameObjects;
    public Dictionary<String, GameObject> EDictionary;
    public Dictionary<String, GameObject> ElpisDictionary;
    public GameObject[] eGameObjects;
    public GameObject[] elpisGameObjects;
    private void Awake()
    {
        instance = this;
    }

    private Dictionary<string, RoleVp> _vps = new Dictionary<string, RoleVp>();
    private void Start()
    {
        _vps = new Dictionary<string,RoleVp>();
        RoleVp elpis = new RoleVp()
            { VpNow = "elpis", PosGameObject = elpisGameObject, VpsDictionary = new Dictionary<string, GameObject>() };
        foreach (GameObject g in elpisGameObjects)
        {
            elpis.VpsDictionary.Add(g.name, g);
        }
        RoleVp ym = new RoleVp()
            { VpNow = "ym", PosGameObject = ymGameObject, VpsDictionary = new Dictionary<string, GameObject>() };
        foreach (GameObject g in ymGameObjects)
        {
            ym.VpsDictionary.Add(g.name, g);
        }
        RoleVp e = new RoleVp()
            { VpNow = "e", PosGameObject = eGameObject, VpsDictionary = new Dictionary<string, GameObject>() };
        foreach (var variable in eGameObjects)
        {
            e.VpsDictionary.Add(variable.name, variable);
        }       
        _vps.Add("elpis",elpis);
        _vps.Add("ym", ym);
        _vps.Add("e",e);
    }

    /// <summary>
    /// 开始新游戏就将其全部变成透明再说,然后覆盖一下存档
    /// </summary>
    public void StartNewGame()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, AutoSaveFile));
        if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, PersonSaveFile));
        foreach (GameObject g in ymGameObjects)
        {
            g.GetComponent<SpriteRenderer>().color = new Color(g.GetComponent<SpriteRenderer>().color.r, g.GetComponent<SpriteRenderer>().color.g, g.GetComponent<SpriteRenderer>().color.b, 0f);
        }
        foreach (GameObject g in eGameObjects)
        {
            g.GetComponent<SpriteRenderer>().color = new Color(g.GetComponent<SpriteRenderer>().color.r, g.GetComponent<SpriteRenderer>().color.g, g.GetComponent<SpriteRenderer>().color.b, 0f);
        }
        foreach (GameObject g in elpisGameObjects)
        {
            g.GetComponent<SpriteRenderer>().color = new Color(g.GetComponent<SpriteRenderer>().color.r, g.GetComponent<SpriteRenderer>().color.g, g.GetComponent<SpriteRenderer>().color.b, 0f);
        }
    }
    #region 各种操作
    public void Fade(string role,string vpName, float fadeTime, float target)
    {
        var roleVp = _vps[role];
        roleVp.VpNow = vpName;
        roleVp.VpsDictionary[vpName].GetComponent<SpriteRenderer>().DOFade(target, fadeTime);
        _vps[role] =  roleVp;
    }

    public void Move(string role, float moveTime, Vector3 target)
    {
        if (moveTime == 0)
        {
            _vps[role].PosGameObject.transform.position = target;
            return;
        }
        _vps[role].PosGameObject.transform.DOLocalMove(target, moveTime);
    }

    public void Scale(string obj, float scaleTime, Vector3 vector3)
    {
        _vps[obj].PosGameObject.transform.DOScale(vector3, scaleTime); 
    }

    public void Move2O(string obj)
    {
        switch (obj)
        {
            case "elpis":
                BlinkForSeconds(_vps["elpis"].VpsDictionary["elpis"].GetComponent<SpriteRenderer>(),2,0.7f,new Vector3(0f,0.27f,0));
                break;
        }
    }
#endregion
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
            //到海边
            _vps["elpis"].PosGameObject.gameObject.transform.localPosition = target;
            _vps["elpis"].PosGameObject.transform.localScale = new Vector3(1f, 1f, 1);
            //播放转场动画G=
            DelayGameObject.transform.DOMove(new(0, 0, 0), 0.1f).OnComplete(() =>
            {
                sr.DOFade(0, 4);
                PygmalionGameManager.Instance.ReadLine();
            });
        });
    }
    public void SaveAllVp(int type)
    {
        
            List<Vp> list = new();
            foreach (var e in _vps["elpis"].VpsDictionary.Values)
            {
                list.Add(new Vp
                {
                    role = "elpis",
                    name = e.name,
                    color = e.GetComponent<SpriteRenderer>().color
                });
            }
            foreach (var e in _vps["e"].VpsDictionary.Values)
            {
                list.Add(new Vp
                {
                    role = "e",
                    name = e.name,
                    color = e.GetComponent<SpriteRenderer>().color
                });
            }
            foreach (var e in _vps["ym"].VpsDictionary.Values)
            {
                list.Add(new Vp
                {
                    role = "ym",
                    name = e.name,
                    color = e.GetComponent<SpriteRenderer>().color,
                });
            }

            SaveVPData data = new() { snapshots = list };
            data.elpispose = _vps["elpis"].PosGameObject.transform.position;
            data.epose =  _vps["e"].PosGameObject.transform.position;
            data.ympose =  _vps["ym"].PosGameObject.transform.position;
            data.elpisScale = _vps["elpis"].PosGameObject.transform.localScale;
            data.eScale  =  _vps["e"].PosGameObject.transform.localScale;
            data.ymScale  =  _vps["ym"].PosGameObject.transform.localScale;
            data.ymnow =   _vps["ym"].VpNow;
            data.enow =   _vps["e"].VpNow;
            data.elpisnow =   _vps["elpis"].VpNow;
            string json = JsonUtility.ToJson(data, true);
        
            File.WriteAllText(
                type == 0
                    ? Path.Combine(Application.persistentDataPath, AutoSaveFile)
                    : Path.Combine(Application.persistentDataPath, PersonSaveFile), json);
    }

    // 从磁盘读出并覆盖当前 vp 的状态
    public void ContinueGame(int type)
    {
        string path = Path.Combine(Application.persistentDataPath, type == 0 ? AutoSaveFile : PersonSaveFile);
        if (!File.Exists(path))
        {
            Debug.Log("[VpManager] 没有找到存档文件，跳过读档");
            return;
        }
        else
        {
            Debug.Log("[VpManager] 读档");
        }
        string json = File.ReadAllText(path);
        SaveVPData data = JsonUtility.FromJson<SaveVPData>(json);
        var vp = _vps["elpis"];
        vp.VpNow = data.elpisnow;
        vp.PosGameObject.transform.position = data.elpispose;
        vp.PosGameObject.transform.localScale = data.elpisScale;
        vp = _vps["e"];
        vp.VpNow = data.enow;
        vp.PosGameObject.transform.position = data.epose;
        vp.PosGameObject.transform.localScale = data.eScale;
        _vps["e"] = vp;
        vp = _vps["ym"];
        vp.PosGameObject.transform.position = data.ympose;
        vp.PosGameObject.transform.localScale = data.ymScale;
        vp.VpNow = data.ymnow;
        _vps["ym"] = vp;
        foreach (var snap in data.snapshots)
        {
           _vps[snap.role].VpsDictionary[snap.name].GetComponent<SpriteRenderer>().color = snap.color;
        }
    }

    public void Change(string role, string vpName)
    {
        RoleVp roleVp = _vps[role];
        _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color = new Color(
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.r,
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.g,
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.b,0);
        roleVp.VpNow = vpName;
        _vps[role] = roleVp;
        _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color = new Color(
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.r,
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.g,
            _vps[role].VpsDictionary[roleVp.VpNow].GetComponent<SpriteRenderer>().color.b,1);
    }
    
}
