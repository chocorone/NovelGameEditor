using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
internal class NovelUIManager : MonoBehaviour
{
    CanvasGroup NovelCanvas;
    ImageManager imagemanager;
    void Init()
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

    internal void NewDataSetUp(List<Image> data)
    {
        imagemanager.Init(data);
        //dialogueField.ResetDialogue();
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
}
