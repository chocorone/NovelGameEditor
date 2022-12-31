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
    /// <summary>
    /// UIを管理するコンポーネント
    /// </summary>
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

        /// <summary>
        /// 初期化用関数
        /// </summary>
        /// <param name="charaFadeTime">キャラがディゾルブで切り替わるのにかかる時間</param>
        /// <param name="nonameDialogueSprite">名前なし用のセリフ枠のSprite</param>
        /// <param name="dialogueSprite">通常のセリフ枠のSprite</param>
        internal void Init(float charaFadeTime, Sprite nonameDialogueSprite, Sprite dialogueSprite)
        {
            NovelCanvas = GetComponent<CanvasGroup>();
            imageManager = new ImageManager(_charaTransform, _backGround, _dialogueImage, charaFadeTime);
            _dialogueImage.SetDialogueSprite(dialogueSprite, nonameDialogueSprite);
        }

        /// <summary>
        /// 立ち絵の情報を新しいデータに合わせてリセットする
        /// </summary>
        /// <param name="data">新しいデータの立ち絵の位置</param>
        /// <param name="isLoad">ロード後かどうか</param>
        internal void Reset(List<Image> data, bool isLoad)
        {
            imageManager.Init(data, isLoad);
            DeleteText();
            _dialogueText.SetDefaultFont();
        }

        /// <summary>
        /// 残りのセリフを一度に表示する
        /// </summary>
        internal void FlashText()
        {
            _dialogueText.FlushText();
        }

        /// <summary>
        /// テキストを初期化する
        /// </summary>
        internal void DeleteText()
        {
            //テキストを初期化
            _dialogueText.DeleteText();
            _nameText.text = "";
        }

        /// <summary>
        /// 立ち絵、背景を含む全てのUIを表示切り替え
        /// </summary>
        /// <param name="display">表示するかどうか</param>
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

        /// <summary>
        /// UIの表示の設定
        /// </summary>
        /// <param name="display">UIを表示するかどうか</param>
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
        }

        /// <summary>
        /// UIをフェードアウトさせる
        /// </summary>
        /// <param name="time">フェードアウトにかかる時間</param>
        /// <param name="token">使用するCancellationToken</param>
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

        /// <summary>
        /// 次のテキストを設定し、1文字ずつ再生する
        /// </summary>
        /// <param name="data">次のセリフのデータ</param>
        /// <param name="token">使用するCancellationToken</param>
        internal async UniTask<bool> SetNextText(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            UpdateNameText(data);
            return await _dialogueText.textUpdate(data, token);
        }

        /// <summary>
        /// 次のセリフの立ち絵、背景を設定する
        /// </summary>
        /// <param name="data">次のセリフのデータ</param>
        /// <param name="token">使用するCancellationToken</param>
        internal async UniTask<bool> SetNextImage(NovelData.ParagraphData.Dialogue data, CancellationToken token)
        {
            DeleteText();
            await imageManager.SetNextImage(data, data.Name != "", token);
            return true;
        }

        /// <summary>
        /// テキストの再生・停止を切り替える
        /// </summary>
        internal void SwitchStopText()
        {
            _dialogueText.IsStop = !_dialogueText.IsStop;
        }

        /// <summary>
        /// テキストの再生を一時停止する
        /// </summary>
        /// <param name="flag">停止するかどうか</param>
        internal void SetStopText(bool flag)
        {
            _dialogueText.IsStop = flag;
        }

        /// <summary>
        /// テキスト再生速度を設定する
        /// </summary>
        /// <param name="speed">新しい速度</param>
        internal void SetTextSpeed(int speed)
        {
            _dialogueText.textSpeed = speed;
        }

        /// <summary>
        /// 現在の背景を取得する
        /// </summary>
        internal Sprite GetNowBack()
        {
            return _backGround.image.sprite;
        }

        /// <summary>
        /// 名前のテキストを設定する
        /// </summary>
        /// <param name="data">次のセリフのデータ</param>
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
    }
}
