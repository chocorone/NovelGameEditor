using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NovelData.ParagraphData;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Text.RegularExpressions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueText : MonoBehaviour
{
    TextMeshProUGUI tmpro;
    int textSpeed = 6;
    public bool IsStop = false;

    void Awake()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
    }
    public async UniTask<bool> textUpdate(Dialogue data, CancellationToken token)
    {
        UpdateFont(data);
        //再生が終わったら通知したい
        return await PlayText(data.text, token);
    }

    internal void UpdateFont(Dialogue data)
    {
        if (data.changeFont)
        {
            tmpro.color = data.fontColor;
            tmpro.fontSize = data.fontSize;

            if (data.font != null)
                tmpro.font = data.font;
        }
    }

    private async UniTask<bool> PlayText(string text, CancellationToken token)
    {
        tmpro.text = "";

        List<string> words = SplitText(text);

        int wordCnt = 0;
        try
        {
            while (wordCnt < words.Count)
            {
                await UniTask.Delay(textSpeed * 10, cancellationToken: token);

                tmpro.text += words[wordCnt];
                await UniTask.WaitUntil(() => !IsStop);
                wordCnt++;
            }
        }
        catch (OperationCanceledException)
        {
            tmpro.text += String.Join("", words.GetRange(wordCnt, words.Count - wordCnt));
        }


        return true;
    }

    List<string> SplitText(string text)
    {
        List<string> words = new List<string>();

        foreach (string str in text.Split('<'))
        {
            string[] split = str.Split('>');

            int i = 0;
            if (split.Length == 2)
            {
                words.Add('<' + split[0] + '>');
                i = 1;
            }
            split[i] = split[i].Replace("&lt;", "<");
            split[i] = split[i].Replace("&gt;", ">");
            words.AddRange(split[i].Select(c => c.ToString()));
        }
        return words;
    }
}
