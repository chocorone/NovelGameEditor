using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace DialogueDesigner
{
    public class ImageManager
    {
        Transform _charaTransform;
        NovelBackGround _backGround;
        DialogueImage _dialogueImage;
        List<NovelCharaImage> _charas = new();

        float _charaFadetime = 0.1f;

        public ImageManager(Transform charaTransform, NovelBackGround backGround, DialogueImage dialogogueImage, float charaFadeTime)
        {
            _charaTransform = charaTransform;
            _backGround = backGround;
            _dialogueImage = dialogogueImage;
            _charaFadetime = charaFadeTime;
        }

        internal async UniTask<bool> SetNextImage(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            switch (data.howBack)
            {
                case BackChangeStyle.UnChange:
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, data.charaEffects, data.charaEffectStrength, token);
                    EffectManager.Instance.SetEffect(_dialogueImage.image, data.DialogueEffect, data.DialogueEffectStrength);
                    EffectManager.Instance.SetEffect(_backGround.image, data.backEffect, data.backEffectStrength);
                    break;
                case BackChangeStyle.Quick:
                    _backGround.Change(data.back);
                    EffectManager.Instance.SetEffect(_backGround.image, data.backEffect, data.backEffectStrength);
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, data.charaEffects, data.charaEffectStrength, token);
                    EffectManager.Instance.SetEffect(_dialogueImage.image, data.DialogueEffect, data.DialogueEffectStrength);
                    break;
                case BackChangeStyle.dissolve:
                    await _backGround.Dissolve(data.backFadeSpeed, data.back, data.backEffect, data.backEffectStrength, token);
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, data.charaEffects, data.charaEffectStrength, token);
                    EffectManager.Instance.SetEffect(_dialogueImage.image, data.DialogueEffect, data.DialogueEffectStrength);
                    break;
                case BackChangeStyle.FadeBack:
                    await _backGround.BackFadeIn(data, token);
                    EffectManager.Instance.SetEffect(_backGround.image, data.backEffect, data.backEffectStrength);
                    await _backGround.BackFadeOut(data, token);
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, data.charaEffects, data.charaEffectStrength, token);
                    EffectManager.Instance.SetEffect(_dialogueImage.image, data.DialogueEffect, data.DialogueEffectStrength);
                    break;
                case BackChangeStyle.FadeFront:
                case BackChangeStyle.FadeAll:
                    await _backGround.BackFadeIn(data, token);
                    ChangeAllCharaQuick(data.howCharas, data.charas, data.charaEffects, data.charaEffectStrength);
                    EffectManager.Instance.SetEffect(_dialogueImage.image, data.DialogueEffect, data.DialogueEffectStrength);
                    EffectManager.Instance.SetEffect(_backGround.image, data.backEffect, data.backEffectStrength);
                    await _backGround.BackFadeOut(data, token);
                    break;
            }

            return true;
        }

        async UniTask<bool> SetChara(CharaChangeStyle[] style, Sprite[] sprites, Color[] color, Effect[] charaEffects, int[] strength, CancellationToken token)
        {
            List<UniTask<bool>> tasks = new();
            for (int i = 0; i < _charas.Count; i++)
            {
                if (style[i] == CharaChangeStyle.dissolve)
                {
                    tasks.Add(_charas[i].DissolveIn(sprites[i], color[i], _charaFadetime, token));
                }
            }
            await UniTask.WhenAll(tasks);

            tasks.Clear();
            for (int i = 0; i < _charas.Count; i++)
            {
                switch (style[i])
                {
                    case CharaChangeStyle.Quick:
                        _charas[i].Change(sprites[i]);
                        EffectManager.Instance.SetEffect(_charas[i].image, charaEffects[i], strength[i]);
                        break;
                    case CharaChangeStyle.dissolve:
                        EffectManager.Instance.SetEffect(_charas[i].image, charaEffects[i], strength[i]);
                        tasks.Add(_charas[i].DissolveOut(sprites[i], color[i], _charaFadetime, token));
                        break;
                }
            }
            await UniTask.WhenAll(tasks);
            return true;
        }

        void ChangeAllCharaQuick(CharaChangeStyle[] style, Sprite[] sprites, Effect[] charaEffects, int[] strength)
        {
            for (int i = 0; i < _charas.Count; i++)
            {
                if (style[i] != CharaChangeStyle.UnChange)
                {
                    _charas[i].Change(sprites[i]);
                    if (sprites[i] != null)
                        _charas[i].image.color = _charas[i]._defaultColor;
                }
                EffectManager.Instance.SetEffect(_charas[i].image, charaEffects[i], strength[i]);
            }
        }

        internal void Init(List<Image> data)
        {
            _charas.Clear();
            _charaTransform.DetachChildren();
            foreach (var image in data)
            {
                var obj = GameObject.Instantiate(image, _charaTransform);
                var charaImage = obj.gameObject.AddComponent<NovelCharaImage>();
                charaImage.Change(null);
                _charas.Add(charaImage);
            }
        }

    }

}
