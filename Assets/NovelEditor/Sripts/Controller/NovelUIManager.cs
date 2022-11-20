using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using static NovelData.ParagraphData;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
internal class NovelUIManager : MonoBehaviour
{
    CanvasGroup NovelCanvas;
    ImageManager imagemanager;
    [SerializeField] NovelBackGround _backGround;
    [SerializeField] DialogueText _dialogueText;
    [SerializeField] TextMeshProUGUI _nameText;


    void Awake()
    {
        NovelCanvas = GetComponent<CanvasGroup>();
    }

    internal void SetStop()
    {

    }

    internal void SetStop(bool stop)
    {
        if (stop)
        {
            //dialogueField.IsStop = true;
            Debug.Log("すとっぷ");
        }
        else
        {
            //dialogueField.IsStop = false;
            Debug.Log("再開");
        }

    }

    internal void Reset(List<Image> data)
    {
        //キャラなどを非表示？

        //テキストを初期化

        //選択肢を全部消す

        //キャラのロケーションをリセットとか
        //imagemanager.Init(data);
    }

    internal void SetDisplay(bool display)
    {
        if (display)
        {
            NovelCanvas.alpha = 1;
            NovelCanvas.interactable = true;
        }
        else
        {
            NovelCanvas.alpha = 0;
            NovelCanvas.interactable = false;
        }
    }

    internal async UniTask<bool> SetNextText(Dialogue data, CancellationToken token)
    {
        UpdateNameText(data);
        return await _dialogueText.textUpdate(data, token);
    }

    internal async UniTask<bool> SetNextImage(Dialogue data, CancellationToken token)
    {
        await _backGround.ChangeBack(data);
        return true;
    }

    internal void UpdateNameText(Dialogue data)
    {
        //名前のフォントなど変更
        _nameText.text = data.Name;
        if (data.changeNameFont)
        {
            _nameText.color = data.nameColor;

            if (data.nameFont != null)
                _nameText.font = data.nameFont;
        }
    }

    internal void StopOrStartText()
    {
        _dialogueText.IsStop = !_dialogueText.IsStop;
    }
}
