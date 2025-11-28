using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
using System.Globalization;
using Ani;
using DG.Tweening;
using GamePlay;
using System.IO;
using Scene;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Page = GamePlay.Page;


public class PygmalionGameManager : MonoBehaviour
{
    public static PygmalionGameManager Instance;
    public GameObject dialog;
    public TMP_Text roleName;
    public Sprite[] roleSprites;
    private Dictionary<string, Sprite> _roleSpriteDict = new Dictionary<string, Sprite>();
    public Image roleHead;
    public GameObject delayObj;
    public GameObject wordCloneObj;
    public Scene.Scene ocean;
    public Scene.Scene start;
    public Scene.Scene lab;
    public Scene.Scene home;
    public Scene.Scene gallery;
    public Dictionary<string, Scene.Scene> ScenesDic = new Dictionary<string, Scene.Scene>();
    public GameObject upperButtons;
    public bool isGameTime = false;
    public Sprite[] sprites;
    private Dictionary<string, Sprite> _frameDic = new Dictionary<string,  Sprite>();
    //ending
    public TMP_Text endText;
    //alert
    public AlertScroll alert;
    public GameObject kiss;
    public GameObject eCircle;
    public GameObject elpisCircle;
    public GameObject ymCircle;
    private  void Awake()
    {
        Instance = this;
        _frameDic =  new Dictionary<string, Sprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            _frameDic.Add(sprites[i].name, sprites[i]);
        }
        
