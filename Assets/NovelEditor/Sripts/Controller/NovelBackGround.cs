using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NovelData.ParagraphData;
using Cysharp.Threading.Tasks;

public class NovelBackGround : NovelImage
{
    private NovelImage _backFade;
    private NovelImage _allFade;
    void Awake()
    {
        Init();
        RectTransform backTransform = GetComponent<RectTransform>();

        RectTransform frontObj = new GameObject("frontFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        frontObj.transform.SetParent(this.transform);
        CopyRectTransformSize(backTransform, frontObj);
        _backFade = frontObj.gameObject.AddComponent<NovelImage>();
        _backFade.HideImage();

        RectTransform allObj = new GameObject("allFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
        allObj.transform.SetParent(this.transform.parent);
        CopyRectTransformSize(backTransform, allObj);
        _allFade = allObj.gameObject.AddComponent<NovelImage>();
        _allFade.HideImage();
    }

    void CopyRectTransformSize(RectTransform source, RectTransform dest)
    {
        dest.anchorMin = source.anchorMin;
        dest.anchorMax = source.anchorMax;
        dest.anchoredPosition = source.anchoredPosition;
        dest.sizeDelta = source.sizeDelta;
    }

    public async UniTask<bool> ChangeBack(Dialogue data)
    {
        switch (data.howBack)
        {
            case BackChangeStyle.Quick:
                Change(data.back);
                break;
            case BackChangeStyle.FadeBack:
                _backFade._defaultColor = data.backFadeColor;
                await _backFade.FadeIn(data.backFadeColor, data.backFadeSpeed / 2);
                Change(data.back);
                await _backFade.FadeOut(data.backFadeColor, data.backFadeSpeed / 2);
                break;
            case BackChangeStyle.FadeFront:

                break;

            case BackChangeStyle.FadeAll:
                _allFade._defaultColor = data.backFadeColor;
                await _allFade.FadeIn(data.backFadeColor, data.backFadeSpeed / 2);
                Change(data.back);
                await _allFade.FadeOut(data.backFadeColor, data.backFadeSpeed / 2);
                break;
        }
        return true;
    }
}
