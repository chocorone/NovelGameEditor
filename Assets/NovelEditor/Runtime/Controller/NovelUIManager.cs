using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using TMPro;

namespace NovelEditor
{

    [RequireComponent(typeof(CanvasGroup))]
    internal class NovelUIManager : MonoBehaviour
    {
        CanvasGroup NovelCanvas;
        ImageManager imageManager;

        [SerializeField] Transform _charaTransform;
        [SerializeField] NovelBackGround _backGround;
        [SerializeField] DialogueImage _dialogueImage;
        [SerializeField] DialogueText _dialogueText;
        [SerializeField] TextMeshProUGUI _nameText;
        [SerializeField] CanvasGroup UIparents;
        public bool canFlush => _dialogueText.canFlush;


        internal void Init(float charaFadeTime, Sprite nonameDialogueSprite, Sprite dialogueSprite)
        {
            NovelCanvas = GetComponent<CanvasGroup>();
            imageManager = new ImageManager(_charaTransform, _backGround, _dialogueImage, charaFadeTime);
            _dialogueImage.SetDialogueSprite(dialogueSprite, nonameDialogueSprite);
        }

        internal void Reset(List<Image> data, bool isLoad)
        {
            imageManager.Init(data, isLoad);
            DeleteText();
            _dialogueText.SetDefaultFont();
        }

        internal void FlashText()
        {
            _dialogueText.FlushText();
        }

        internal void DeleteText()
        {
            //テキストを初期化
            _dialogueText.DeleteText();
            _nameText.text = "";
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

        internal void SetUIDisplay(bool display)
        {
            if (display)
            {
                UIparents.alpha = 1;
                UIparents.interactable = true;
            }
            else
            {
                UIparents.alpha = 0;
                UIparents.interactable = false;
            }
            Debug.Log(UIparents.alpha);
        }

        internal async UniTask<bool> FadeOut(float time, CancellationToken token)
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
                    NovelCanvas.alpha = alpha;
                    await UniTask.Delay(TimeSpan.FromSeconds(time * alphaSpeed), cancellationToken: token);
                    alpha -= alphaSpeed;
                }
            }
            catch (OperationCanceledException)
            { }

            return true;
        }

        internal async UniTask<bool> SetNextText(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            UpdateNameText(data);
            return await _dialogueText.textUpdate(data, token);
        }

        internal async UniTask<bool> SetNextImage(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            DeleteText();
            await imageManager.SetNextImage(data, data.Name != "", token);
            return true;
        }

        void UpdateNameText(NovelData.ParagraphData.Dialogue data)
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

        internal void SwitchStopText()
        {
            _dialogueText.IsStop = !_dialogueText.IsStop;
        }

        internal void SetStopText(bool flag)
        {
            _dialogueText.IsStop = flag;
        }

        internal void SetTextSpeed(int speed)
        {
            _dialogueText.textSpeed = speed;
        }

        internal Sprite GetNowBack()
        {
            return _backGround.image.sprite;
        }
    }
}
