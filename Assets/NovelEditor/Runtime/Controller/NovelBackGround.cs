using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace NovelEditorPlugin
{
    public class NovelBackGround : NovelImage
    {
        private NovelImage _backFade;
        private NovelImage _frontFade;
        private NovelImage _allFade;

        void Awake()
        {
            Init();
            RectTransform backTransform = GetComponent<RectTransform>();

            RectTransform backObj = new GameObject("backFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
            backObj.transform.SetParent(this.transform);
            CopyRectTransformSize(backTransform, backObj);
            _backFade = backObj.gameObject.AddComponent<NovelImage>();
            _backFade.HideImage();

            RectTransform frontObj = new GameObject("frontFadePanel", typeof(RectTransform)).GetComponent<RectTransform>();
            frontObj.transform.SetParent(this.transform.parent);
            frontObj.transform.SetSiblingIndex(2);
            CopyRectTransformSize(backTransform, frontObj);
            _frontFade = frontObj.gameObject.AddComponent<NovelImage>();
            _frontFade.HideImage();

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

        public async UniTask<bool> BackFadeIn(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            switch (data.howBack)
            {
                case BackChangeStyle.FadeBack:
                    await FadeIn(_backFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;

                case BackChangeStyle.FadeFront:
                    await FadeIn(_frontFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;

                case BackChangeStyle.FadeAll:
                    await FadeIn(_allFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;
            }
            Change(data.back);
            return true;
        }

        public async UniTask<bool> BackFadeOut(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            switch (data.howBack)
            {
                case BackChangeStyle.FadeBack:
                    await FadeOut(_backFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;

                case BackChangeStyle.FadeFront:
                    await FadeOut(_frontFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;

                case BackChangeStyle.FadeAll:
                    await FadeOut(_allFade, data.backFadeColor, data.backFadeSpeed, token);
                    break;
            }
            return true;
        }

        async UniTask<bool> FadeIn(NovelImage Panel, Color dest, float speed, CancellationToken token)
        {
            Panel.image.sprite = null;
            Color from = new Color(dest.r, dest.g, dest.b, 0);
            await Panel.Fade(from, dest, speed / 2, token);
            return true;
        }

        async UniTask<bool> FadeOut(NovelImage Panel, Color from, float speed, CancellationToken token)
        {
            Color dest = new Color(from.r, from.g, from.b, 0);
            await Panel.Fade(from, dest, speed / 2, token);
            return true;
        }

        public async UniTask<bool> Dissolve(float speed, Sprite sprite, CancellationToken token)
        {
            if (_image.sprite == null)
            {
                HideImage();
                Change(sprite);
                Color from = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);
                await Fade(from, _defaultColor, speed, token);
            }
            else
            {
                _backFade.Change(_image.sprite);
                _backFade.image.color = _image.color;
                Change(sprite);
                Color dest = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
                await _backFade.Fade(_image.color, dest, speed, token);
                _backFade.HideImage();
            }
            return true;
        }
    }

}
