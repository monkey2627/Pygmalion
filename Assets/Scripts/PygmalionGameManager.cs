using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
using System.Globalization;
using Ani;
using DG.Tweening;
using GamePlay;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.IO;
using Scene;
using TMPro;
using UnityEngine.Serialization;


public class PygmalionGameManager : MonoBehaviour
{
    public static PygmalionGameManager Instance;
    public VideoPlayer videoPlayer;
    public GameObject guideScene;
    public GameObject dialog;
    public TMP_Text roleName;
    public Sprite[] roleSprites;
    private Dictionary<string, Sprite> _roleSpriteDict = new Dictionary<string, Sprite>();
    public GameObject roleHeadGameObject;
    public bool isPlayingVideo = false;
    public GameObject delayObj;
    public GameObject wordCloneObj;
    public Scene.Scene ocean;
    public Scene.Scene start;
    public Scene.Scene lab;
    public Dictionary<string, Scene.Scene> ScenesDic = new Dictionary<string, Scene.Scene>();
    public GameObject upperButtons;
    public bool isGameTime = false;
    public GameObject frameNow;
    public Sprite[] sprites;
    private Dictionary<string, Sprite> _frameDic = new Dictionary<string,  Sprite>();
    //ending
    public TMP_Text endText;
    //alert
    public AlertScroll alert;
    private  void Awake()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            _frameDic.Add(sprites[i].name, sprites[i]);
        }
        Instance = this;
        for (int i = 0; i < roleSprites.Length; i++)
        {
            _roleSpriteDict.Add(roleSprites[i].name, roleSprites[i]);
        }
        ScenesDic["ocean"] = ocean;
        ScenesDic["lab"] = lab;
        ScenesDic["start"] = start;
        upperButtons.SetActive(false);
        dialog.SetActive(false);
    }

    private async void Start()
    {           
        await LoadScript("0");
        await LoadScript("pre");
        await LoadScript("ym");
        await LoadScript("ymEnd1");
        await LoadScript("ymEnd2");
        await LoadScript("ymEnd3");
        await LoadScript("ymsupport");
        await LoadScript("e");
        await LoadScript("eEnd1");
        await LoadScript("eEnd2");
        await LoadScript("eSupport");
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

    public GameObject black;
    public void StartNewGame()
    {
        DataManager.Instance.StartNewGame();
        VpManager.Instance.StartNewGame();
        dialog.SetActive(false);
        black.GetComponent<SpriteRenderer>().DOFade(1, 0);
        isGameTime = false;
        ocean.gameObject.SetActive(false);
        lab.gameObject.SetActive(false);
        PSceneManager.Instance._currentScene = null;
        videoPlayer.gameObject.SetActive(false);
        TestChange2Ym();        
        upperButtons.SetActive(true);
        ReadLine();
    }

    public void TestChange2Ym()
    {
        DataManager.Instance.LineNow = 192;
        DataManager.Instance.ScriptNow = "e";
    }

    [Serializable]
    public struct GameManagerSaveStruct
    {
        //为了持久化
        public string objName;       
        public Vector3 pos;
        public Vector3 scale;
        public int isActivate;
        public Color color;
    }
    [Serializable]
    public class SaveMyStructData
    {
        public bool isGameTime;//是否正在改句游戏中
        public bool dialogActive;//对话框是否activate
        public string frameNow;//现在的对话框样式
        public bool isVideoPlayer;//videoPlayer是否active
        public bool isUpperButtons;//控制键active
        public Color blackColor;
        public string currentScene;
        public List<GameManagerSaveStruct> snapshots;
    }
    private static readonly string SaveFile = "SaveGameManagerData.json";
    //存一些其他的状态
    public void Save(int type = 0)
    {
        if (type == 0)
        {   
            SaveMyStructData data = new SaveMyStructData();
            data.currentScene = PSceneManager.Instance._currentScene.name;
            data.isGameTime = isGameTime;
            data.dialogActive = dialog.activeInHierarchy;
            data.frameNow = frameNow.name;
            data.isUpperButtons = upperButtons.activeInHierarchy;
            data.isVideoPlayer = videoPlayer.gameObject.activeInHierarchy;
            data.blackColor =  black.gameObject.GetComponent<SpriteRenderer>().color;
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFile), json);
            Debug.Log($"[PygmalionGameManager] 已保存");
        }
    }

    public void Load()
    {

        if (isGameTime)
        {
            //加载背后的水母和人
            WaterAni.Instance.LoadDataAndShowScene();
            //加载词语
        }
        else
        {
            
            
        }
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
    IEnumerator Read()
    {
        String scriptname = DataManager.Instance.ScriptNow; 
        string l = ResourceLoader.textLoader[scriptname].Lines[DataManager.Instance.LineNow++];
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
                        videoPlayer.isLooping = parsedTag.ContainsKey("loop");
                        videoPlayer.loopPointReached += (source) =>
                        {
                            if (!videoPlayer.isLooping)
                            {
                                videoPlayer.GetComponent<VideoPlayer>().clip = null;
                                videoPlayer.gameObject.SetActive(false);
                                isPlayingVideo = false;
                               
                                if (parsedTag["name"]=="002")
                                {
                                    
                                    PlayerPrefs.SetString("gameCircle", "1");
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
                if (parsedTag.ContainsKey("frame"))
                {
                    frameNow.GetComponent<SpriteRenderer>().sprite = _frameDic[parsedTag["frame"]];
                }

                if (parsedTag.ContainsKey("sprite"))
                {
                    if (parsedTag["sprite"] == "none")
                    {
                        roleHeadGameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                    else
                    {
                        if(_roleSpriteDict.ContainsKey(parsedTag["sprite"]))
                            roleHeadGameObject.GetComponent<SpriteRenderer>().sprite = _roleSpriteDict[parsedTag["sprite"]];
                        else
                            roleHeadGameObject.GetComponent<SpriteRenderer>().sprite = null;
                    }
                }
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
            case "ending":
                endText.gameObject.SetActive(true);
                endText.text = parsedTag["content"];
                endText.DOFade(1, 0.5f);
                delayObj.transform.DOMove(new Vector3(0, 1, 0), 2f).OnComplete(() => { 
                    endText.DOFade(0, 1).OnComplete(() =>
                {
                    endText.gameObject.SetActive(false);
                    ReadLine();
                });});
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
            case "alert":
                alert.text = parsedTag["content"];
                alert.gameObject.SetActive(true);
                break;
            case "sentence":
                switch (parsedTag["op"])
                {
                    case "create":
                        //每次遇到创建的时候就清空，然后将所有的句子都注册
                        isGameTime=true;
                        SentenceManager.instance.sentenceBeginPlace = DataManager.Instance.LineNow-1;
                        SentenceManager.instance.paragraphs = new List<Paragraph>();
                        SentenceManager.instance.guideTime = false;
                        CreateSentence(-1);
                        break;
                    case "enable":
                        int wordNum = int.Parse(parsedTag["word"]);
                        int sentenceNum = int.Parse(parsedTag["sentence"]);
                        int page = int.Parse(parsedTag["page"]);
                        SentenceManager.instance.paragraphs[sentenceNum].pages[page].words[wordNum].enable = parsedTag["enable"]=="1";
                        break;
                    case "fade":
                        SentenceManager.instance.Fade();
                        break;
                    case "enableEvery":
                        //将所有都enable
                        SentenceManager.instance.EnableEveryWord();
                        break;
                    case "disableEvery":
                        //将所有都enable
                        SentenceManager.instance.DisableEveryWord();
                        break;
                    case "changeColor":
                        wordNum = int.Parse(parsedTag["word"]);
                        sentenceNum = int.Parse(parsedTag["sentence"]);
                        page = int.Parse(parsedTag["page"]);
                        if(parsedTag["color"]=="yellow")
                            SentenceManager.instance.paragraphs[sentenceNum].pages[page].words[wordNum].wordText.color = Color.yellow;
                        break;
                    case "changeType":
                        wordNum = int.Parse(parsedTag["word"]);
                        sentenceNum = int.Parse(parsedTag["sentence"]);
                        page = int.Parse(parsedTag["page"]);
                        SentenceManager.instance.paragraphs[sentenceNum].pages[page].words[wordNum].wordType = int.Parse(parsedTag["type"]);

                        break;
                }
                break;
            case "ani":
                switch (parsedTag["name"])
                {
                    case "water":
                        WaterAni.Instance.target = parsedTag["role"];
                        WaterAni.Instance.gameObject.SetActive(true);
                        break;
                    case "back2ocean":
                        Back2OceanAni.Instance.gameObject.SetActive(true);
                        break;
                    case "ymani1":
                        break;
                }
                break;
            case "script"://换脚本
                string scriptName = parsedTag["name"];
                DataManager.Instance.ScriptNow = scriptName;
                DataManager.Instance.LineNow = int.Parse(parsedTag["line"]);
                ReadLine();
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
                        print("fade++");
                        print(img.transform.parent.name);
                        float time = float.Parse(parsedTag["time"]);
                        float target = float.Parse(parsedTag["target"]);
                        print(target);
                        print(time);
                        print(img.gameObject.name);
                        img.GetComponent<SpriteRenderer>().DOFade(0, 0f);
                          //  .OnComplete(() => Debug.Log("after: " + img.GetComponent<SpriteRenderer>().color.a));
                        break;
                }

                break;
}

if (parsedTag.ContainsKey("move"))
{
float delay = float.Parse(parsedTag["move"]);
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

[FormerlySerializedAs("sentenceCloneObj")] public GameObject paragraphCloneObj;

// ReSharper disable Unity.PerformanceAnalysis
private void CreateSentence(int fatherSentenceNumber)
{
    GameObject cloneParagraph = Instantiate(paragraphCloneObj,SentenceManager.instance.transform);
    cloneParagraph.GetComponent<Paragraph>().fatherSentenceNumber = fatherSentenceNumber;

    cloneParagraph.GetComponent<Paragraph>().pages = new List<Page>();
    cloneParagraph.GetComponent<Paragraph>().sentenceNumber = SentenceManager.instance.paragraphs.Count;
    SentenceManager.instance.paragraphs.Add(cloneParagraph.GetComponent<Paragraph>());
    string l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
    print(l);
    Dictionary<string, string> parsedTag = Utils.ParseLine(l);
    while (!string.Equals(parsedTag["tag"], "end", StringComparison.Ordinal))
    {
        if(parsedTag["tag"]=="sentenceEnd")
        {
            switch (parsedTag["type"])
            {
                case "0":
                    SentenceManager.instance.type = ConfirmType.Normal;
                    for (int i = 0; i < 3; i++)
                    {
                        SentenceManager.instance.endScriptsList.Add(ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++].TrimStart().Trim());
                    }
                    break;
                case "1":
                    SentenceManager.instance.type = ConfirmType.OnlyOneCorrect;
                    for (int i = 0; i < 2; i++)
                    {
                        SentenceManager.instance.endScriptsList.Add(ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++].TrimStart().Trim());
                    }
                    break; 
                   
            } 
            l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
            print(l);//把endend读掉
        }
        else if (parsedTag["tag"] == "page")
        {
            GameObject page = Instantiate(cloneParagraph.GetComponent<Paragraph>().PageCloneGameObject,cloneParagraph.GetComponent<Paragraph>().pagesFather.transform);
            cloneParagraph.GetComponent<Paragraph>().pages.Add(page.GetComponent<Page>());
            l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
            print(DataManager.Instance.LineNow+" l "+l);
            parsedTag = Utils.ParseLine(l);
            Page pageScript =page.GetComponent<Page>();  
            pageScript.paragraph = cloneParagraph.GetComponent<Paragraph>(); 
            while (!string.Equals(parsedTag["tag"], "pageEnd", StringComparison.Ordinal))
            {
                GameObject word = CloneWord(wordCloneObj,pageScript.layout.gameObject);
                switch (parsedTag["type"])
                {
                  case "0":
                      word.GetComponent<Word>().wordType = 0;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      if (parsedTag.ContainsKey("special"))
                          word.GetComponent<Word>().special = true;
                      word.GetComponent<Word>().wordText.color = Color.white;
                      print( parsedTag["content"]);
                      break;
                  case "1":
                      word.GetComponent<Word>().wordType = 1;
                      word.GetComponent<Word>().spaceYellow.SetActive(true);
                      word.GetComponent<Word>().wordText.text = "<color=#00000000>空</color>";
                      word.GetComponent<Word>().wordText.color = Color.yellow;;
                      word.GetComponent<Word>().addText =  parsedTag["content"];
                      word.GetComponent<Word>().changeWordList = new List<string>();
                      word.GetComponent<Word>().changeWordList.Add("<color=#00000000>空</color>");
                      switch (parsedTag["right"])
                      {
                          case "0"://add
                              word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                              break;
                          case "1"://none
                              word.GetComponent<Word>().answerList.Add("<color=#00000000>空</color>");
                              break;
                          case "2"://none and add
                              word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                              word.GetComponent<Word>().answerList.Add("<color=#00000000>空</color>");
                              break;
                      }
                      word.GetComponent<Word>().doubleClick1Board.GetComponent<DoubleClick1Board>().Gen(word.GetComponent<Word>().changeWordList);
                      break;
                  case "2":
                      word.GetComponent<Word>().wordType = 2;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.yellow;
                      word.GetComponent<Word>().changeWordList = new List<string>();
                      word.GetComponent<Word>().changeWordList.Add(parsedTag["content"]);
                      for (int i = 0; i < int.Parse(parsedTag["changeNumber"]); i++)
                      {
                          l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                          l = l.TrimStart();
                          word.GetComponent<Word>().changeWordList.Add(l);
                          Debug.Log(l);
                      }
                      if (parsedTag.ContainsKey("changeDialog"))
                      {
                          word.GetComponent<Word>().changeDialog = true;
                          word.GetComponent<Word>().scriptLine = int.Parse(parsedTag["scriptLine"]); ;
                          word.GetComponent<Word>().scriptName = parsedTag["scriptName"];
                      }
                      if(parsedTag.ContainsKey("right"))
                          switch (parsedTag["right"])
                          {
                              case "0"://change
                                  for (int i = 1; i < word.GetComponent<Word>().changeWordList.Count; i++)
                                  {
                                      word.GetComponent<Word>().answerList.Add(word.GetComponent<Word>().changeWordList[i]);
                                  }
                                  break;
                              case "1"://delete
                                  word.GetComponent<Word>().answerList.Add("/");
                                  break;
                              case "2"://change and delete
                                  word.GetComponent<Word>().answerList.Add("/");
                                  for (int i = 1; i < word.GetComponent<Word>().changeWordList.Count; i++)
                                  {
                                      word.GetComponent<Word>().answerList.Add(word.GetComponent<Word>().changeWordList[i]);
                                  }
                                  break;
                              case "3"://none
                                  word.GetComponent<Word>().answerList.Add(parsedTag["content"]);
                                  break;
                          }
                      word.GetComponent<Word>().doubleClick2Board.GetComponent<DoubleClick2Board>().Gen(word.GetComponent<Word>().changeWordList);
                      break;
                  case "3":
                      word.GetComponent<Word>().wordType = 3;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.red;
                      word.GetComponent<Word>().nextParagraphNumber = SentenceManager.instance.paragraphs.Count;
                      l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                      if (parsedTag.ContainsKey("script"))
                      {
                          word.GetComponent<Word>().hasSpecial = true;
                      }
                      if(parsedTag.ContainsKey("special"))
                            word.GetComponent<Word>().special = true;
                      print("create scentence "+ SentenceManager.instance.paragraphs.Count);
                      CreateSentence(cloneParagraph.GetComponent<Paragraph>().sentenceNumber);
                      break;
                  case "4":
                      word.GetComponent<Word>().wordType = 4;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.yellow;
                      break;
                  case "5":
                      word.GetComponent<Word>().wordType = 5;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.yellow;
                      break;
                  case "6":
                      word.GetComponent<Word>().wordType = 6;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      word.GetComponent<Word>().wordText.color = Color.red;
                      if (parsedTag.ContainsKey("end"))
                          word.GetComponent<Word>().endText = parsedTag["end"];
                      else
                          word.GetComponent<Word>().endText = "";
                      word.GetComponent<Word>().pic = parsedTag["pic"];
                      l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                      parsedTag = Utils.ParseLine(l);
                      print("create word6: "+l);
                      word.GetComponent<Word>().dialogList = new List<Word.Dialog>();
                      while(parsedTag["tag"] != "dialogEnd")
                      {
                          word.GetComponent<Word>().dialogList.Add(new Word.Dialog(){Name = parsedTag["role"],Text = parsedTag["content"]});
                          l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                          print("create word6: "+l);
                          parsedTag = Utils.ParseLine(l);
                      }
                      break;
            }
                word.GetComponent<AutoBox>().RefreshBox2d();
                word.SetActive(true);                
                pageScript.words.Add(word.GetComponent<Word>());
                word.GetComponent<Word>().page = pageScript;
                l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                parsedTag = Utils.ParseLine(l);
            }
            pageScript.layout.Refresh();
            pageScript.gameObject.SetActive(pageScript.paragraph.pages.Count==1);
        }
        l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
        parsedTag = Utils.ParseLine(l);    
        print(l);
        if(parsedTag.ContainsKey("move"))
        {print("move         sda"); ReadLine();}
    
    }
    cloneParagraph.GetComponent<Paragraph>().Refresh();    
    cloneParagraph.SetActive(fatherSentenceNumber == -1);    
    cloneParagraph.GetComponent<Paragraph>().Appear();
}
    public void Change2ScriptAndReadLine(string endScripts,int line=0)
    {
        ResourceLoader.textLoader[DataManager.Instance.ScriptNow].SavedLine=DataManager.Instance.LineNow;
        DataManager.Instance.LineNow = line;
        DataManager.Instance.ScriptNow = endScripts;
        ReadLine();
    }
}

