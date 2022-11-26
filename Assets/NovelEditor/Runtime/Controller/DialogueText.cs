using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Text.RegularExpressions;


namespace NovelEditor
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal class DialogueText : MonoBehaviour
    {
        TextMeshProUGUI tmpro;
        internal int textSpeed = 6;
        public bool IsStop = false;
        string nowText;

        TMP_FontAsset defaultFont;
        float defaultFontSize;
        Color defaultFontColor;

        void Awake()
        {
            tmpro = GetComponent<TextMeshProUGUI>();
            defaultFont = tmpro.font;
            defaultFontSize = tmpro.fontSize;
            defaultFontColor = tmpro.color;
        }
        internal async UniTask<bool> textUpdate(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            UpdateFont(data);
            //再生が終わったら通知したい
            return await PlayText(data.text, token);
        }

        internal void SetDefaultFont()
        {
            tmpro.font = defaultFont;
            tmpro.fontSize = defaultFontSize;
            tmpro.color = defaultFontColor;
        }

        internal void DeleteText()
        {
            tmpro.text = "";
        }

        internal void UpdateFont(NovelData.ParagraphData.Dialogue data)
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
            nowText = text;

            List<string> words = SplitText(text);

            int wordCnt = 0;
            try
            {
                while (wordCnt < words.Count)
                {
                    await UniTask.Delay(250 / textSpeed, cancellationToken: token);

                    tmpro.text += words[wordCnt];
                    await UniTask.WaitUntil(() => !IsStop);
                    wordCnt++;
                }
            }
            catch (OperationCanceledException)
            { }


            return true;
        }

        internal void FlushText()
        {
            tmpro.text = nowText;
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

}
