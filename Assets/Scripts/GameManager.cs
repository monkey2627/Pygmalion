using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Globalization;
using DG.Tweening;
using GamePlay;
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
    public SceneManager ocean;
    public Dictionary<string, SceneManager> ScenesDic = new Dictionary<string, SceneManager>();
    public GameObject upperButtons;
    private async void Awake()
    {
        Instance = this;
        await LoadScript("0");
        ScenesDic["ocean"] = ocean;
        string firstOpenGame = PlayerPrefs.GetString("FirstOpenGame","null");
        if (firstOpenGame == "null")
        {
            //说明这是玩家第一次点开这个游戏,那就直接从播放动画开始
            DataManager.Instance.ScriptNow= "0";
            DataManager.Instance.LineNow = 0;
            DataManager.Instance.GameCircle = "true";
            PlayerPrefs.SetString("gameCircle", "0");
            PlayerPrefs.SetString("scriptNow","0");
            PlayerPrefs.SetInt("lineNow", 0);
            GameManager.Instance.ReadLine();
        }
        upperButtons.SetActive(false);
        dialog.SetActive(false);
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
        if (parsedTag != null)
        {
            Debug.Log("Parsed Tag:");
            foreach (var kvp in parsedTag)
            {
                Debug.Log($"  {kvp.Key} = {kvp.Value}");
            }
        }
        else
        {
            Debug.Log("Non-tag line: " + l);
        }
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
                string name = parsedTag["name"];
                roleName.text = name;
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
                if (parsedTag.ContainsKey("fade"))
                {
                    float fadeTime = float.Parse(parsedTag["time"]);
                    float target =  float.Parse(parsedTag["fade"]);
                    VpManager.Instance.Fade(parsedTag["name"],fadeTime,target);
                }else if (parsedTag.ContainsKey("move"))
                {
                    float fadeTime = float.Parse(parsedTag["time"]);
                    float x = float.Parse(parsedTag["x"], CultureInfo.InvariantCulture);
                    float y = float.Parse(parsedTag["y"], CultureInfo.InvariantCulture);
                    float z = float.Parse(parsedTag["z"], CultureInfo.InvariantCulture);
                    VpManager.Instance.Move(parsedTag["name"], fadeTime,new Vector3(x, y, z));
                }
                break;
            case "sentence":
                switch (parsedTag["op"])
                {
                    case "create":
                         CreateSentence(0);
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

public GameObject sentenceManagerObj;
public GameObject sentenceCloneObj;
//递归生成句子
public void CreateSentence(int sentenceNumber)
{
    GameObject cloneSentence = Instantiate(sentenceCloneObj,sentenceManagerObj.transform);
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
                      word.GetComponent<Word>().wordText.color = Color.white;
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
            cloneSentence.GetComponent<Sentence>().words.Add(word.GetComponent<Word>());
        }
        l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
        parsedTag = Utils.ParseLine(l);
    }
    if (sentenceNumber == 0)
        sentenceManagerObj.GetComponent<SentenceManager>().sentences = new List<Sentence>();
    sentenceManagerObj.GetComponent<SentenceManager>().sentences.Add(cloneSentence.GetComponent<Sentence>());
}

}

