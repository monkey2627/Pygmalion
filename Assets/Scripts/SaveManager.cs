using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SaveManager 
{
   /* public static SaveManager instance;
    public static readonly string SAVE_FOLDER = Path.Combine(Application.persistentDataPath, "Saves");
    public int saveNumber = 0;
   public void Init()
    {
      
        instance = this;
        if (!Directory.Exists(SAVE_FOLDER))
            Directory.CreateDirectory(SAVE_FOLDER);
        //�������浵�������������浵��Ӧ�ļ�����������
        saveNumber = PlayerPrefs.GetInt("saveNumber", 0);
        for(int i = 1; i <= saveNumber; i++)
        {
            int slot = PlayerPrefs.GetInt("slotid" + i.ToString());
            DataManager.Instance.saveSlots[slot].id = i;
        }
    }
    public void Destroy()
    {
        PlayerPrefs.SetInt("saveNumber", saveNumber);
    }
    /// <summary>
    /// ���浱ǰ�浵��save_id.json�ļ�
    /// </summary>
    /// <param name="id"></param>
    public void Save(int id,Texture2D img)
    {
        string path = Path.Combine(SAVE_FOLDER, $"save_{id}.json");
        PlayerProgress saveData = new PlayerProgress();        
        saveData.img = Utils.TextureToBase64(img);
        saveData.saveTime = DateTime.Now.ToString();
        saveData.line = DataManager.instance.LineNow;
        saveData.scriptNow = DataManager.instance.ScriptNow;
        //����ű���������
        foreach ( var item in ResourceLoader.textLoader)
        {
            saveData.scriptsName.Add(item.Key);
            saveData.scriptsLine.Add(item.Value.lineIndex);
        }
        string json = JsonUtility.ToJson(saveData);
        byte[] bytes = Encoding.UTF8.GetBytes(json);        
        byte[] cipher = Utils.Encrypt(bytes);
        //��pointerλ�� ״̬
        saveData.pPos = PygmalionGameManager.Instance.pointer.transform.position;
        saveData.pEnable = PygmalionGameManager.Instance.pEnable;
        File.WriteAllBytes(path,cipher);
        foreach (var item in PygmalionGameManager.Instance.scenesDic.Keys)
        {
            PygmalionGameManager.Instance.scenesDic[item].save(id);
        }
    }
    public void Load(int id)
    {
        string path = Path.Combine(SAVE_FOLDER, $"save_{id}.json");
        if (!File.Exists(path)) 
                return;

        byte[] cipher = File.ReadAllBytes(path);               // 1. ��������
        byte[] plain = Utils.Decrypt(cipher);                // 2. ����
        string json = Encoding.UTF8.GetString(plain);        // 3. �ֽ� -> �ַ���
        var data =  JsonUtility.FromJson<PlayerProgress>(json);
        DataManager.instance.LineNow = data.line;
        DataManager.instance.ScriptNow = data.scriptNow;
        for (int i = 0; i < data.scriptsName.Count;i++)
        {
            var item = data.scriptsName[i];
            ResourceLoader.textLoader[item].lineIndex = data.scriptsLine[i];
        }
        PygmalionGameManager.Instance.pointer.transform.position = data.pPos;
        if (data.pEnable == 1)
        {
            PygmalionGameManager.Instance.pointer.SetActive(true);
            PygmalionGameManager.Instance.pEnable = 1;
        }
        else
        {
            PygmalionGameManager.Instance.pEnable = 0;
            PygmalionGameManager.Instance.pointer.SetActive(false);
        }
        foreach (var item in PygmalionGameManager.Instance.scenesDic.Keys)
        {
            PygmalionGameManager.Instance.scenesDic[item].load(id);
        }
    }*/
}
[Serializable]   // ���룡���� JsonUtility �޷����л�
public class PlayerProgress
{
    public string name;                 //�浵������
    public string img;                  //�浵��ʾ��ͼƬ
    public string saveTime;             // �浵ʱ��
    public int line;                    //�ڵ�ǰlines�����ڼ���
    public string scriptNow;            // ��ǰ�½�����
    public List<string> scriptsName;
    public List<int> scriptsLine;
    public List<string> lines;
    public Vector3 pPos;
    public int pEnable;
}



