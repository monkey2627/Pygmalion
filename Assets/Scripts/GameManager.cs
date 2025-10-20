using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Globalization;
using Ani;
using DG.Tweening;
using GamePlay;
using Scene;
using UnityEngine.Serialization;

[RequireComponent(typeof(GameManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public VideoPlayer videoPlayer;
    public GameObject guideScene;
    public GameObject dialog;
    public TMPro.TMP_Text roleName;
    public bool isPlayingVideo = false;
    public GameObject delayObj;
    public GameObject wordCloneObj;
    public Scene.Scene ocean;
    public Scene.Scene start;
    public Dictionary<string, Scene.Scene> ScenesDic = new Dictionary<string, Scene.Scene>();
    public GameObject upperButtons;
    private  void Awake()
    {
        Instance = this;

        ScenesDic["ocean"] = ocean;
        ScenesDic["start"] = ocean;
        upperButtons.SetActive(false);
        dialog.SetActive(false);
    }

    private async void Start()
    {           
        await LoadScript("0");
                await LoadScript("pre");
                await LoadScript("ym");
                await LoadScript("ymEnd");
        //说明这是玩家第一次点开这个游戏,或者是序章都没过完，那就直接从播放动画开始
        if (DataManager.Instance.GameCircle == "0" || DataManager.Instance.GameCircle == "null")
        {
            print("start from pre");
            DataManager.Instance.ScriptNow= "pre";
            DataManager.Instance.LineNow = 0;
            DataManager.Instance.GameCircle = "true";
            PlayerPrefs.SetString("gameCircle", "0");
            PlayerPrefs.SetString("scriptNow","pre");
            PlayerPrefs.SetInt("lineNow", 0);
            ReadLine();
        }
        else
        {
            //直接显示开始界面
            ScenesDic["start"].gameObject.SetActive(true);
        }
    }

    public void StartNewGame()
    {
        DataManager.Instance.StartNewGame();
        ReadLine();
    }

    public void ContinueGame()
    {
        DataManager.Instance.ContinueGame();
    }
    void ShowGuide()
    {
        videoPlayer.Stop();
        guideScene.SetActive(true);
    }
    public async TaskString LoadScript(string storage)
    {
        await ResourceLoader.LoadText(storage);
        return null;
    }
    public void ReadLine()
    {
        StartCoroutine("Read");
    }
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator Read()
    {
        string l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
        Dictionary<string, string> parsedTag = Utils.ParseLine(l);
        Debug.Log(l);
        if (parsedTag == null) yield break;
        if (parsedTag.TryGetValue("delay", out var delayTimeStr))
        {
            int delayTime = int.Parse(delayTimeStr);
            float time = 0;
            while (time < delayTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }
        switch (parsedTag["tag"])
        {
            case "video":
                isPlayingVideo = true;
                Addressables.LoadAssetAsync<VideoClip>(parsedTag["name"]).Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        VideoClip videoClip = handle.Result;
                        videoPlayer.clip = videoClip;
                        videoPlayer.isLooping = false || parsedTag.ContainsKey("loop");
                        videoPlayer.loopPointReached += (source) =>
                        {
                            if (!videoPlayer.isLooping)
                            {
                                videoPlayer.GetComponent<VideoPlayer>().clip = null;
                                videoPlayer.gameObject.SetActive(false);
                                isPlayingVideo = false;
                               
                                if (parsedTag["name"]=="002")
                                {
                                    DataManager.Instance.GameCircle = "1";
                                    return;
                                } 
                                ReadLine();
                            }
                        };
                        videoPlayer.gameObject.SetActive(true);
                        videoPlayer.Play();
                    }
                    else
                    {
                        Debug.LogError("Failed to load video: " + parsedTag["name"]);
                    }
                };
                break;
            case "role":
                roleName.text = parsedTag["name"];
                dialog.SetActive(true);
                l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                TextLoader.Instance.Push(l);
                break;
            case "operation":
                GameObject obj = Utils.FindChildInTransform(GameObject.Find(parsedTag["parent"]).transform, parsedTag["obj"]).gameObject;
                if (parsedTag.TryGetValue("setActive", out var value))
                {
                    obj.SetActive(value == "true");
                }
                else if (parsedTag.TryGetValue("enable", out var value1))
                {
                    Type scriptType = Type.GetType(parsedTag["script"]);
                    if (scriptType != null)
                    {
                        MonoBehaviour scriptComponent = (MonoBehaviour)obj.GetComponent(scriptType);
                        scriptComponent.enabled = value1 == "true";
                    }
                }
                else if (parsedTag.ContainsKey("att"))
                {
                    Type scriptType = Type.GetType(parsedTag["script"]);
                    if (scriptType != null)
                    {
                        MonoBehaviour scriptComponent = (MonoBehaviour)obj.GetComponent(scriptType);
                        switch (parsedTag["content"])
                        {
                            case "true":
                                Utils.ModifyField(scriptComponent, parsedTag["att"], true);
                                break;
                            case "false":
                                Utils.ModifyField(scriptComponent, parsedTag["att"], false);
                                break;
                        }
                    }
                }
                else if (parsedTag.ContainsKey("method"))
                {
                    Type scriptType = Type.GetType(parsedTag["script"]);
                    if (scriptType != null)
                    {
                        MonoBehaviour scriptComponent = (MonoBehaviour)obj.GetComponent(scriptType);
                        Utils.InvokeMethod(scriptComponent, parsedTag["method"]);
                    }
                }

                break;
            case "vp":
                float fadeTime=0;
                if(parsedTag.ContainsKey("time"))
                    fadeTime = float.Parse(parsedTag["time"]);
                float x, y, z;
                switch (parsedTag["op"])
                { 
                    case "move2":
                        x = float.Parse(parsedTag["x"], CultureInfo.InvariantCulture);
                        y = float.Parse(parsedTag["y"], CultureInfo.InvariantCulture);
                        z = float.Parse(parsedTag["z"], CultureInfo.InvariantCulture);
                        VpManager.Instance.Move(parsedTag["name"], fadeTime,new Vector3(x, y, z));
                        break;
                    case "fade":
                        float target =  float.Parse(parsedTag["fade"]);
                        print("dsdasd");
                        VpManager.Instance.Fade(parsedTag["name"],fadeTime,target);
                        break;
                    case "scale":      
                        x = float.Parse(parsedTag["x"], CultureInfo.InvariantCulture);
                        y = float.Parse(parsedTag["y"], CultureInfo.InvariantCulture);
                        z = float.Parse(parsedTag["z"], CultureInfo.InvariantCulture);
                        VpManager.Instance.Scale(parsedTag["name"], fadeTime,new Vector3(x, y, z));
                        break;
                    case "move2o":
                        VpManager.Instance.Move2O(parsedTag["name"]);
                        break;
                }
                break;
            case "sentence":
                switch (parsedTag["op"])
                {
                    case "create":
                        //每次遇到创建的时候就清空，然后将所有的句子都注册
                        SentenceManager.Instance.sentences = new List<Sentence>();
                        CreateSentence(0);
                        break;
                    case "enable":
                        int wordNum = int.Parse(parsedTag["word"]);
                        int sentenceNum = int.Parse(parsedTag["sentence"]);
                        SentenceManager.Instance.sentences[sentenceNum].words[wordNum].enable = parsedTag["enable"]=="1";
                        break;
                    case "fade":
                        SentenceManager.Instance.Fade();
                        break;
                }
                break;
            case "ani":
                switch (parsedTag["name"])
                {
                    case "water":
                        waterAni.Instance.target = parsedTag["role"];
                        waterAni.Instance.gameObject.SetActive(true);
                        break;
                    case "back2ocean":
                        Back2oceanAni.Instance.gameObject.SetActive(true);
                        break;
                }
                break;
            case "scene":
                switch (parsedTag["load"])
                {
                    case "1":
                        ScenesDic[parsedTag["name"]].Load();
                        break;
                    case "0":
                        ScenesDic[parsedTag["name"]].Unload();
                        break;
                }
                break;
            case "image":
                GameObject img = Utils.FindChildInTransform(GameObject.Find(parsedTag["parent"]).transform, parsedTag["obj"]).gameObject;
                switch (parsedTag["op"])
                {
                    case "fade":
                        float time = float.Parse(parsedTag["time"]);
                        float target = float.Parse(parsedTag["target"]);
                        img.GetComponent<SpriteRenderer>().DOFade(target, time);
                        break;
                }

                break;
            default:
            break;
}

if (parsedTag.ContainsKey("move"))
{
int delay = int.Parse(parsedTag["move"]);
if (delay > 0)
   delayObj.transform.DOMove(new Vector3(1000, 1000), delay).OnComplete(ReadLine);
else
   ReadLine();
}
}
public GameObject CloneWord(GameObject original,GameObject cloneSentence)
{
    // 克隆并指定父节点
    GameObject clone = Instantiate(original,cloneSentence.transform);
    clone.SetActive(true);
    return clone;
}

public GameObject sentenceCloneObj;
//递归生成句子
public void CreateSentence(int sentenceNumber)
{
    GameObject cloneSentence = Instantiate(sentenceCloneObj,SentenceManager.Instance.transform);
    RectTransform cloneRt = cloneSentence.transform as RectTransform;
    if (cloneRt != null)
    {
        cloneRt.anchoredPosition3D = (sentenceCloneObj.transform as RectTransform).anchoredPosition3D;
        cloneRt.sizeDelta = Vector2.zero;
    }
    cloneSentence.SetActive(true);
    cloneSentence.GetComponent<Sentence>().number = sentenceNumber;
    cloneSentence.GetComponent<Sentence>().SentenceEnds = new List<SentenceEnd>();
    string l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
    Dictionary<string, string> parsedTag = Utils.ParseLine(l);
    while (!string.Equals(parsedTag["tag"], "end", StringComparison.Ordinal))
    {
        if (parsedTag["tag"] == "sentence")
        {
          CreateSentence(sentenceNumber+1);
        }
        else if(parsedTag["tag"]=="string")
        {
             string content = parsedTag["content"];
             int jum2 = int.Parse(parsedTag["jump2"]);
             cloneSentence.GetComponent<Sentence>().SentenceEnds.Add(new SentenceEnd(){Content = content,Jump2 = jum2,Enable = true});
        }
        else
        {
            GameObject word = CloneWord(wordCloneObj,cloneSentence);
            cloneSentence.GetComponent<FlowLayoutGroupCentered>().Refresh();
            word.GetComponent<Word>().enable = parsedTag["enable"]=="1";
            word.GetComponent<Word>().sentence = cloneSentence.GetComponent<Sentence>();
            switch (parsedTag["type"])
            {
                  case "0":
                      word.GetComponent<Word>().wordType = 0;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.white;
                      word.GetComponent<Word>().sentenceNumber = sentenceNumber;
                      break;
                  case "1":
                      word.GetComponent<Word>().wordType = 1;
                      word.GetComponent<Word>().wordText.text = "<color=#00000000>空</color>";
                      word.GetComponent<Word>().wordText.color = Color.white;
                      word.GetComponent<Word>().addText =  parsedTag["content"];
                      word.GetComponent<Word>().sentenceNumber = sentenceNumber;
                      break;
                  case "2":
                      word.GetComponent<Word>().wordType = 2;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.yellow;
                      word.GetComponent<Word>().sentenceNumber = sentenceNumber;
                      word.GetComponent<Word>().changeWordList = new List<string>();
                      word.GetComponent<Word>().changeWordList.Add(parsedTag["content"]);
                      for (int i = 0; i < int.Parse(parsedTag["changeNumber"]); i++)
                      {
                          l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                          word.GetComponent<Word>().changeWordList.Add(l);
                      }
                      break;
                  case "3":
                      word.GetComponent<Word>().wordType = 3;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.red;
                      word.GetComponent<Word>().sentenceNumber = sentenceNumber;
                      word.GetComponent<Word>().nextSentenceNumber = sentenceNumber + 1;
                      break;
            }
            word.GetComponent<AutoBox>().RefreshBox2d();
            cloneSentence.GetComponent<Sentence>().words.Add(word.GetComponent<Word>());
        }
        l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
        parsedTag = Utils.ParseLine(l);
    }
    SentenceManager.Instance.sentences.Add(cloneSentence.GetComponent<Sentence>());
}

}

