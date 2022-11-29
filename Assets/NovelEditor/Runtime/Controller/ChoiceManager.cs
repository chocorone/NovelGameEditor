using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace NovelEditor
{
    internal class ChoiceManager : MonoBehaviour
    {
        private ChoiceButton _button;
        internal void Init(ChoiceButton button)
        {
            _button = button;
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        internal async UniTask<NovelData.ChoiceData> WaitChoice(List<NovelData.ChoiceData> datas, CancellationToken token)
        {
            List<UniTask<NovelData.ChoiceData>> wait = new();

            foreach (NovelData.ChoiceData data in datas)
            {
                ChoiceButton button = Instantiate(_button, transform);
                button.transform.SetParent(transform);
                wait.Add(button.SetChoice(data, token));
            }

            var sendData = await UniTask.WhenAny(wait);
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            return sendData.result;


        }

        internal void ResetChoice()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}