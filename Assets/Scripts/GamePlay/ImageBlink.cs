using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageBlink : MonoBehaviour
{
    [Header("闪动频率（次/秒）")]
    public float frequency = 2f;

    [Range(0,1)]
    public float minAlpha = 0.4f;   // 最低透明度

    [Range(0,1)]
    public float maxAlpha = 1.0f;   // 最高透明度

    private Image img;
    private Color originalColor;

    void Awake()
    {
        img = GetComponent<Image>();
        originalColor = img.color;
    }

    void Update()
    {
        // 0~1 的三角波
        float t = Mathf.PingPong(Time.time * frequency, 1f);
        // 映射到 minAlpha~maxAlpha
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        img.color = new Color(originalColor.r,
            originalColor.g,
            originalColor.b,
            alpha);
    }
}