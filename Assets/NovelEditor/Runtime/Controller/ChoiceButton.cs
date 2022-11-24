using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;

namespace NovelEditorPlugin
{
    [RequireComponent(typeof(Button))]
    public class ChoiceButton : MonoBehaviour
    {
        bool _choiced = false;
        Button _button;

        internal async UniTask<NovelData.ChoiceData> SetChoice(NovelData.ChoiceData data, CancellationToken token)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Clicked);
            GetComponentInChildren<TextMeshProUGUI>().text = data.text;
            try
            {
                await UniTask.WaitUntil(() => _choiced, cancellationToken: token);
            }
            catch { }

            return data;
        }

        void Clicked()
        {
            _choiced = true;
        }
    }
}

