using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class NovelImage : MonoBehaviour
{
    protected Image _image;
    private Color _defaultColor;

    public void Change(Sprite next)
    {

    }

    void Awake(){
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
        Debug.Log("parent");
    }

    // async UniTask<bool> FadeOut(Color color,float fadeTime){
    //     float alpha = 0;

    //     while (alpha<1)
    //     {
    //         _image.color.a = alpha;
    //         _image.color = Color.Lerp(_defaultColor,color,alpha);
    //         await UniTask.Delay(fadeTime*0.01);
    //         alpha += 0.01f;
    //     }

    //     _image.color.a = 1;
    //     return true;
    // }

    void FadeIn(){

    }

}
