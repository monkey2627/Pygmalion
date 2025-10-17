using System;
using System.Globalization;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public SaveSlot[] saveSlots;
    private void Awake()
    {
        Instance = this; 
        PlayerPrefs.SetString("FirstOpenGame","null");
        _scriptNow =PlayerPrefs.GetString("scriptNow", "0");
        _lineNow = PlayerPrefs.GetInt("lineNow", 0);
        _gameCircle =  PlayerPrefs.GetString("newGame", "true");
        
    }

    private void Start()
    {
        
    }

    private int _lineNow;
    public int LineNow
    {
        get => _lineNow;
        set { _lineNow = value;}
    }
    private string _scriptNow;
    public string ScriptNow
    {
        get => _scriptNow; 
        set { _scriptNow = value;} 
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

    public void StartNewGame()
    {
        PlayerPrefs.SetString("gameCircle", "0");
        PlayerPrefs.SetString("scriptNow","0");
        PlayerPrefs.SetInt("lineNow", 3);
    }

    public void ContinueGame()
    {
        PlayerPrefs.GetString("scriptNow", "0");
        PlayerPrefs.GetInt("lineNow", 3);
    }

    private void OnDestroy()
    {
       // PlayerPrefs.SetString("firstOpenGame",Time.time.ToString(CultureInfo.CurrentCulture));
    }

    public void Save()
    {
      //  ResourceLoader.instance.Save();
    }
    public void ResetAll()
    {

    }
}

