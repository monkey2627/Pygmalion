using System;
using System.Globalization;
using System.IO;
using Ani;
using Scene;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private void Awake()
    {
        Instance = this;
        _gameCircle = PlayerPrefs.GetString("gameCircle","null");
    }
    private int _lineNow;
    public int LineNow
    {
        get => _lineNow;
        set
        {
            _lineNow = value;
        }
    }
    private string _scriptNow;
    public string ScriptNow
    {
        get => _scriptNow; 
        set { _scriptNow = value;
            
        } 
    }

    private string _gameCircle;

    /// <summary>
    /// 0代表处于最开始的那个视频阶段
    /// 1代表视频阶段已过完，检测到为1时，打开游戏应该直接出现新游戏界面
    /// </summary>
    public string GameCircle
    {
        get => _gameCircle;
        set { _gameCircle = value;}
    }

    /// <summary>
    /// 只在一整个操作之后save
    /// </summary>
    [System.Serializable]
    public class SaveDataManagerData
    {
        public string SaveTime;
        public int lineNow;
        public string scriptNow;
        
    }
    private static readonly string AutoSaveFile = "AutoSaveDataManagerData.json";
    private static readonly string PersonSaveFile = "PersonSaveDataManagerData.json";
    public void Save(int type=0)
    {
        SaveDataManagerData data = new SaveDataManagerData();
        data.lineNow = LineNow;
        data.scriptNow = ScriptNow;
        data.SaveTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        print("save at "+data.SaveTime);
        string json = JsonUtility.ToJson(data, true);  
        if (type == 0)//autoSave
        {
              File.WriteAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile), json);    
        }
        else
        {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile), json);
        }   
        PygmalionGameManager.Instance.Save(type);
        if (PygmalionGameManager.Instance.isGameTime)
        {
                SentenceManager.instance.Save(type);
        }
        VpManager.instance.SaveAllVp(type);
        GuideSceneGamePlay.instance.Save(type);
        TransAniManager.Instance.Save(type);
        Debug.Log($"[DataManager] 已保存");
    }

    public void PersonSave()
    {
        Save(1);
    }
    /// <summary>
    /// 开始新游戏，从新手指导开始
    /// </summary>
    public void StartNewGame()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, AutoSaveFile));
        if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
            File.Delete(Path.Combine(Application.persistentDataPath, PersonSaveFile));
        _gameCircle = "1";
        _scriptNow = "0";
        _lineNow = 0;
    }

    public int ContinueGame()
    {
        int save = 0;
        if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)) && File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
        {
            string jsonAuto = File.ReadAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile)); 
            print(jsonAuto);
            SaveDataManagerData autodata = JsonUtility.FromJson<SaveDataManagerData>(jsonAuto);
            string jsonPerson = File.ReadAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile));
            print(jsonPerson);
            SaveDataManagerData personData = JsonUtility.FromJson<SaveDataManagerData>(jsonPerson);
            DateTime auto = DateTime.ParseExact(
                autodata.SaveTime,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            
            DateTime person = DateTime.ParseExact(
                personData.SaveTime,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            print("have both");
            print(auto);
            print(person);
            if (auto > person)
            {
                print("a");
                _lineNow = autodata.lineNow;
                _scriptNow =  autodata.scriptNow;
                print(_scriptNow);
                return 0;
            }else if (auto <= person)
            {
                _lineNow = personData.lineNow;
                _scriptNow = personData.scriptNow;
                print("b");
                print(_scriptNow);
                return 1;
            }
        }
        else if (File.Exists(Path.Combine(Application.persistentDataPath, AutoSaveFile)))
        {
            print("c");
            string jsonAuto = File.ReadAllText(Path.Combine(Application.persistentDataPath, AutoSaveFile)); 
            SaveDataManagerData autodata = JsonUtility.FromJson<SaveDataManagerData>(jsonAuto);
            _lineNow = autodata.lineNow;
            _scriptNow =  autodata.scriptNow;
            return 0;
        }else if (File.Exists(Path.Combine(Application.persistentDataPath, PersonSaveFile)))
        {
            print("d");
            string jsonPerson = File.ReadAllText(Path.Combine(Application.persistentDataPath, PersonSaveFile));
            SaveDataManagerData personData = JsonUtility.FromJson<SaveDataManagerData>(jsonPerson);
            _lineNow = personData.lineNow;
            _scriptNow =  personData.scriptNow;
            return 1;
        }

        return -1;
    }


}

