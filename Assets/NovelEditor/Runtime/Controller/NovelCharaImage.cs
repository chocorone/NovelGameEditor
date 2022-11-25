using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;


namespace NovelEditor
{
    internal class NovelCharaImage : NovelImage
    {
        void Awake()
        {
            Init();
        }

        internal async UniTask<bool> DissolveIn(Sprite sprite, Color color, float fadeTime, CancellationToken token)
        {
            if (_image.sprite == null)
            {
                Color from = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);
                Change(sprite);
                await Fade(from, from, fadeTime / 2, token);
            }
            else
            {
                Color dest = new Color(color.r, color.g, color.b, 0);
                await Fade(_image.color, dest, fadeTime / 2, token);
                Change(sprite);
            }

            return true;
        }

        internal async UniTask<bool> DissolveOut(Sprite sprite, Color color, float fadeTime, CancellationToken token)
        {
            if (_image.sprite == null)
            {
                Color from = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);
                await Fade(from, from, fadeTime / 2, token);
            }
            else
            {
                Color dest = new Color(color.r, color.g, color.b, 0);
                await Fade(dest, _defaultColor, fadeTime / 2, token);
            }

            return true;
        }
    }
}