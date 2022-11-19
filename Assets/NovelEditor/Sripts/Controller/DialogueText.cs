using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NovelData.ParagraphData;
using Cysharp.Threading.Tasks;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueText : MonoBehaviour
{
    TextMeshProUGUI tmpro;
    int textSpeed = 6;

    void Awake()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
    }
    public void textUpdate(Dialogue data)
    {
        //再生が終わったら通知したい
        PlayText(data.text);
    }

    private async UniTask<bool> PlayText(string text)
    {
        tmpro.text = "";

        int wordcnt = 0;
        while (wordcnt < text.Length)
        {
            await UniTask.Delay(textSpeed * 10);

            tmpro.text += text[wordcnt];

            // if (IsStop)
            // {
            //     await UniTask.WaitUntil(() => !IsStop);
            // }
            wordcnt++;
        }

        return true;
    }
}
