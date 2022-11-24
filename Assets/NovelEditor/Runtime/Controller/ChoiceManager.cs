using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace NovelEditorPlugin
{
    public class ChoiceManager : MonoBehaviour
    {
        private ChoiceButton _button;
        // Start is called before the first frame update
        internal void Init(ChoiceButton button)
        {
            _button = button;
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        internal async UniTask<NovelData.ChoiceData> WaitChoice(List<NovelData.ChoiceData> datas)
        {
            List<UniTask<NovelData.ChoiceData>> wait = new();
            CancellationTokenSource cancel = new();

            foreach (NovelData.ChoiceData data in datas)
            {
                ChoiceButton button = Instantiate(_button, transform);
                button.transform.SetParent(transform);
                wait.Add(button.SetChoice(data, cancel.Token));
            }

            var sendData = await UniTask.WhenAny(wait);

            cancel.Cancel();
            cancel.Dispose();
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            return sendData.result;
        }
    }
}