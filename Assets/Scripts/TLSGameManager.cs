/*using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TLS;
using TaskString = Cysharp.Threading.Tasks.UniTask<string>;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Video;
using System;
using System.Reflection;
using DG.Tweening;

public enum ResourseType
{
    Scene
}
[System.Serializable]
public class AAPack
{
    public string NodeName;
    public Transform ParentTran;
    public ResourseType rt;
    public Vector3 scale = Vector3.one;
}
public class ScriptReader
{
    public string textName;
    public int line;
    public string[] lines;
}
public class TLSGameManager : MonoBehaviour
{
    public GameObject Movie;
    public static TLSGameManager instance;
    public static string scriptName;
    public static int line;
    public string[] lines;
    public GameObject UI;
    public Image ProcessImg;
    public TMPro.TMP_Text ProcessText;
    [SerializeField]
    Dictionary<string, TLSSceneManager> scenesDic = new Dictionary<string, TLSSceneManager>();
    [SerializeField]
    Dictionary<string, MonoBehaviour> scriptsDic = new Dictionary<string, MonoBehaviour>();
    [SerializeField]
    public Dictionary<string, ScriptReader> scriptsSaved = new Dictionary<string,ScriptReader>();
    public AAPack[] AAPacks;
    int NowIdx = 0;
    float TotalRat = 0;
    public int dialogMode = 0;//1��ʾ�����ǵ�������е����崥����
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (AAPacks.Length > 0)
        {
            //+1
            TotalRat = (float)(NowIdx) / (float)AAPacks.Length + AO.PercentComplete / AAPacks.Length;   //AO.GetDownloadStatus().Percent
            //Debug.Log("NowIdx:" + NowIdx + " TotalRat:" + TotalRat + "  AAPacks:" + AAPacks.Length + " Percent:" + AO.PercentComplete);
            ProcessImg.fillAmount = TotalRat;
            ProcessText.text = (TotalRat * 100).ToString("F1") + "%";
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        //Ĭ�ϲ�ѭ��
        Movie.GetComponent<VideoPlayer>().isLooping = false;
        //
        Movie.GetComponent<VideoPlayer>().loopPointReached += (source) =>
        {
            Movie.GetComponent<VideoPlayer>().clip = null;
            MovieImage.enabled = false;
            isPlayingVideo = false;
        };
        Movie.GetComponent<VideoPlayer>().Play();
        ResourceLoader.Initialize();
        StartCoroutine(LoadAllResourse());
        // UI.SetActive(false);
        ProcessImg.fillAmount = 0;
        ProcessText.text = "0%";
        //load all text asset
        LoadScript("0");
        LoadScript("kitchen-hu");
    
    }
    AsyncOperationHandle<GameObject> AO;
    IEnumerator LoadAllResourse()
    {
        for (int i = 0; i < AAPacks.Length; i++)
        {
            NowIdx = i;
          
            if (AAPacks[i] != null)
            {
                if(AAPacks[i].rt == ResourseType.Scene)
                AO = Addressables.LoadAssetAsync<GameObject>(AAPacks[i].NodeName);
                yield return AO;
                if (AO.Status == AsyncOperationStatus.Succeeded)
                {
                    
                    GameObject scene = Instantiate(AO.Result);
                    scene.name = AAPacks[i].NodeName;
                    scene.transform.SetParent(AAPacks[i].ParentTran == null ? null: AAPacks[i].ParentTran);
                   // scene.AddComponent<TLSSceneManager>();
                    scene.SetActive(false);
                    scene.GetComponent<TLSSceneManager>().sceneName = scene.name;
                    scene.GetComponent<TLSSceneManager>().scene = scene;
                    scenesDic.Add(scene.name, scene.GetComponent<TLSSceneManager>());
                }
            }
        }

        scenesDic["StartScene"].scene.SetActive(true);
        ProcessImg.gameObject.SetActive(false);
        ProcessText.gameObject.SetActive(false);
    }
    public void StartNewGame()
    {

        scenesDic["StartScene"].UnLoad();
        scriptName = "0";
        line = 0;
        scriptsSaved["0"].line = 0;
        delayObj.transform.DOMove(new Vector3(1000, 1000), 2.1f).OnComplete(() => ReadLine());
    }
    private bool isPlayingVideo = false;
    public RawImage MovieImage;
    public void ReadLine()
    {
        StartCoroutine("Read");
       // StartCoroutine(LoadAllResourse)
    }
    /// <summary>
    /// ���籾�����һ�仰
    /// </summary>
    IEnumerator Read()
    {
        string l = ResourceLoader.textLoader[scriptName].lines[line++];
        scriptsSaved[scriptName].line = line;
        //��ģ��ƥ��һ����ƥ��[]��ı�ǩ
        Dictionary<string, string> parsedTag = ParseLine(l);
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
            Debug.Log("Non-tag line: " + line);
        }
        if (parsedTag.ContainsKey("delay"))
        {
            int delayTime = int.Parse(parsedTag["delay"]);
            float time = 0;
            while(time < delayTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }
        if (parsedTag["tag"] == "video")
        {
            isPlayingVideo = true;
            Debug.Log("video");
            Addressables.LoadAssetAsync<VideoClip>(parsedTag["storage"]).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    VideoClip videoClip = handle.Result;

                    // �� VideoClip ���� VideoPlayer
                    Movie.GetComponent<VideoPlayer>().clip = videoClip;

                    //Ĭ�ϲ�ѭ��
                    Movie.GetComponent<VideoPlayer>().isLooping = false;
                    //
                    Movie.GetComponent<VideoPlayer>().loopPointReached += (source) =>
                    {
                        Movie.GetComponent<VideoPlayer>().clip = null;
                        MovieImage.enabled = false;
                        isPlayingVideo = false;
                        ReadLine();
                    };
                    // ������Ƶ
                    Movie.GetComponent<VideoPlayer>().Play();
                    MovieImage.enabled = true;

                }
                else
                {
                    Debug.LogError("Failed to load video: " + parsedTag["storage"]);
                }
            };
        }
        if (parsedTag["tag"] == "scene")
        {
            if (parsedTag["load"] == "1")
            {
                scenesDic[parsedTag["name"]].Load();
             /*  List<Transform> list = new List<Transform>();
                GetAllChildrenRecursive(scenesDic[parsedTag["name"]].scene.transform, list);
                foreach (var item in list)
                {
                    if (item.GetComponent<SpriteRenderer>())
                    {
                        item.GetComponent<SpriteRenderer>().DOFade(1, 2);
                    }
                }

            }
            else
            { 
                scenesDic[parsedTag["name"]].UnLoad();
               List<Transform> list = new List<Transform>();
                GetAllChildrenRecursive(scenesDic[parsedTag["name"]].scene.transform, list);
                foreach (var item in list)
                {
                    if (item.GetComponent<SpriteRenderer>())
                    {
                        item.GetComponent<SpriteRenderer>().DOFade(0, 2).OnComplete(() => { scenesDic[parsedTag["name"]].scene.SetActive(false); });
                    }
                }
            }
        }
        if (parsedTag["tag"] == "role")
        {
            string name = parsedTag["name"];
            rolaName.text = name;
            dialog.SetActive(true);
            l = ResourceLoader.textLoader[scriptName].lines[line++];
            scriptsSaved[scriptName].line = line;
            TextLoader.instance.Push(l);
        }
        if (parsedTag["tag"] == "operation")
        {
            if (parsedTag.ContainsKey("setActive"))
            {
                GameObject obj = GameObject.Find(parsedTag["parent"]).transform.Find(parsedTag["name"]).gameObject;
                if (parsedTag["setActive"] == "true")
                {
                   
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
            else if (parsedTag.ContainsKey("enAble"))
            {
                GameObject obj = GameObject.Find(parsedTag["parent"]).transform.Find(parsedTag["obj"]).gameObject;
                // ʹ�� Type.GetType ��ȡ����
                Type scriptType = Type.GetType(parsedTag["scripts"]);

                if (scriptType != null)
                {
                    // ʹ�� GetComponent ��ȡ�ű����
                    MonoBehaviour scriptComponent = (MonoBehaviour)obj.GetComponent(scriptType);


                    if (parsedTag["enAble"] == "true")
                    {
                        scriptComponent.enabled = true;
                    }
                    else
                    {
                        scriptComponent.enabled = false;
                    }
                }
            }else if (parsedTag.ContainsKey("att"))
            {
                GameObject obj = GameObject.Find(parsedTag["parent"]).transform.Find(parsedTag["obj"]).gameObject;
                // ʹ�� Type.GetType ��ȡ����
                Type scriptType = Type.GetType(parsedTag["scripts"]);

                if (scriptType != null)
                {
                    // ʹ�� GetComponent ��ȡ�ű����
                    MonoBehaviour scriptComponent = (MonoBehaviour)obj.GetComponent(scriptType);
                    if(parsedTag["content"]=="true")
                        ModifyField(scriptComponent, parsedTag["att"], true);
                    else if (parsedTag["content"] == "false")
                        ModifyField(scriptComponent, parsedTag["att"],false);

                }
               
            }
 
        }
        if (parsedTag["tag"] == "scripts")
        {
         //  line = ResourceLoader.textLoader[].line
        
            scriptName = parsedTag["name"];
            if (parsedTag["line"] == "save")
            {
                line = scriptsSaved[scriptName].line;
            }
            else
            {
                line = int.Parse(parsedTag["line"]);
            }
        }
        
        if (parsedTag.ContainsKey("move"))
            {
                int delay = int.Parse(parsedTag["move"]);
            if (delay > 0)
                delayObj.transform.DOMove(new Vector3(1000, 1000), delay).OnComplete(() => ReadLine());
            else
                ReadLine();
            }
    }
    public GameObject delayObj;
    private Component GetScriptComponentByName(GameObject gameObject, string scriptTypeName)
    {
        // ���������ϵ��������
        Component[] components = gameObject.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component != null && component.GetType().Name == scriptTypeName)
            {
                return component; // �ҵ�ƥ��Ľű����
            }
        }
        return null; // δ�ҵ�ƥ��Ľű����
    }
    private void ModifyField(Component component, string fieldName, object newValue)
    {
        // ��ȡ���������
        Type componentType = component.GetType();

        // ʹ�� BindingFlags �������ֶ�
        FieldInfo fieldInfo = componentType.GetField(
            fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (fieldInfo != null)
        {
            // �޸��ֶ�ֵ
            fieldInfo.SetValue(component, newValue);
        }
        else
        {
            Debug.LogError("Field " + fieldName + " not found in " + componentType.Name);
        }
    }
    private void ModifyProperty(Component component, string propertyName, object newValue)
    {
        // ��ȡ���������
        Type componentType = component.GetType();
        Debug.Log(propertyName);
        Debug.Log(componentType.Name);

        // ʹ�� BindingFlags ���������Ի��ֶ�
        PropertyInfo propertyInfo = componentType.GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
        );
        if (propertyInfo != null)
        {
            // ��������Ƿ��� setter
            if (propertyInfo.CanWrite)
            {
                // �޸�����ֵ
                propertyInfo.SetValue(component, newValue);
            }
            else
            {
                Debug.LogError("Property " + propertyName + " is read-only and cannot be modified.");
            }
        }
        else
        {
            Debug.LogError("Property " + propertyName + " not found in " + componentType.Name);
        }
    }
    public GameObject dialog;
    public TMPro.TMP_Text rolaName;
    // �ݹ麯������ȡ����������
    void GetAllChildrenRecursive(Transform parent, List<Transform> childrenList)
    {
        // ������ǰ���������������
        foreach (Transform child in parent)
        {
            // ����ǰ��������ӵ��б���
            childrenList.Add(child);

            // �����ǰ�����廹�������壬�����ݹ�
            if (child.childCount > 0)
            {
                GetAllChildrenRecursive(child, childrenList);
            }
        }
    }
    private Dictionary<string, string> ParseLine(string line)
    {
        // ������ʽƥ�� [��ǩ ����=ֵ]
        Regex tagRegex = new Regex(@"\[(.*?)\]");
        Match match = tagRegex.Match(line);

        if (match.Success)
        {
            string tagContent = match.Groups[1].Value.Trim();
            Dictionary<string, string> tagAttributes = new Dictionary<string, string>();

            // �ָ��ǩ���ݣ��Կո�Ϊ�ָ���
            string[] parts = tagContent.Split(' ');
            string tagName = parts[0]; // ��һ�������Ǳ�ǩ��
            tagAttributes["tag"] = tagName;

            // ��������������
            for (int i = 1; i < parts.Length; i++)
            {
                string attribute = parts[i];
                if (attribute.Contains("="))
                {
                    string[] keyValue = attribute.Split(new char[] { '=' }, 2);
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        tagAttributes[key] = value;
                    }
                }
            }

            return tagAttributes;
        }

        return null; // ���û��ƥ�䵽��ǩ������ null
    }
    public async TaskString LoadScript(string storage)
    {
        string script = await TLS.ResourceLoader.LoadText(storage);
        scriptsSaved.Add(storage, new ScriptReader() { textName = storage, line = 0, lines = ResourceLoader.textLoader[storage].lines });
        return script;
    }

    private Dictionary<string, List<bool>> flagDatas;

    public static readonly string readFlagData = "ReadFlags.dat";
    public void SaveReadFlag()
    {
        string path = Application.persistentDataPath + "/" + readFlagData;
#if true
        string jsonData = JsonConvert.SerializeObject(flagDatas);//, Formatting.Indented);

        File.WriteAllText(path, jsonData);
#else
            System.Runtime.Serialization.DataContractSerializer dataSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Dictionary<string, List<bool>>));

            MemoryStream dataStream = new MemoryStream();
            using (var binWriter = System.Xml.XmlDictionaryWriter.CreateBinaryWriter(dataStream))
            {
                dataSerializer.WriteObject(binWriter, flagDatas);

                byte[] binArray = dataStream.ToArray();

                BinaryWriter writer = new BinaryWriter(File.OpenWrite(path));
                writer.Write(binArray);
            }
#endif
    }
    public void LoadReadFlag()
    {
        string path = Application.persistentDataPath + "/" + readFlagData;
#if true
        //            string jsonData =  PlayerPrefs.GetString(readFlagData);

        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(jsonData))
                flagDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<bool>>>(jsonData);
        }

        if (flagDatas == null)
            flagDatas = new Dictionary<string, List<bool>>();

        /*
        if(flagDatas != null)
                foreach (KeyValuePair<string, string> script in flagDatas)
                {
                    scriptInstance[script.Key].resourse.SetReadFlagJsonData(script.Value);
                }
        
#else
#if UNITY_EDITOR
            string path = Directory.GetCurrentDirectory();
#else
            string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
            path += ("/" + readFlagData);

            //            BinaryReader reader = new BinaryReader(File.OpenRead(path));

            using (FileStream fileStream = new FileStream(path, FileMode.Open))//, FileAccess.Read);
            {
                //            MemoryStream dataStream = new MemoryStream();

                if (fileStream != null)
                {
                    using (var binReader = System.Xml.XmlDictionaryReader.CreateBinaryReader(fileStream, System.Xml.XmlDictionaryReaderQuotas.Max))
                    {
                        System.Runtime.Serialization.DataContractSerializer dataSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Dictionary<string, List<bool>>));
                        flagDatas = (Dictionary<string, List<bool>>)dataSerializer.ReadObject(binReader);

                        if (flagDatas == null)
                            flagDatas = new Dictionary<string, List<bool>>();
                    }
                }
            }
#endif
    }
}
*/