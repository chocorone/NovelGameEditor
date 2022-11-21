using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NovelData.ParagraphData;
using Cysharp.Threading.Tasks;
using System.Threading;

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

    public async UniTask<bool> ChangeBack(Dialogue data, CancellationToken token)
    {
        switch (data.howBack)
        {
            case BackChangeStyle.Quick:
                Change(data.back);
                break;

            case BackChangeStyle.FadeBack:
                _backFade.image.sprite = null;
                _backFade._defaultColor = data.backFadeColor;
                await _backFade.FadeIn(data.backFadeColor, data.backFadeSpeed / 2, token);
                Change(data.back);
                await _backFade.FadeOut(data.backFadeColor, data.backFadeSpeed / 2, token);
                break;
            case BackChangeStyle.FadeFront:

                break;

            case BackChangeStyle.FadeAll:
                _allFade.image.sprite = null;
                _allFade._defaultColor = data.backFadeColor;
                await _allFade.FadeIn(data.backFadeColor, data.backFadeSpeed / 2, token);
                Change(data.back);
                await _allFade.FadeOut(data.backFadeColor, data.backFadeSpeed / 2, token);
                break;

            case BackChangeStyle.dissolve:
                if (_image.sprite == null)
                {
                    HideImage();
                    Change(data.back);
                    _defaultColor = _image.color;
                    await FadeIn(data.backFadeColor, data.backFadeSpeed, token);
                }
                else
                {
                    _backFade.Change(_image.sprite);
                    _backFade.image.color = _image.color;
                    _backFade._defaultColor = _image.color;
                    Change(data.back);
                    await _backFade.FadeOut(_image.color, 3, token);
                    _backFade.HideImage();
                }

                break;
        }
        return true;
    }
}
