using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoBox : MonoBehaviour
{
    public void RefreshBox2d()
    {
        var tmp = GetComponent<TMP_Text>();
        var box2d = GetComponent<BoxCollider2D>();
        if (tmp == null || box2d == null) return;
        // 确保 TMP 已计算完宽高
        tmp.ForceMeshUpdate();

        float w = tmp.preferredWidth;
        float h = tmp.preferredHeight;

        box2d.size   = new Vector2(w, h);
        box2d.offset = Vector2.zero;   // 以文本 Pivot 为中心
    }
}
