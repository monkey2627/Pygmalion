using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Word : MonoBehaviour
{
    //点击词语，根据词语的种类不同应该是有不同的表现
    public int wordType;
    public TMP_Text wordText; 
    public int sentenceNumber;
    public string addText;
    public List<string> changeWordList;
    public int nextSentenceNumber;
    public bool enable;
}