        _roleSpriteDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < roleSprites.Length; i++)
        {
            _roleSpriteDict.Add(roleSprites[i].name, roleSprites[i]);
        }
        ScenesDic["ocean"] = ocean;
        ScenesDic["lab"] = lab;
        ScenesDic["start"] = start;
        ScenesDic["home"] = home;
        ScenesDic["gallery"] = gallery;
        upperButtons.SetActive(false);
    }

    public void StopGame()
    {
        stopPanel.SetActive(true);
        upperButtons.SetActive(false);
        UIVoice.instance.OpenMenu();
    }

    private async void Start()
    {           
        await LoadScript("0");
        await LoadScript("ym");
        await LoadScript("ymEnd1");
        await LoadScript("ymEnd2");
        await LoadScript("ymEnd3");
        await LoadScript("ymsupport");
        await LoadScript("e");
        await LoadScript("eEnd1");
        await LoadScript("eEnd2");
        await LoadScript("eSupport");
        DataManager.Instance.ScriptNow= "0";
        DataManager.Instance.LineNow = 0;
        PlayerPrefs.SetString("scriptNow","0");
        PlayerPrefs.SetInt("lineNow", 0);
        pre.SetActive(true);
        if (DataManager.Instance.GameCircle == "0" || DataManager.Instance.GameCircle == "null")
        {
            jumpPre.SetActive(false);
        }
        else
        {
            jumpPre.SetActive(true);
        }
    }

    public GameObject jumpPre;
    public GameObject pre;
    public GameObject black;

    public void ClearPre()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, AutoSaveFile));
        if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, PersonSaveFile));
    }
    public GameObject intersystem;
    public void StartNewGame()
    {
        //GameManager
        ClearPre();
        intersystem.GetComponent<SpriteRenderer>().DOFade(0, 0);
        dialog.SetActive(false);
        black.GetComponent<SpriteRenderer>().DOFade(1, 0);
        isGameTime = false;
        ocean.gameObject.SetActive(false);
        lab.gameObject.SetActive(false);
        home.gameObject.SetActive(false);
        upperButtons.SetActive(true); 
        kiss.SetActive(false);
        content.GetComponent<DialogClick>().enable = true;
        //所有的ani要false
        TransAniManager.Instance.eAni.SetActive(false);
        TransAniManager.Instance.elpisAni.SetActive(false);
        TransAniManager.Instance.ymAni.SetActive(false);
        TransAniManager.Instance.ecircle.SetActive(false);
        TransAniManager.Instance.elpiscircle.SetActive(false);
        TransAniManager.Instance.ymcircle.SetActive(false);
        //
        PSceneManager.Instance._currentScene = null;
        //DataManager
        DataManager.Instance.StartNewGame();
        //SentenceManager
        SentenceManager.instance.StartNewGame();
        //VPManager
        VpManager.instance.StartNewGame();
        //GuideSceneManager
        GuideSceneGamePlay.instance.StartNewGame();
        //transAniManager.
        TransAniManager.Instance.StartNewGame();
        ReadLine();
    }
    public void Back2Start()
    {
        //GameManager
        dialog.SetActive(false);
        black.GetComponent<SpriteRenderer>().DOFade(1, 0);
        upperButtons.SetActive(false);
        stopPanel.SetActive(false);
        ocean.gameObject.SetActive(false);
        lab.gameObject.SetActive(false);
        home.gameObject.SetActive(false);
        GuideSceneGamePlay.instance.gameObject.SetActive(false);
        kiss.SetActive(false);
        //所有的ani要false
        TransAniManager.Instance.eAni.SetActive(false);
        TransAniManager.Instance.elpisAni.SetActive(false);
        TransAniManager.Instance.ymAni.SetActive(false);
        TransAniManager.Instance.ecircle.SetActive(false);
        TransAniManager.Instance.elpiscircle.SetActive(false);
        TransAniManager.Instance.ymcircle.SetActive(false);
        //
        PSceneManager.Instance._currentScene = null;
        //
        VpManager.instance.back2start();
        //
        SentenceManager.instance.Destroyall();
        //
        BGM.instance.StopALL();
        //
        start.gameObject.SetActive(true);
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

    public GameObject exit;
    [Serializable]
    public class SaveMyStructData
    {
        public string gameRole;
        public bool wave;
        public bool highTide;
        public bool lowTide;
        public string text;
        public string name;
        public bool isKiss;
        public bool isExit;
        public Color exitColor;
        public bool isGameTime;//是否正在改句游戏中
        public bool dialogEnbale;
        public bool dialogActive;//对话框是否activate
        public string frameNow;//现在的对话框样式
        public bool isUpperButtons;//控制键active
        public Color blackColor;
        public Color InterColor;
        public string currentScene;
        public string spriteNow;
        public string bgm;//背景音乐的名字
        public bool isGuidePlayGame;
        public bool isTransAniManager;
        public List<GameManagerSaveStruct> snapshots;
    }

    public GameObject guidePlayGame;
    private static readonly string AutoSaveFile = "AutoSaveGameManagerData.json";
    private static readonly string PersonSaveFile = "PersonSaveGameManagerData.json";

    public GameObject content;
    //存一些其他的状态
    public void Save(int type = 0)
    {
         
            SaveMyStructData data = new SaveMyStructData();
            data.wave = wave.playing;
            data.highTide = highTide.playing;
            data.lowTide = lowTide.playing;
            data.text = content.GetComponent<TMP_Text>().text;
            data.exitColor = exit.GetComponent<SpriteRenderer>().color;
            data.isExit = exit.activeInHierarchy;
            data.name = roleName.text;
            data.InterColor = intersystem.GetComponent<SpriteRenderer>().color;
            data.isKiss = kiss.activeInHierarchy;
            data.isTransAniManager = TransAniManager.Instance.gameObject.activeInHierarchy;
            if (PSceneManager.Instance._currentScene)
                data.currentScene = PSceneManager.Instance._currentScene.name;
            else
                data.currentScene = "null";
            data.isGameTime = isGameTime;
            if (roleHead.sprite != null)
                data.spriteNow = roleHead.sprite.name;
            else
                data.spriteNow = "none";
            Debug.Log(data.spriteNow);
            data.dialogActive = dialog.activeInHierarchy;
            data.dialogEnbale =  content.GetComponent<DialogClick>().enable;
            data.frameNow = FrameImage.sprite.name;
            data.isUpperButtons = upperButtons.activeInHierarchy;
            data.blackColor =  black.gameObject.GetComponent<SpriteRenderer>().color;
            data.bgm = BGM.instance.bgmNow;
            data.isGuidePlayGame = guidePlayGame.activeInHierarchy;
            if (elpisCircle.activeInHierarchy)
                data.gameRole = "elpis";
            else if (eCircle.activeInHierarchy)
                    data.gameRole = "e";
            else if (ymCircle.activeInHierarchy)
                data.gameRole = "ym";
            string json = JsonUtility.ToJson(data, true);
            print(json);
            if (type == 0)
            {       
                File.WriteAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile), json);
            }
            else
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath,PersonSaveFile), json);

            }
    }

    public Image FrameImage;
    public GameObject stopPanel;
    public CrossFadeMusic highTide;
    public CrossFadeMusic lowTide;
    public CrossFadeMusic wave;
    /// <summary>
    /// 从存档点继续游戏
    /// </summary>
    public void ContinueGame()
    {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)) &&
            !File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
        {
            return;
        }
        stopPanel.SetActive(false);
        int i = DataManager.Instance.ContinueGame();
        string json;
        SaveMyStructData data;
        if (i == 0)
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile)); 
            data = JsonUtility.FromJson<SaveMyStructData>(json);
        }
        else
        {
            json = File.ReadAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile)); 
            data = JsonUtility.FromJson<SaveMyStructData>(json);
        }
        //载入现在的音乐
        BGM.instance.Play(data.bgm);
        if (data.highTide)
        {
            highTide.FadeInFromSecond(4.9f,1);
        }

        if (data.lowTide)
        {
            lowTide.FadeInFromZero(1);
        }

        if (data.wave)
        {
            wave.FadeInFromZero(1);
        }
        intersystem.GetComponent<SpriteRenderer>().color = data.InterColor;
        //载入现在的场景
        if(data.currentScene!="null"){
            PSceneManager.Instance._currentScene =  ScenesDic[data.currentScene];
            ScenesDic[data.currentScene].gameObject.SetActive(true);
        }
        else
        {
            ScenesDic["ocean"].gameObject.SetActive(false);
            ScenesDic["lab"].gameObject.SetActive(false);
            ScenesDic["home"].gameObject.SetActive(false);
        }
        //是否在文字游戏时间
        isGameTime=data.isGameTime;
        exit.SetActive(data.isExit);
        exit.GetComponent<SpriteRenderer>().color = data.exitColor;
        if (isGameTime)
        {
            SentenceManager.instance.ContinueGame(i);
            BGM.instance.Play("textGame");
        }
        else
        {
            SentenceManager.instance.Destroyall();
            BGM.instance.Stop("textGame");
        }
        //对话框是否active
        dialog.SetActive(data.dialogActive);
        //
        TransAniManager.Instance.gameObject.SetActive(data.isTransAniManager);
        //
        kiss.SetActive(data.isKiss);
        //现在的文本框样式
        FrameImage.sprite = _frameDic[data.frameNow];
        //头像
        Debug.Log(data.spriteNow);
        if (data.spriteNow != "none")
        {
            roleHead.sprite = _roleSpriteDict[data.spriteNow];
            roleHead.DOFade(1, 0);
        }
        else
        {
            roleHead.sprite = null;
        }
        data.frameNow = FrameImage.sprite.name;
        //内容
        content.GetComponent<TMP_Text>().text=data.text;
        content.GetComponent<DialogClick>().enable = data.dialogEnbale;
        roleName.text=data.name;
        upperButtons.SetActive(data.isUpperButtons);
        black.gameObject.GetComponent<SpriteRenderer>().color=data.blackColor;
        //
        if(data.gameRole=="elpis")
            elpisCircle.SetActive(true);
        else if(data.gameRole=="e")
            eCircle.SetActive(true);
        else if(data.gameRole=="ym")
            ymCircle.SetActive(true);
        //如果正在新手引导的阶段
        guidePlayGame.SetActive(data.isGuidePlayGame);
        print(data.isGuidePlayGame);
        if (data.isGuidePlayGame)
        {
            print("a");
            GuideSceneGamePlay.instance.ContinueGame(i);
            BGM.instance.Play("textGame");
        }
        //VPManager
        VpManager.instance.ContinueGame(i);
        //WaterAni
        if(data.isGameTime || data.isGuidePlayGame)
        {TransAniManager.Instance.ContinueGame(i);print("pppp");}
        String scriptname = DataManager.Instance.ScriptNow;
        print(scriptname);
        print(DataManager.Instance.LineNow);
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

    public TW_MultiStrings_Regular tmregular;
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
            case "bgm":
                if (parsedTag.ContainsKey("op"))
                {
                    if (parsedTag["op"] == "stop")
                    {
                        BGM.instance.FadeOutAndStop(0.5f);;
                    }

                    if (parsedTag["op"] == "fade")
                    {
                        BGM.instance.FadeOutAndStop(float.Parse(parsedTag["time"]));
                    }

                    if (parsedTag["op"] == "begin")
                    {
                        BGM.instance.Play(parsedTag["name"]);
                    }
                }
                else
                {
                    BGM.instance.Play(parsedTag["name"]);
                }
                break;
            case "env":
                if (parsedTag.ContainsKey("op"))
                {
                    if (parsedTag["op"] == "stop" || parsedTag["op"] == "fade" )
                    {
                        Environment.instance.Stop(parsedTag["name"],float.Parse(parsedTag["time"]));
                    }
                    if (parsedTag["op"] == "begin")
                    {
                        Environment.instance.Play(parsedTag["name"],float.Parse(parsedTag["time"]));
                    }

                    if (parsedTag["op"] == "beginOnce")
                    {
                        Environment.instance.PlayOnce(parsedTag["name"],float.Parse(parsedTag["time"]));
                    }
                    {
                        
                    }
                }
                else
                {
                    Environment.instance.Play(parsedTag["name"],float.Parse(parsedTag["time"]));
                }
                break;
            case "situ":
                SituationVoice.instance.Play(parsedTag["name"]);
                break;
            case "role":
                
                if (parsedTag.ContainsKey("frame"))
                {
                    FrameImage.sprite = _frameDic[parsedTag["frame"]];
                }

                if (parsedTag.ContainsKey("color"))
                {
                    content.GetComponent<TMP_Text>().color = parsedTag["color"]=="white"?Color.white : Color.black;
                }
                if (parsedTag.ContainsKey("sprite"))
                {
                    if (parsedTag["sprite"] == "none" && parsedTag["name"]=="Pygmalion")
                    {
                        roleHead.color = new Color(1, 1, 1, 1);
                        roleHead.transform.DOScale(new Vector3(1.77f, 1.77f, 1.77f),0);
                        roleHead.sprite = _roleSpriteDict["Pygmalion"];
                    }
                    else if (parsedTag["sprite"] == "none")
                    {
                        roleHead.sprite = null;
                        roleHead.color = new Color(1, 1, 1, 0);
                        roleHead.transform.DOScale(new Vector3(2.37f, 2.37f, 2.37f),0);
                    }else
                    {
                        roleHead.color = new Color(1, 1, 1, 1);
                        roleHead.transform.DOScale(new Vector3(2.37f, 2.37f, 2.37f),0);
                        roleHead.sprite = _roleSpriteDict[parsedTag["sprite"]];
                    }
                }
                
                if (parsedTag["name"]=="Elpis(电子)"||parsedTag["name"]=="Elpis（电子）")
                {
                    roleName.text = "内部通话";
                }
                else
                {
                    roleName.text = parsedTag["name"];
                }
                tmregular.target = !parsedTag.ContainsKey("voice") ? parsedTag["name"] : parsedTag["voice"];
                //RoleVoice.instance.Role(parsedTag["name"]);
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
                        print(scriptComponent.name);
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
                delayObj.transform.DOMove(new Vector3(0, 1, 0), 6f).OnComplete(() => { 
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
                string vpname = "";
                string role = parsedTag["role"];
                if(parsedTag.ContainsKey("name"))
                    vpname = parsedTag["name"];
                switch (parsedTag["op"])
                { 
                    case "move2":
                        x = float.Parse(parsedTag["x"], CultureInfo.InvariantCulture);
                        y = float.Parse(parsedTag["y"], CultureInfo.InvariantCulture);
                        z = float.Parse(parsedTag["z"], CultureInfo.InvariantCulture);
                        VpManager.instance.Move(role, fadeTime,new Vector3(x, y, z));
                        break;
                    case "fade":
                        float target =  float.Parse(parsedTag["fade"]);
                        VpManager.instance.Fade(role,vpname,fadeTime,target);
                        break;
                    case "scale":      
                        x = float.Parse(parsedTag["x"], CultureInfo.InvariantCulture);
                        y = float.Parse(parsedTag["y"], CultureInfo.InvariantCulture);
                        z = float.Parse(parsedTag["z"], CultureInfo.InvariantCulture);
                        VpManager.instance.Scale(role, fadeTime,new Vector3(x, y, z));
                        break;
                    case "move2o":
                        VpManager.instance.Move2O(parsedTag["name"]);
                        break;
                    case "change":
                        VpManager.instance.Change(role, vpname, parsedTag.ContainsKey("keepnone"));
                        break;
                }
                break;
            case "alert":
                
                l = ResourceLoader.textLoader[DataManager.Instance.ScriptNow].Lines[DataManager.Instance.LineNow++];
                alert.text = l;
                alert.gameObject.SetActive(true);
                break;
            case "sentence":
                switch (parsedTag["op"])
                {
                    case "create":
                        //每次遇到创建的时候就清空，然后将所有的句子都注册
                        SentenceManager.instance.endScriptsList = new List<string>();
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
                        TransAniManager.Instance.target = parsedTag["role"];
                        TransAniManager.Instance.ShowTarget();
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
                        if (parsedTag.ContainsKey("fresh"))
                            PSceneManager.Instance._currentScene = null;
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
                        img.GetComponent<SpriteRenderer>().DOFade(target, float.Parse(parsedTag["time"]));
                          //  .OnComplete(() => Debug.Log("after: " + img.GetComponent<SpriteRenderer>().color.a));
                        break;
                }
                break;                
            case "save":
                DataManager.Instance.Save(0);      
                break;
            case "text":
                switch (parsedTag["op"])
                {
                    case "show":
                        textPanel.SetActive(true);
                        text.text = parsedTag["content"];
                        text.DOFade(1, 0F);
                        break;
                    case "fade":
                        text.DOFade(0, 0.2F).OnComplete(() =>
                        {
                            textPanel.SetActive(false);
                            text.text = "";
                        });
                        break;
                }
                break;
            case "clear":
                TextLoader.Instance.gameObject.GetComponent<TMP_Text>().text = "";
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

    public GameObject textPanel;
    public TMP_Text text;
    public void EnableDiaLog()
    {
        dialog.SetActive(true);

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
                GameObject word = Instantiate(wordCloneObj,pageScript.layout.gameObject.transform);
                word.SetActive(true);
                switch (parsedTag["type"])
                {
                  case "0":
                      word.GetComponent<Word>().wordType = 0;
                      word.GetComponent<Word>().wordText.text = parsedTag["content"];
                      if (parsedTag.ContainsKey("special"))
                      {
                          word.GetComponent<Word>().answerList = new List<string>();
                          word.GetComponent<Word>().answerList.Add("/");
                          word.GetComponent<Word>().special = true;
                      }
                      
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
                          word.GetComponent<Word>().hasSpecial2NextPara = true;
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

