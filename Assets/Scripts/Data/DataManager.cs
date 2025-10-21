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
    public void Save(int type=0)
    {
        if (type == 0)//说明是自动存
        {
            PlayerPrefs.SetString("scriptNow", _scriptNow);
            PlayerPrefs.SetInt( "lineNow", _lineNow);
            PlayerPrefs.SetString("currentScene", PSceneManager.Instance._currentScene.name);
            //存所有vp的位置、颜色、大小
            VpManager.Instance.SaveAllVp(type);
            //存场景中一些重要物体的状态
            GameManager.Instance.Save(type);
        }
    }
    /// <summary>
    /// 开始新游戏，从新手指导开始
    /// </summary>
    public void StartNewGame()
    {
        _gameCircle = "1";
        _scriptNow = "0";
        _lineNow = 0;
        PlayerPrefs.SetString("gameCircle","1");
        PlayerPrefs.SetString("scriptNow","0");
        PlayerPrefs.SetInt("lineNow", 0);
    }

    public void ContinueGame()
    {
        PlayerPrefs.GetString("scriptNow", "0");
        PlayerPrefs.GetInt("lineNow", 3);
    }


}

