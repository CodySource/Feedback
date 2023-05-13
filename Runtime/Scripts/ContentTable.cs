using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace CodySource
{
    namespace Feedback
    {
        [ExecuteAlways]
        public class ContentTable : MonoBehaviour
        {

            #region PROPERTIES

            public ScrollRect headerScroll = null;
            public ScrollRect contentScroll = null;
            public Transform headerParent = null;
            public Transform contentParent = null;
            public List<ContentTableHeader> headers = new List<ContentTableHeader>();
            public List<ContentTableCell> cells = new List<ContentTableCell>();

            [Header("LOADING DATA")]
            public GameObject headerCellPrefab = null;
            public GameObject entryRowPrefab = null;
            public GameObject entryCellPrefab = null;
            private int cachedHeaderCount = 0;
            private List<GameObject> headerPool = new List<GameObject>();
            private List<GameObject> entryPool = new List<GameObject>();
            private Dictionary<GameObject, List<GameObject>> cellPool = new Dictionary<GameObject, List<GameObject>>();

            #endregion

            #region PUBLIC METHODS

            public void Draw()
            {
                if (headerParent == null || contentParent == null) return;
                headers = new List<ContentTableHeader>(headerParent.GetComponentsInChildren<ContentTableHeader>());
                cells = new List<ContentTableCell>(contentParent.GetComponentsInChildren<ContentTableCell>());
                headers.ForEach(h =>
                    cells.FindAll(c => c.col == h.col).ForEach(c =>
                        c.rect.sizeDelta = new Vector2(h.cellWidth, c.rect.sizeDelta.y)));
                headers.ForEach(h => h.divider.SetActive(h.col <= headers.Count + 1));
                cells.ForEach(c => c.divider.SetActive(c.col <= headers.Count + 1));
                headers.ForEach(h => {
                    RectTransform row = headerParent.GetComponent<RectTransform>();
                    HorizontalLayoutGroup group = h.GetComponent<HorizontalLayoutGroup>();
                    RectTransform img = h.divider.transform.GetChild(0).GetComponent<RectTransform>();
                    img.sizeDelta = new Vector2(img.sizeDelta.x, row.sizeDelta.y - group.padding.top - group.padding.bottom);
                    img.anchoredPosition = new Vector2(0f, -group.padding.top);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(row);
                });
                cells.ForEach(c =>
                {
                    RectTransform row = c.transform.parent.GetComponent<RectTransform>();
                    HorizontalLayoutGroup group = c.GetComponent<HorizontalLayoutGroup>();
                    RectTransform img = c.divider.transform.GetChild(0).GetComponent<RectTransform>();
                    img.sizeDelta = new Vector2(img.sizeDelta.x, row.sizeDelta.y - group.padding.top - group.padding.bottom);
                    img.anchoredPosition = new Vector2(0f, -group.padding.top);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(row);
                });
            }

            public void LoadData(Table pTable)
            {
                headerPool.ForEach(h => h.SetActive(false));
                //  Load headers
                for (int i = 0; i < pTable.headers.Length; i++)
                {
                    if (headerPool.Count > i) headerPool[i].SetActive(true);
                    else headerPool.Add(Instantiate(headerCellPrefab, headerParent));
                    ContentTableHeader header = headerPool[i].GetComponent<ContentTableHeader>();
                    header.element.minWidth = Mathf.Max(pTable.headers[i].width, 100f);
                    header.label.text = pTable.headers[i].value;
                    header.divider.transform.SetParent(headerParent);
                }
                //  Wipe cell entries if there is a change in header count
                if (headerPool.Count != cachedHeaderCount && cachedHeaderCount != 0)
                {
                    headerPool.ForEach(h => Destroy(h.gameObject));
                    headerPool.Clear();
                    cellPool.Clear();
                }
                //  Load entries
                for (int e = 0; e < pTable.entries.Length; e++)
                {
                    if (entryPool.Count > e) entryPool[e].SetActive(true);
                    else
                    {
                        entryPool.Add(Instantiate(entryRowPrefab, contentParent));
                        cellPool.Add(entryPool[e], new List<GameObject>());
                    }
                    if (pTable.entries[e].color != Color.clear) entryPool[e].GetComponent<Image>().color = pTable.entries[e].color;
                    else entryPool[e].GetComponent<Image>().color = entryRowPrefab.GetComponent<Image>().color;
                    for (int c = 0; c < pTable.entries[e].values.Length; c++)
                    {
                        if (cellPool[entryPool[e]].Count > c) cellPool[entryPool[e]][c].SetActive(true);
                        else cellPool[entryPool[e]].Add(Instantiate(entryCellPrefab, entryPool[e].transform));
                        ContentTableCell cell = cellPool[entryPool[e]][c].GetComponent<ContentTableCell>();
                        cell.label.text = pTable.entries[e].values[c];
                        cell.divider.transform.SetParent(entryPool[e].transform);
                    }
                }
                cachedHeaderCount = headerPool.Count;
                headers = new List<ContentTableHeader>(headerParent.GetComponentsInChildren<ContentTableHeader>());
                cells = new List<ContentTableCell>(contentParent.GetComponentsInChildren<ContentTableCell>());
                StartCoroutine(_CompleteDraw());
            }

            #endregion

            #region PRIVATE METHODS

            private void OnEnable() => StartCoroutine(_CompleteDraw());

            private void LateUpdate()
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying && EditorUtility.IsDirty(GetComponent<RectTransform>().GetInstanceID())) Draw();
#endif
                if (headerScroll != null && contentScroll != null && headerScroll.horizontalNormalizedPosition != contentScroll.horizontalNormalizedPosition) headerScroll.horizontalNormalizedPosition = contentScroll.horizontalNormalizedPosition;
            }

            private IEnumerator _CompleteDraw()
            {
                int frames = 3;
                while (frames > 0)
                {
                    frames--;
                    yield return new WaitForEndOfFrame();
                }
                Draw();
            }

            #endregion

        }

        [System.Serializable]
        public struct Table
        {
            public Header[] headers;
            public Entry[] entries;
        }

        [System.Serializable]
        public struct Header
        {
            public float width;
            public string value;
        }

        [System.Serializable]
        public struct Entry
        {
            public Color color;
            public string[] values;
        }
    }
}