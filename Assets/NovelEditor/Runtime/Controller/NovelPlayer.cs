using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace NovelEditorPlugin
{
    [RequireComponent(typeof(NovelUIManager))]
    public class NovelPlayer : MonoBehaviour
    {
        [SerializeField] private NovelData _noveldata;

        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _hideAfterPlay = false;
        [SerializeField] private float _hideFadeTime = 1;
        [SerializeField] private bool _isDisplay = true;

        [SerializeField] private HowInput _inputSystem;
        [SerializeField] private KeyCode[] _nextButton;
        [SerializeField] private KeyCode[] _skipButton;
        [SerializeField] private KeyCode[] _hideOrDisplayButton;
        [SerializeField] private KeyCode[] _stopOrStartButton;

        [SerializeField] private float _charaFadeTime = 0.2f;

        [SerializeField, Range(0, 1)] private float _BGMVolume = 1;
        [SerializeField, Range(0, 1)] private float _SEVolume = 1;



        NovelInputProvider _inputProvider;

        public bool IsStop { get; private set; } = false;
        public bool IsPlaying { get; private set; } = false;
        public bool IsChoicing { get; private set; } = false;
        public bool IsDisplay => _isDisplay;

        public int nowDialogueNum { get; private set; } = 0;

        private NovelUIManager novelUI;
        private AudioPlayer audioPlayer;
        private NovelData.ParagraphData _nowParagraph;
        private bool _isReading = false;
        private bool _isImageChangeing = false;

        CancellationTokenSource textCTS = new CancellationTokenSource();
        CancellationTokenSource imageCTS = new CancellationTokenSource();
        CancellationTokenSource endFadeCTS = new CancellationTokenSource();

        void Awake()
        {
            switch (_inputSystem)
            {
                case HowInput.Default:
                    _inputProvider = new DefaultInputProvider();
                    break;
                case HowInput.UserSetting:
                    _inputProvider = new CustomInputProvider(_nextButton, _skipButton, _hideOrDisplayButton, _stopOrStartButton);
                    break;
            }

            novelUI = GetComponent<NovelUIManager>();
            novelUI.Init(_charaFadeTime);
            audioPlayer = gameObject.AddComponent<AudioPlayer>();
            audioPlayer.Init(_BGMVolume, _SEVolume);

            if (_playOnAwake && _noveldata != null)
            {
                Play(_noveldata, _hideAfterPlay);
            }
        }

        public void Play(NovelData data, bool hideAfterPlay, int paragraphNum = 0, int dialogueNum = 0)
        {
            _noveldata = data;
            _hideAfterPlay = hideAfterPlay;

            Reset();

            nowDialogueNum = 0;
            _nowParagraph = _noveldata.paragraphList[nowDialogueNum];
            SetNext();

            SetStop(false);
            IsPlaying = true;
        }

        public void SetStop(bool isStop)
        {
            novelUI.SetStopText(isStop);
        }

        public void SetDisplay(bool isDisplay)
        {
            endFadeCTS.Cancel();
            endFadeCTS.Dispose();
            if (isDisplay)
            {
                SetStop(false);
                novelUI.SetDisplay(isDisplay);
                _isDisplay = true;
            }
            else
            {
                SetStop(true);
                novelUI.SetDisplay(isDisplay);
                _isDisplay = false;
            }
        }

        //現在再生しているものをリセット
        void Reset()
        {
            novelUI.Reset(_noveldata.locations);
            IsChoicing = false;
        }

        void Update()
        {
            if (!IsPlaying || IsChoicing || _isImageChangeing)
            {
                return;
            }
            if (_inputProvider.GetNext())
            {
                //全部表示
                if (_isReading)
                {
                    FlashText();
                }
                else
                {
                    SetNext();
                }

            }
            if (_inputProvider.GetSkip())
            {
                Debug.Log("GetSkip");
            }

            if (_inputProvider.GetStopOrStart())
            {
                novelUI.SwitchStopText();
            }
        }

        void SetNextParagraph()
        {
            if (_nowParagraph.nextParagraphIndex == -1)
            {
                end();
            }
            else
            {
                _nowParagraph = _noveldata.paragraphList[_nowParagraph.nextParagraphIndex];
                nowDialogueNum = 0;
                SetNextDialogue();
            }
        }

        void SetNext()
        {
            if (nowDialogueNum >= _nowParagraph.dialogueList.Count)
            {
                switch (_nowParagraph.next)
                {
                    case Next.Choice:
                        SetChoice();
                        break;
                    case Next.Continue:
                        SetNextParagraph();
                        break;
                    case Next.End:
                        end();
                        break;
                }
            }
            else
            {
                SetNextDialogue();
            }

        }

        async void SetChoice()
        {
            IsChoicing = true;
        }

        async void SetNextDialogue()
        {
            _isImageChangeing = true;
            imageCTS.Dispose();
            imageCTS = new CancellationTokenSource();
            _isImageChangeing = !await novelUI.SetNextImage(_nowParagraph.dialogueList[nowDialogueNum], imageCTS.Token);
            audioPlayer.SetSound(_nowParagraph.dialogueList[nowDialogueNum]);
            textCTS.Dispose();
            textCTS = new CancellationTokenSource();
            _isReading = true;
            _isReading = !await novelUI.SetNextText(_nowParagraph.dialogueList[nowDialogueNum], textCTS.Token);
            nowDialogueNum++;
        }

        async void end()
        {
            IsPlaying = false;
            if (_hideAfterPlay)
            {
                endFadeCTS = new CancellationTokenSource();
                audioPlayer.AllStop();
                await novelUI.FadeOut(_hideFadeTime, endFadeCTS.Token);
                SetDisplay(false);
            }
        }

        void FlashText()
        {
            textCTS.Cancel();
        }

        void OnValidate()
        {
            if (audioPlayer != null)
            {
                audioPlayer.SetSEVolume(_SEVolume);
                audioPlayer.SetBGMVolume(_BGMVolume);
            }
        }

    }
}

