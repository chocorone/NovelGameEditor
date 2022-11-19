using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NovelData.ParagraphData;

[RequireComponent(typeof(CanvasGroup))]
internal class NovelUIManager : MonoBehaviour
{
    CanvasGroup NovelCanvas;
    ImageManager imagemanager;
    [SerializeField] DialogueText dialogueText;

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

    internal void SetNextDialogue(Dialogue data)
    {
        dialogueText.textUpdate(data);
        UpdateNameText(data);
    }

    internal void UpdateNameText(Dialogue data)
    {
        //名前のフォントなど変更
    }
}