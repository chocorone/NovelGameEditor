using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

[RequireComponent(typeof(Image))]
public class NovelImage : MonoBehaviour
{
    protected Image _image;
    [HideInInspector] public Color _defaultColor;
    private float _defaultAlpha;

    public Image image => _image;

    public void Change(Sprite next)
    {
        if (next == null)
        {
            _image.sprite = null;
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


    public async UniTask<bool> Fade(Color from, Color dest, float fadeTime, CancellationToken token)
    {
        float alpha = 0;
        _image.color = from;
        try
        {
            while (alpha < 1)
            {
                _image.color = Color.Lerp(from, dest, alpha);
                await UniTask.Delay(TimeSpan.FromSeconds((double)fadeTime * 0.01), cancellationToken: token);
                alpha += 0.01f;
            }
        }
        catch (OperationCanceledException)
        {
            //return false;
        }


        _image.color = dest;
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
