using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    /// <summary>
    /// չʾ�ڲ��ϵ�ͼƬ
    /// </summary>
    public Texture2D img;
    /// <summary>
    /// ������϶�Ӧ�Ĵ浵id,ֻ�������йأ����浵������ʱid���ı�
    /// </summary>
    public int id = 0;
    void Start()
    {
        
    }

   /*public void Save()
    {
        img = Utils.Capture();

        // �ж�����ط��Ƿ��д浵
        if(id == 0)
        {
            SaveManager.instance.saveNumber++;
            id = SaveManager.instance.saveNumber;
            SaveManager.instance.Save(id,img);
        }
        else
        {
            SaveManager.instance.Save(id,img);
        }
    }*/ 
}
