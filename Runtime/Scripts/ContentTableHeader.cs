using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CodySource
{
    namespace Feedback
    {
        [ExecuteAlways]
        public class ContentTableHeader : MonoBehaviour
        {
            #region PROPERTIES

            public RectTransform rect;
            public HorizontalLayoutGroup layout;
            public int col => rect.GetSiblingIndex();
            public float cellWidth => (rect != null && layout != null) ? rect.rect.width - layout.padding.left - layout.padding.right : 0f;
            public TMP_Text label;
            public LayoutElement element;
            public GameObject divider;

            #endregion
        }
    }
}