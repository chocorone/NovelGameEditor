using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(Image))]
public class NovelImage : MonoBehaviour
{
    protected Image _image;
    private Color _defaultColor;

    public void Change(Sprite next)
    {
        _image.sprite = next;
    }

    void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    async UniTask<bool> FadeIn(float fadeTime)
    {
        await FadeIn(_image.color, fadeTime);
        return true;
    }

    async UniTask<bool> FadeIn(Color color, float fadeTime)
    {
        float alpha = 0;

        while (alpha < 1)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
            _image.color = Color.Lerp(_defaultColor, color, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime * 0.01f));
            alpha += 0.01f;
        }

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
        return true;
    }

    async UniTask<bool> FadeOut(float fadeTime)
    {
        await FadeOut(_image.color, fadeTime);
        return true;
    }

    async UniTask<bool> FadeOut(Color color, float fadeTime)
    {
        float alpha = 1;

        while (alpha > 0)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, alpha);
            _image.color = Color.Lerp(_defaultColor, color, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime * 0.01f));
            alpha -= 0.01f;
        }

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
        return true;
    }

}
