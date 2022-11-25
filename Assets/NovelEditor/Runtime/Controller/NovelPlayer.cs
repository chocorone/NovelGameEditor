using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace NovelEditor
{
    [RequireComponent(typeof(NovelUIManager))]
    public class NovelPlayer : MonoBehaviour
    {
        # region variable
        [SerializeField] private NovelData _noveldata;
        [SerializeField] private ChoiceButton _choiceButton;

        [SerializeField] private bool _playOnAwake = true;
        [SerializeField] private bool _hideAfterPlay = false;
        [SerializeField] private float _hideFadeTime = 1;
        [SerializeField] private bool _isDisplay = true;

        [SerializeField] private HowInput _inputSystem;
        [SerializeField] private KeyCode[] _nextButton;
        [SerializeField] private KeyCode[] _skipButton;
        [SerializeField] private KeyCode[] _hideOrDisplayButton;
        [SerializeField] private KeyCode[] _stopOrStartButton;

        public float charaFadeTime = 0.2f;
        public float textSpeed = 2.0f;

        [SerializeField, Range(0, 1)] public float BGMVolume = 1;
        [SerializeField, Range(0, 1)] public float SEVolume = 1;

        private NovelInputProvider _inputProvider;
        private NovelUIManager _novelUI;
        private AudioPlayer _audioPlayer;
        private ChoiceManager _choiceManager;

        private NovelData.ParagraphData _nowParagraph;
        private int _nowDialogueNum = 0;

        private bool _isReading = false;
        private bool _isImageChangeing = false;
        private bool _isStop = false;
        private bool _isPlaying = false;
        private bool _isChoicing = false;

        private List<string> _choiceName = new();
        private List<string> _ParagraphName = new();

        private CancellationTokenSource _textCTS = new CancellationTokenSource();
        private CancellationTokenSource _imageCTS = new CancellationTokenSource();
        private CancellationTokenSource _endFadeCTS = new CancellationTokenSource();

        #endregion

        #region property
        public bool IsStop => _isStop;
        public bool IsPlaying => _isPlaying;
        public bool IsChoicing => _isChoicing;
        public bool IsDisplay => _isDisplay;
        public bool mute = false;

        #endregion


        #region publicMethod

        public void Play(NovelData data, bool hideAfterPlay, int paragraphNum = 0, int dialogueIndex = 0, int paragraphID = 0)
        {
            _noveldata = data;
            _hideAfterPlay = hideAfterPlay;

            Reset();

            _nowDialogueNum = dialogueIndex;
            _nowParagraph = _noveldata.paragraphList[paragraphID];
            SetNext();

            UnPause();
            _isPlaying = true;
        }

        public void Pause()
        {
            _novelUI.SetStopText(true);
        }

        public void UnPause()
        {
            _novelUI.SetStopText(false);
        }

        public void HideUI()
        {

        }

        public void DisplayUI()
        {

        }

        public void Skip()
        {

        }

        public void SkipNextNode()
        {

        }

        public int GetNowParagraphID()
        {
            return _nowParagraph.index;
        }

        public int GetNowDialogueIndex()
        {
            return _nowDialogueNum;
        }

        public void SetInputProvider(NovelInputProvider input)
        {
            _inputProvider = input;
        }

        #endregion


        #region privateMethod
        void Awake()
        {
            switch (_inputSystem)
            {
                case HowInput.UserSetting:
                    _inputProvider = new CustomInputProvider(_nextButton, _skipButton, _hideOrDisplayButton, _stopOrStartButton);
                    break;
                default:
                    _inputProvider = new DefaultInputProvider();
                    break;
            }

            _novelUI = GetComponent<NovelUIManager>();
            _novelUI.Init(charaFadeTime);
            _audioPlayer = gameObject.AddComponent<AudioPlayer>();
            _audioPlayer.Init(BGMVolume, SEVolume);
            _choiceManager = GetComponentInChildren<ChoiceManager>();
            _choiceManager.Init(_choiceButton);

            if (_playOnAwake && _noveldata != null)
            {
                Play(_noveldata, _hideAfterPlay);
            }
        }

        public void SetDisplay(bool isDisplay)
        {

            if (isDisplay)
            {
                UnPause();

                _novelUI.SetDisplay(isDisplay);
                _isDisplay = true;
            }
            else
            {
                _endFadeCTS.Cancel();
                _endFadeCTS.Dispose();
                Pause();
                _novelUI.SetDisplay(isDisplay);
                _isDisplay = false;
            }
        }

        //現在再生しているものをリセット
        void Reset()
        {
            _novelUI.Reset(_noveldata.locations);
            _isChoicing = false;
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
                _novelUI.SwitchStopText();
            }
        }

        void SetNextParagraph(int nextIndex)
        {
            if (nextIndex == -1)
            {
                end();
            }
            else
            {
                _nowParagraph = _noveldata.paragraphList[nextIndex];
                _nowDialogueNum = 0;
                SetNextDialogue();
            }
        }

        void SetNext()
        {
            if (_nowDialogueNum >= _nowParagraph.dialogueList.Count)
            {
                switch (_nowParagraph.next)
                {
                    case Next.Choice:
                        Debug.Log("choice");
                        SetChoice();
                        break;
                    case Next.Continue:
                        SetNextParagraph(_nowParagraph.nextParagraphIndex);
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
            _isChoicing = true;
            List<NovelData.ChoiceData> list = new();
            foreach (int i in _nowParagraph.nextChoiceIndexes)
            {
                if (i == -1)
                    continue;
                list.Add(_noveldata.choiceList[i]);
            }
            if (list.Count == 0)
            {
                end();
                return;
            }

            var ans = await _choiceManager.WaitChoice(list);
            _isChoicing = false;
            SetNextParagraph(ans.nextParagraphIndex);
        }

        async void SetNextDialogue()
        {
            _isImageChangeing = true;
            _imageCTS.Dispose();
            _imageCTS = new CancellationTokenSource();
            _isImageChangeing = !await _novelUI.SetNextImage(_nowParagraph.dialogueList[_nowDialogueNum], _imageCTS.Token);
            _audioPlayer.SetSound(_nowParagraph.dialogueList[_nowDialogueNum]);
            _textCTS.Dispose();
            _textCTS = new CancellationTokenSource();
            _isReading = true;
            _isReading = !await _novelUI.SetNextText(_nowParagraph.dialogueList[_nowDialogueNum], _textCTS.Token);
            _nowDialogueNum++;
        }

        async void end()
        {
            _isPlaying = false;
            if (_hideAfterPlay)
            {
                _endFadeCTS = new CancellationTokenSource();
                _audioPlayer.AllStop();
                await _novelUI.FadeOut(_hideFadeTime, _endFadeCTS.Token);
                SetDisplay(false);
            }
        }

        void FlashText()
        {
            _textCTS.Cancel();
        }

        void OnValidate()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.SetSEVolume(SEVolume);
                _audioPlayer.SetBGMVolume(BGMVolume);
            }
        }

        void OnDisable()
        {

        }

        void OnDestroy()
        {

        }
        #endregion

    }


}

