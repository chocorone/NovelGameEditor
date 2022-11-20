using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NovelData.ParagraphData;

public class NovelBackGround : NovelImage
{
    private NovelImage _frontImage;
    private NovelImage _allImage;
    void Awake()
    {
        Init();
        RectTransform backTransform = GetComponent<RectTransform>();

        RectTransform frontObj = new GameObject("frontFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        frontObj.transform.SetParent(this.transform);
        CopyRectTransformSize(backTransform, frontObj);
        _frontImage = frontObj.gameObject.AddComponent<NovelImage>();
        _frontImage.HideImage();

        RectTransform allObj = new GameObject("allFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        allObj.transform.SetParent(this.transform.parent);
        CopyRectTransformSize(backTransform, allObj);
        _allImage = allObj.gameObject.AddComponent<NovelImage>();
        _allImage.HideImage();
    }

    void CopyRectTransformSize(RectTransform source, RectTransform dest)
    {
        dest.anchorMin = source.anchorMin;
        dest.anchorMax = source.anchorMax;
        dest.anchoredPosition = source.anchoredPosition;
        dest.sizeDelta = source.sizeDelta;
    }

    public void ChangeBack(Dialogue data)
    {

    }
}
