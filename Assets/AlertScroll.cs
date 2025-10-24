using TMPro;
using UnityEngine;

/// <summary>
/// 屏幕顶部一次性横向滚动红色小字，跑完自动销毁。
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class AlertScroll : MonoBehaviour
{
    [Header("滚动文字")]
    [TextArea]
    public string text = "/ / /ALERT – Bio-Perception Unit: Illicit emotional input detected. Containment Level Ω engaged. / / /";

    [Header("滚动时间（秒）")]
    public float scrollDuration = 6f;

    [Header("起始偏移（像素）")]
    public float startOffset = 1920f;   // 从屏幕右边外开始

    [Header("结束偏移（像素）")]
    public float endOffset = -1920f;    // 到屏幕左边外结束

    private TextMeshProUGUI tmp;
    private RectTransform rt;
    private float timer;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        rt = GetComponent<RectTransform>();
        tmp.text = text;
        tmp.ForceMeshUpdate();          // 立即计算宽度
    }

    private void OnEnable()
    {
        // 初始位置：屏幕右边外
        Vector2 p = rt.anchoredPosition;
        p.x = startOffset;
        rt.anchoredPosition = p;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / scrollDuration;      // 0~1
        if (t >= 1f)
        {
            PygmalionGameManager.Instance.ReadLine();
            // 跑完自动销毁
            gameObject.SetActive(false);
            return;
        }

        // 线性插值移动
        float x = Mathf.Lerp(startOffset, endOffset, t);
        Vector2 pos = rt.anchoredPosition;
        pos.x = x;
        rt.anchoredPosition = pos;
    }
}