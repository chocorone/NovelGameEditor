using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace NovelEditorPlugin
{
    public class ImageManager
    {
        Transform _charaTransform;
        NovelBackGround _backGround;
        DialogueImage _dialogueImage;
        List<NovelCharaImage> _charas = new();

        EffectManager _effectManager;

        float _charaFadetime = 0.1f;

        public ImageManager(Transform charaTransform, NovelBackGround backGround, DialogueImage dialogogueImage, float charaFadeTime)
        {
            _charaTransform = charaTransform;
            _backGround = backGround;
            _dialogueImage = dialogogueImage;

            _effectManager = new EffectManager();
            _charaFadetime = charaFadeTime;
        }

        internal async UniTask<bool> SetNextImage(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            switch (data.howBack)
            {
                case BackChangeStyle.UnChange:
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, token);
                    break;
                case BackChangeStyle.Quick:
                    _backGround.Change(data.back);
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, token);
                    break;
                case BackChangeStyle.dissolve:
                    await _backGround.Dissolve(data.backFadeSpeed, data.back, token);
                    await SetChara(data.howCharas, data.charas, data.charaFadeColor, token);
                    break;
                case BackChangeStyle.FadeBack:
                case BackChangeStyle.FadeFront:
                case BackChangeStyle.FadeAll:
                    await _backGround.BackFadeIn(data, token);
                    ChangeAllCharaQuick(data.howCharas, data.charas);
                    await _backGround.BackFadeOut(data, token);
                    break;

            }

            return true;
        }

        async UniTask<bool> SetChara(CharaChangeStyle[] style, Sprite[] sprites, Color[] color, CancellationToken token)
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
                        break;
                    case CharaChangeStyle.dissolve:
                        tasks.Add(_charas[i].DissolveOut(sprites[i], color[i], _charaFadetime, token));
                        break;
                }
            }
            await UniTask.WhenAll(tasks);
            return true;
        }

        void ChangeAllCharaQuick(CharaChangeStyle[] style, Sprite[] sprites)
        {
            for (int i = 0; i < _charas.Count; i++)
            {
                if (style[i] != CharaChangeStyle.UnChange)
                {
                    _charas[i].Change(sprites[i]);
                    _charas[i].image.color = _charas[i]._defaultColor;
                }

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
                //effectManager.SetCharaMaterial(dialogueImage.locations);
            }
            //effectManager.SetCharaMaterial(dialogueImage.locations);
        }
    }

}
