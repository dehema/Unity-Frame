using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewContentLayout : MonoBehaviour
{
    public bool needStartRefresh = true;
  
    // Start is called before the first frame update
    void Start()
    {
        if (needStartRefresh)
        {
            setContentLayout();
        }
    }

    public void setContentLayout()
    {
        Vector2 cellSize = GetComponent<GridLayoutGroup>().cellSize;
        Vector2 spacing = GetComponent<GridLayoutGroup>().spacing;
        float spaceTop = GetComponent<GridLayoutGroup>().padding.top;
        float spaceBottom = GetComponent<GridLayoutGroup>().padding.bottom;
        int constraintCount = GetComponent<GridLayoutGroup>().constraintCount;
        int childCount = transform.childCount;
        int lineCount = childCount / constraintCount + (childCount % constraintCount == 0 ? 0 : 1);
        float height = spaceTop + spaceBottom + lineCount * cellSize.y + (lineCount - 1) * spacing.y;
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
