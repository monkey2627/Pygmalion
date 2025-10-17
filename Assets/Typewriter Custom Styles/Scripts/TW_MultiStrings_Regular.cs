using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TW_MultiStrings_Regular)), CanEditMultipleObjects]
[Serializable]
public class TW_MultiStrings_Regular_Editor : Editor
{
    private int indexOfString;
    private static string[] PointerSymbols = { "None", "<", "_", "|", ">" };
    private TW_MultiStrings_Regular TW_MS_RegularScript;


    private void Awake() {
        TW_MS_RegularScript = (TW_MultiStrings_Regular)target;
    }

    private void MakeArrayGUI(SerializedObject obj, string name)
    {
        int size = obj.FindProperty(name + ".Array.size").intValue;
        int newSize = size;
        if (newSize != size)
            obj.FindProperty(name + ".Array.size").intValue = newSize;
        int[] array_value = new int[newSize];
        for (int i = 1; i < newSize; i++)
        {
            array_value[i] = i;
        }
        string[] array_content = new string[newSize];
        for (int i = 1; i < newSize; i++)
        {
            array_content[i] = (array_value[i] + 1).ToString();
        }
        if (TW_MS_RegularScript.MultiStrings.Count == 0)
            EditorGUILayout.HelpBox("Number of Strings must be more than 0!", MessageType.Error);
        MakePopup(obj);
        EditorGUILayout.HelpBox("Chose number of string in PoPup and edit text in TextArea below", MessageType.Info, true);
        indexOfString = EditorGUILayout.IntPopup("Edit string №", indexOfString, array_content, array_value, EditorStyles.popup);
        TW_MS_RegularScript.MultiStrings[indexOfString] = EditorGUILayout.TextArea(TW_MS_RegularScript.MultiStrings[indexOfString], GUILayout.ExpandHeight(true));
    }

    private void MakePopup(SerializedObject obj)
    {
        TW_MS_RegularScript.pointer = EditorGUILayout.Popup("Pointer symbol", TW_MS_RegularScript.pointer, PointerSymbols, EditorStyles.popup);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SerializedObject SO = new SerializedObject(TW_MS_RegularScript);
        MakeArrayGUI(SO, "MultiStrings");
    }
}
#endif

public class TW_MultiStrings_Regular : MonoBehaviour {

    public static TW_MultiStrings_Regular instance;
    public bool LaunchOnStart = true;
    public int timeOut = 1;
    public List<string> MultiStrings = new List<string>();
    [HideInInspector]
    public int pointer=0;
    public string ORIGINAL_TEXT;

    private float time = 0f;
    private int сharIndex = 0;
    private int index_of_string = 0;
    private bool start;
    private List<int> n_l_list;
    private static string[] PointerSymbols = { "None", "<", "_", "|", ">" };
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 有更改，将初始化改到Textloader
    /// </summary>
    void Start ()
    {
      
    }
	
	void Update () {
        if (start == true){
            NewLineCheck(ORIGINAL_TEXT);
        }
    }

    public void StartTypewriter()
    {
        start = true;
        сharIndex = 0;
        time = 0f;
    }

    public void SkipTypewriter()
    {
        сharIndex = ORIGINAL_TEXT.Length - 1;
    }
    /// <summary>
    /// 开始打下一行字,注意，此处是可以实现循环的
    /// </summary>
    public void NextString()
    {
        start = true;
        finishOneText = false;
        сharIndex = 0;
        time = 0f;
        ORIGINAL_TEXT = MultiStrings[0];
    }

    public void LastString()
    {
        start = true;
        ORIGINAL_TEXT = MultiStrings[MultiStrings.Count - 1];
        сharIndex = ORIGINAL_TEXT.Length - 1;
    }

    private void NewLineCheck(string S)
    {
        if (S.Contains("\n"))
        {
            StartCoroutine(MakeTypewriterTextWithNewLine(S, GetPointerSymbol(), MakeList(S)));
        }
        else
        {
            StartCoroutine(MakeTypewriterText(S, GetPointerSymbol()));
        }
    }
    /// <summary>
    /// 标志是不是刚刚完成了一次打字
    /// </summary>
    public bool finishOneText = false;
    /// <summary>
    /// 通过协程的方式将字一行一行打出来
    /// </summary>
    /// <param name="ORIGINAL"></param>
    /// <param name="POINTER"></param>
    /// <returns></returns>
    private IEnumerator MakeTypewriterText(string ORIGINAL, string POINTER)
    {
        start = false;
        if (сharIndex != ORIGINAL.Length + 1)
        {
            string emptyString = new string(' ', ORIGINAL.Length-POINTER.Length);
            string TEXT = ORIGINAL.Substring(0, сharIndex);
            if (сharIndex < ORIGINAL.Length) TEXT = TEXT + POINTER + emptyString.Substring(сharIndex);
            gameObject.GetComponent<TMP_Text>().text = TEXT;
            time += 1;
            yield return new WaitForSeconds(0.01f);
            CharIndexPlus();
            start = true;  
        }
        else { 
            finishOneText = true;
        }
        
    }
    public void showAll()
    {
        gameObject.GetComponent<TMP_Text>().text = MultiStrings[0];
        finishOneText = true;
        сharIndex = MultiStrings[0].Length+1;
    }
    private IEnumerator MakeTypewriterTextWithNewLine(string ORIGINAL, string POINTER, List<int> List)
    {
        start = false;
        if (сharIndex != ORIGINAL.Length + 1)
        {
            string emptyString = new string(' ', ORIGINAL.Length - POINTER.Length);
            string TEXT = ORIGINAL.Substring(0, сharIndex);
            if (сharIndex < ORIGINAL.Length) TEXT = TEXT + POINTER + emptyString.Substring(сharIndex);
            TEXT = InsertNewLine(TEXT, List);
            gameObject.GetComponent<TMP_Text>().text = TEXT;
            time += 1f;
            yield return new WaitForSeconds(0.01f);
            CharIndexPlus();
            start = true;
        }
    }

    private List<int> MakeList(string S)
    {
        n_l_list = new List<int>();
        for (int i = 0; i < S.Length; i++)
        {
            if (S[i] == '\n')
            {
                n_l_list.Add(i);
            }
        }
        return n_l_list;
    }

    private string InsertNewLine(string _TEXT, List<int> _List)
    {
        for (int index = 0; index < _List.Count; index++)
        {
            if (сharIndex - 1 < _List[index])
            {
                _TEXT = _TEXT.Insert(_List[index], "\n");
            }
        }
        return _TEXT;
    }

    private string GetPointerSymbol()
    {
        if (pointer == 0){
            return "";
        }
        else{
            return PointerSymbols[pointer];
        }
    }

    private void CharIndexPlus()
    {
        if (time == timeOut)
        {
            time = 0f;
            сharIndex += 1;
        }
    }
}


