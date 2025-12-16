using TMPro;
using UnityEngine;

/// <summary>
/// 屏幕顶部一次性横向滚动红色小字，跑完自动销毁。
/// </summary>
public class AlertScroll : MonoBehaviour
{
    public static AlertScroll instance;
    public TMPro.TMP_Text alertText;
    public GameObject labCrash;
    public GameObject galleryCrash;
    private float timeCounter = 0;
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timeCounter = 0;
    }

    private void Update()
    {
            timeCounter += Time.deltaTime;
            if (this.timeCounter >= 4)
            {
                gameObject.SetActive(false);
                labCrash.SetActive(false);
                galleryCrash.SetActive(false);
                PygmalionGameManager.instance.ReadLine();
            }
    }
}