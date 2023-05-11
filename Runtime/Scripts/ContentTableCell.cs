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
        public class ContentTableCell : MonoBehaviour
        {
            #region PROPERTIES

            public RectTransform rect;
            public int row => rect.parent.parent.GetSiblingIndex();
            public int col => rect.parent.GetSiblingIndex();
            public TMP_Text label;
            public GameObject divider;

            #endregion
        }
    }
}