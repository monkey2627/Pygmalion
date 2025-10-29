using System;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class BackgroundWander : MonoBehaviour
{
    public GameObject canvas;
    public TMP_Text Text;
    [Header("移动速度（秒）")]
    public float moveDuration = 6f;

    [Header("基准方向：rotation = 0 时的前进方向")]
    public Vector2 baseDir = new Vector2(0.5f, 1f);
    
    private Tweener moveTweener;
    public Vector3 initPos;

    private void OnEnable()
    {
        transform.position = initPos;
        moveTweener?.Kill();
        GenerateSegment();
    }

    void Awake()
    {
        initPos = transform.position;
    }

    public Vector2 end;
    void Enable() => GenerateSegment();

    /// <summary>
    /// 生成“起点、终点都在范围外，且线段贯穿范围”的路径
    /// </summary>
    void GenerateSegment()
    {
        Vector2 start = initPos;
        // 已知起点在范围外，随机挑对侧外的终点即可
        // 摆到起点
        transform.position = start;
        // 一次性计算并设置朝向
        Vector2 dir = (end - start).normalized;
        float angle = Vector2.SignedAngle(baseDir, dir);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        canvas.transform.rotation = Quaternion.Euler(0, 0, 0);
        // 直线移动,移动到位置之后显示出字来
        moveTweener?.Kill();
        moveTweener = transform.DOMove(end, moveDuration)
            .SetEase(Ease.Linear).OnComplete(()=>
            {
            });
    }

    void OnDestroy() => moveTweener?.Kill();
}