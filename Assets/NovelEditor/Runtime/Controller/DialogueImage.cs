using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace NovelEditorPlugin
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueImage : NovelImage
    {
        private CanvasGroup dialogueCanvas;
        // Start is called before the first frame update
        void Awake()
        {
            Init();
            dialogueCanvas = GetComponent<CanvasGroup>();
        }

        public async UniTask<bool> DissolveIn(float time, CancellationToken token)
        {
            float alpha = 0;

            float alphaSpeed = 0.01f;
            if (time < 0.5)
            {
                alphaSpeed = 0.1f;
            }
            try
            {
                while (alpha < 1)
                {
                    dialogueCanvas.alpha = alpha;
                    await UniTask.Delay(TimeSpan.FromSeconds(time * alphaSpeed), cancellationToken: token);
                    alpha += alphaSpeed;
                }
            }
            catch (OperationCanceledException)
            { }

            return true;
        }

        public async UniTask<bool> DissolveOut(float time, CancellationToken token)
        {
            float alpha = 1;

            float alphaSpeed = 0.01f;
            if (time < 0.5)
            {
                alphaSpeed = 0.1f;
            }
            try
            {
                while (alpha > 0)
                {
                    dialogueCanvas.alpha = alpha;
                    await UniTask.Delay(TimeSpan.FromSeconds(time * alphaSpeed), cancellationToken: token);
                    alpha -= alphaSpeed;
                }
            }
            catch (OperationCanceledException)
            { }

            return false;
        }
    }
}

