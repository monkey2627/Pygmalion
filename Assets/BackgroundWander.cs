using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class BackgroundWander : MonoBehaviour
{
    public GameObject canvas;
    [Header("移动范围（世界坐标）")]
    public float xMin = -10f, xMax = 10f;
    public float yMin = -6f, yMax = 6f;

    [Header("边界外侧缓冲区")]
    public float margin = 2f;

    [Header("移动速度（秒）")]
    public float moveDuration = 6f;

    [Header("基准方向：rotation = 0 时的前进方向")]
    public Vector2 baseDir = new Vector2(0.5f, 1f);

    private Transform mTrans;
    private Tweener moveTweener;
    public Vector3 initPos;

    private void OnEnable()
    {
        transform.position = initPos;
        moveTweener?.Kill();
        GenerateSegment();
    }

    void Awake() => mTrans = transform;

    void Start() => GenerateSegment();

    /// <summary>
    /// 生成“起点、终点都在范围外，且线段贯穿范围”的路径
    /// </summary>
    void GenerateSegment()
    {
        Vector2 start = gameObject.transform.position;
        
        // 已知起点在范围外，随机挑对侧外的终点即可
        Vector2 end;
        if (start.x < xMin)        // 起点在左侧外 → 终点去右侧外
            end = new Vector2(xMax + Random.Range(margin, margin + 3f), Random.Range(yMin, yMax));
        else if (start.x > xMax)   // 起点在右侧外 → 终点去左侧外
            end = new Vector2(xMin - Random.Range(margin, margin + 3f), Random.Range(yMin, yMax));
        else if (start.y < yMin)   // 起点在下侧外 → 终点去上侧外
            end = new Vector2(Random.Range(xMin, xMax), yMax + Random.Range(margin, margin + 3f));
        else                       // 起点在上侧外 → 终点去下侧外
            end = new Vector2(Random.Range(xMin, xMax), yMin - Random.Range(margin, margin + 3f));
        // 摆到起点
        mTrans.position = start;

        // 一次性计算并设置朝向
        Vector2 dir = (end - start).normalized;
        float angle = Vector2.SignedAngle(baseDir, dir);
        mTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
        // 直线移动
        moveTweener?.Kill();
        moveTweener = mTrans.DOMove(end, moveDuration)
                            .SetEase(Ease.Linear)
                            .OnComplete(GenerateSegment);
    }

    void OnDestroy() => moveTweener?.Kill();
}