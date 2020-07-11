using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ContentResizeComponent : MonoBehaviour
{
    private RectTransform rectTransform;
    private VerticalLayoutGroup layoutGroup;
    public int ChildHeight;

    void Start()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        layoutGroup = this.gameObject.GetComponent<VerticalLayoutGroup>();
    }

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(0, ChildHeight * rectTransform.childCount + layoutGroup.spacing * rectTransform.childCount);

        UpdateChildrenHeight();
    }

    void UpdateChildrenHeight()
    {
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var child = rectTransform.GetChild(i);
            child.GetComponent<RectTransform>().sizeDelta = new Vector2(0, ChildHeight);
        }
    }
}
