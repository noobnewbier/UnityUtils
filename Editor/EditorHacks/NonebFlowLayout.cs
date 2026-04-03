#nullable disable

using UnityEditor;
using UnityEngine;

namespace NonebNi.EditorHacks
{
    /// <summary>
    /// Copied from <see cref="FlowLayout" /> decompiled, if it works you lucked out
    /// </summary>
    internal class NonebFlowLayout : GUILayoutGroup
    {
        private LineInfo[] m_LineInfo;
        private int m_Lines;

        public override void CalcWidth()
        {
            var flag = minWidth != 0.0;
            base.CalcWidth();
            if (isVertical || flag)
                return;
            minWidth = 0.0f;
            foreach (var entry in entries)
                minWidth = Mathf.Max(m_ChildMinWidth, entry.minWidth);
        }

        public override void SetHorizontal(float x, float width)
        {
            base.SetHorizontal(x, width);
            if (resetCoords)
                x = 0.0f;
            if (isVertical)
            {
                Debug.LogError("Wordwrapped vertical group. Don't. Just Don't");
            }
            else
            {
                m_Lines = 0;
                var pulledOffset = 0.0f; // How far we need to pull each item back.

                #region Logic Changed Regions

                var firstEntryX = 0f;
                var leftPaddingForIndent = 0f;
                var moreThanOneEntryHack = entries.Count > 1;
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    /*
                     * Note:
                     * Not sure why, but when NonebUniversalInspector is trying to draw buttons,
                     * and when there's only one buttons(i.e one entries), the following condition can become true,
                     * meaning there will be more than one lines.
                     *
                     * Which makes no sense as we only have one entry, so this is an attempt to make this work in that case.
                     */
                    if (moreThanOneEntryHack)
                    {
                        if (entry.rect.xMax - (double)pulledOffset > x + (double)width)
                        {
                            pulledOffset = entry.rect.x - entry.marginLeft;
                            ++m_Lines;
                        }
                    }

                    var childX = entry.rect.x - pulledOffset;
                    switch (i)
                    {
                        case > 0:
                            if (firstEntryX > childX) leftPaddingForIndent = firstEntryX - childX;

                            break;
                        case 0:
                            firstEntryX = childX;
                            break;
                    }

                    childX += leftPaddingForIndent;

                    entry.SetHorizontal(childX, entry.rect.width);

                    entry.rect.y = m_Lines;
                }

                #endregion


                ++m_Lines;
            }
        }

        public override void CalcHeight()
        {
            if (entries.Count == 0)
            {
                maxHeight = minHeight = 0;
                return;
            }

            m_ChildMinHeight = m_ChildMaxHeight = 0;
            int topMarginMin = 0, bottomMarginMin = 0;
            m_StretchableCountY = 0;
            if (isVertical) { }
            else
            {
                m_LineInfo = new LineInfo[m_Lines];
                for (int i = 0; i < m_Lines; i++)
                {
                    m_LineInfo[i].topBorder = 10000;
                    m_LineInfo[i].bottomBorder = 10000;
                }

                // Figure out border values for each line
                foreach (GUILayoutEntry i in entries)
                {
                    i.CalcHeight();
                    int j = (int)i.rect.y;
                    m_LineInfo[j].minSize = Mathf.Max(i.minHeight, m_LineInfo[j].minSize);
                    m_LineInfo[j].maxSize = Mathf.Max(i.maxHeight, m_LineInfo[j].maxSize);
                    m_LineInfo[j].topBorder = Mathf.Min(i.marginTop, m_LineInfo[j].topBorder);
                    m_LineInfo[j].bottomBorder = Mathf.Min(i.marginBottom, m_LineInfo[j].bottomBorder);
                }

                for (int i = 0; i < m_Lines; i++)
                {
                    m_ChildMinHeight += m_LineInfo[i].minSize;
                    m_ChildMaxHeight += m_LineInfo[i].maxSize;
                }

                // Add in the the extra lines
                for (int i = 1; i < m_Lines; i++)
                {
                    float space = Mathf.Max(m_LineInfo[i - 1].bottomBorder, m_LineInfo[i].topBorder);
                    m_ChildMinHeight += space;
                    m_ChildMaxHeight += space;
                }

                topMarginMin = m_LineInfo[0].topBorder;
                bottomMarginMin = m_LineInfo[m_LineInfo.Length - 1].bottomBorder;
            }

            // Do the dance between children & parent for haggling over how many empty pixels to have

            m_MarginTop = topMarginMin;
            m_MarginBottom = bottomMarginMin;
            var lastPadding = 0f;
            var firstPadding = lastPadding;

            minHeight = Mathf.Max(minHeight, m_ChildMinHeight + firstPadding + lastPadding);
            if (maxHeight == 0)
            {
                stretchHeight += m_StretchableCountY + (style.stretchHeight ?
                    1 :
                    0);
                maxHeight = m_ChildMaxHeight + firstPadding + lastPadding;
            }
            else
            {
                stretchHeight = 0;
            }

            maxHeight = Mathf.Max(maxHeight, minHeight);
        }

        public override void SetVertical(float y, float height)
        {
            if (entries.Count == 0)
            {
                base.SetVertical(y, height);
            }
            else if (isVertical)
            {
                base.SetVertical(y, height);
            }
            else
            {
                if (resetCoords)
                    y = 0.0f;
                var num1 = y - marginTop;
                var num2 = y + marginVertical - spacing * (m_Lines - 1);
                var t = 0.0f;
                if (m_ChildMinHeight != (double)m_ChildMaxHeight)
                    t = Mathf.Clamp((float)((num2 - (double)m_ChildMinHeight) / (m_ChildMaxHeight - (double)m_ChildMinHeight)), 0.0f, 1f);
                var num3 = num1;
                for (var index = 0; index < m_Lines; ++index)
                {
                    if (index > 0)
                        num3 += Mathf.Max(m_LineInfo[index].topBorder, m_LineInfo[index - 1].bottomBorder);
                    m_LineInfo[index].start = num3;
                    m_LineInfo[index].size = Mathf.Lerp(m_LineInfo[index].minSize, m_LineInfo[index].maxSize, t);
                    num3 += m_LineInfo[index].size + spacing;
                }

                foreach (var entry in entries)
                {
                    var lineInfo = m_LineInfo[(int)entry.rect.y];
                    if (entry.stretchHeight != 0)
                        entry.SetVertical(lineInfo.start + entry.marginTop, lineInfo.size - entry.marginVertical);
                    else
                        entry.SetVertical
                        (
                            lineInfo.start + entry.marginTop,
                            Mathf.Clamp(lineInfo.size - entry.marginVertical, entry.minHeight, entry.maxHeight)
                        );
                }
            }
        }

        private struct LineInfo
        {
            public float minSize;
            public float maxSize;
            public float start;
            public float size;
            public int topBorder;
            public int bottomBorder;
        }
    }
}