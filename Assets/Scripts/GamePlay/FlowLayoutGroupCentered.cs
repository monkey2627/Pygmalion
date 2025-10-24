using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GamePlay
{
    public class FlowLayoutGroupCentered : MonoBehaviour
    {
        public float lineHeightScale = 1.2f;
        public float lineSpacing = 5f;

        public RectTransform rt;

        /* 用来存每行的信息 */
        private class Line
        {
            public List<RectTransform> segments = new();
            public float width;
        }

        private void OnEnable()
        {
            Refresh();
        }

        private List<Line> lines = new();
        [ContextMenu("Refresh")]
        public void Refresh()
        {
            float usableWidth =  rt.rect.width;
            /* ---------- 第一遍：流式排布，同时收集行信息 ---------- */
            lines.Clear();
            Line curLine = null;
            float curX = 0, curY = 0f, lineH = 0f;
            for (int i = 0; i < rt.childCount; i++)//从上到下遍历所有的
            {
                var child = rt.GetChild(i) as RectTransform;
                if(child.GetComponent<Word>())
                    child.GetComponent<Word>().RefreshBox2d();
                var text = child.GetComponent<TMP_Text>();
                if (!text) continue;

                text.ForceMeshUpdate();
                float w = text.preferredWidth;
                float h = text.preferredHeight;

                /* 需要换行 */
                if (curX > 0 && curX + w > usableWidth)
                {
                    curY -= 60;
                    curX = 0f;
                }

                /* 新建一行记录 */
                if (curX == 0f)
                {
                    curLine = new Line();
                    lines.Add(curLine);
                }

                /* 先临时放左上角，后面统一居中 */
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0, 1);
                child.anchoredPosition = new Vector2(curX, curY);

                curLine.segments.Add(child);
                curLine.width = curX + w;   // 记录行总宽
                curX += w;
                lineH = Mathf.Max(lineH, h);
            }

            /* ---------- 第二遍：每行整体居中 ---------- */
            foreach (Line line in lines)
            {
                float offset = (usableWidth - line.width) * 0.5f;
                foreach (var seg in line.segments)
                {
                    Vector2 v = seg.anchoredPosition;
                    v.x += offset;
                    seg.anchoredPosition = v;
                }
            }
        }

        /* 运行时加段接口照旧 */
        public void AddSegment(string txt, Color color, bool bold = false)
        {
            var go = new GameObject("Segment", typeof(RectTransform), typeof(TMP_Text));
            go.transform.SetParent(rt, false);
            var t = go.GetComponent<TMP_Text>();
            t.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>" +
                     (bold ? "<b>" : "") + txt + (bold ? "</b>" : "") +
                     "</color>";
            t.fontSize = 24;
            t.enableWordWrapping = false;
            Refresh();
        }
    }
}