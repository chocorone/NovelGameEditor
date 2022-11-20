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
    public Color _defaultColor;
    private float _defaultAlpha;

    public void Change(Sprite next)
    {
        if (next == null)
        {
            HideImage();
        }
        else
        {
            _image.sprite = next;
            DisplayImage();
        }

    }

    void Awake()
    {
        Init();
    }

    protected void Init()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public async UniTask<bool> FadeIn(float fadeTime)
    {
        await FadeIn(_defaultColor, fadeTime);
        return true;
    }

    public async UniTask<bool> FadeIn(Color color, float fadeTime)
    {
        float alpha = 0;
        color = new Color(color.r, color.g, color.b, 1);
        Color beforeColor = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);
        while (alpha < 1)
        {
            _image.color = Color.Lerp(beforeColor, color, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime * 0.01f));
            alpha += 0.01f;
        }

        _image.color = new Color(color.r, color.g, color.b, 1);
        return true;
    }

    public async UniTask<bool> FadeOut(float fadeTime)
    {
        await FadeOut(_defaultColor, fadeTime);
        return true;
    }

    public async UniTask<bool> FadeOut(Color color, float fadeTime)
    {
        float alpha = 1;
        color = new Color(color.r, color.g, color.b, 0);
        while (alpha > 0)
        {
            _image.color = Color.Lerp(color, _defaultColor, alpha);
            await UniTask.Delay(TimeSpan.FromSeconds(fadeTime * 0.01f));
            alpha -= 0.01f;
        }

        _image.color = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0);
        return true;
    }

    public void HideImage()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0);
    }

    public void DisplayImage()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
    }


}
