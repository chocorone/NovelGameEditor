using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using static NovelData.ParagraphData;
public class ImageManager
{
    Transform _charaTransform;
    NovelBackGround _backGround;
    DialogueImage _dialogueImage;
    List<NovelCharaImage> _charas;

    EffectManager _effectManager;

    public ImageManager(Transform charaTransform, NovelBackGround backGround, DialogueImage dialogogueImage)
    {
        _charaTransform = charaTransform;
        _backGround = backGround;
        _dialogueImage = dialogogueImage;

        _effectManager = new EffectManager();

    }

    internal async UniTask<bool> SetBackGround(Dialogue data, CancellationToken token)
    {
        await _backGround.ChangeBack(data, token);
        //effectManager.setEffect();
        return true;
    }

    void Init(List<Image> data)
    {
        //effectManager.SetCharaMaterial(dialogueImage.locations);
    }
}
