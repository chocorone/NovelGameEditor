using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;

namespace NovelEditor
{
    [RequireComponent(typeof(NovelUIManager))]
    public class NovelPlayer : MonoBehaviour
    {
        # region variable
        [SerializeField, HideInInspector] private NovelData _novelData;
        [SerializeField, HideInInspector] private ChoiceButton _choiceButton;
        [SerializeField, HideInInspector] private Sprite _dialogueSprite;
        [SerializeField, HideInInspector] private Sprite _nonameDialogueSprite;
        [SerializeField, HideInInspector] private bool _playOnAwake = true;
        [SerializeField, HideInInspector] private bool _hideAfterPlay = false;
        [SerializeField, HideInInspector] private float _hideFadeTime = 0.5f;
        [SerializeField, HideInInspector] private bool _isDisplay = true;

        [SerializeField, HideInInspector] private float _charaFadeTime = 0.2f;
        [SerializeField, HideInInspector] private int _textSpeed = 6;

        [SerializeField, HideInInspector] private float _BGMVolume = 1;
        [SerializeField, HideInInspector] private float _SEVolume = 1;

        [SerializeField, HideInInspector] private HowInput _inputSystem;
        [SerializeField] private KeyCode[] _nextButton;
        [SerializeField] private KeyCode[] _skipButton;
        [SerializeField] private KeyCode[] _hideOrDisplayButton;
        [SerializeField] private KeyCode[] _stopOrStartButton;

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
        private bool _isUIDisplay = true;
        private bool _mute = false;
        private bool _isEnd = false;

        private List<string> _choiceName = new();
        private List<string> _ParagraphName = new();
        private List<int> _passedParagraphID = new();

        private CancellationTokenSource _textCTS = new CancellationTokenSource();
        private CancellationTokenSource _imageCTS = new CancellationTokenSource();
        private CancellationTokenSource _endFadeCTS = new CancellationTokenSource();

        #endregion

        #region property
        public NovelData novelData => _novelData;
        public bool IsStop => _isStop;
        public bool IsPlaying => _isPlaying;
        public bool IsChoicing => _isChoicing;
        public bool IsUIDisplay
        {
            get
            {
                return _isUIDisplay;
            }

            set
            {
                if (value)
                {
                    UnPause();
                    DisplayUI();
                }
                else
                {
                    Pause();
                    HideUI();
                }

                _isDisplay = value;
            }
        }
        public bool IsDisplay
        {
            get
            {
                return _isDisplay;
            }

            set
            {
                if (value)
                {
                    UnPause();
                }
                else
                {
                    Pause();
                }
                _novelUI.SetDisplay(value);
                _isDisplay = value;
            }
        }

        public bool mute
        {
            get
            {
                return _mute;
            }
            set
            {
                _audioPlayer.SetMute(value);
                _mute = value;
            }
        }

        public float BGMVolume
        {
            get
            {
                return _BGMVolume;
            }

            set
            {
                _BGMVolume = Mathf.Clamp(value, 0, 1);
                _audioPlayer.SetBGMVolume(_BGMVolume);
            }
        }

        public float SEVolume
        {
            get
            {
                return _SEVolume;
            }

            set
            {
                _SEVolume = Mathf.Clamp(value, 0, 1);
                _audioPlayer.SetSEVolume(_SEVolume);
            }
        }

        public int textSpeed
        {
            get
            {
                return _textSpeed;
            }
            set
            {
                _textSpeed = value;

                if (value < 1)
                {
                    _textSpeed = 1;
                }

                _novelUI.SetTextSpeed(_textSpeed);
            }
        }

        public List<string> ParagraphName => _ParagraphName;
        public List<string> ChoiceName => _choiceName;

        #endregion

        #region delegate
        public delegate void OnBeginDelegate();
        public OnBeginDelegate OnBegin;
        public OnBeginDelegate OnLoad;

        public delegate void OnEndDelegate();
        public OnEndDelegate OnEnd;

        public delegate void OnDialogueChangedDelegate();
        public OnDialogueChangedDelegate OnDialogueChanged;

        public delegate void NodeChangedDelegate(string nodeName);
        public NodeChangedDelegate ParagraphNodeChanged;
        public NodeChangedDelegate OnChoiced;

        #endregion


        #region publicMethod

        public void Play(NovelData data, bool hideAfterPlay)
        {
            _novelData = data;
            _hideAfterPlay = hideAfterPlay;

            Reset();

            _nowDialogueNum = 0;
            _nowParagraph = _novelData.paragraphList[0];
            _ParagraphName.Add(_nowParagraph.nodeName);
            if (ParagraphNodeChanged != null)
                ParagraphNodeChanged(_nowParagraph.nodeName);
            _passedParagraphID.Add(0);

            UnPause();
            if (OnBegin != null)
                OnBegin();

            SetNext();
            _isPlaying = true;
        }

        public void Load(NovelSaveData saveData, bool hideAfterPlay)
        {
            _novelData = saveData.novelData;
            _hideAfterPlay = hideAfterPlay;
            _textCTS.Cancel();
            _novelUI.FlashText();

            Reset();

            _nowDialogueNum = saveData.dialogueIndex;
            _nowParagraph = _novelData.paragraphList[saveData.paragraphIndex];
            _ParagraphName = saveData.ParagraphName;
            _choiceName = saveData.choiceName;

            //復元、新しいデータをとりあえず再生
            NovelData.ParagraphData.Dialogue newData = DataLoader.Instance.LoadDialogue(saveData);

            UnPause();

            if (OnLoad != null)
                OnLoad();

            SetNextDialogue(newData);
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
            _novelUI.SetUIDisplay(false);
            Pause();
            _isUIDisplay = false;
        }

        public void DisplayUI()
        {
            _novelUI.SetUIDisplay(true);
            UnPause();
            _isUIDisplay = true;
        }

        public void Skip()
        {
            if (_isChoicing || _isImageChangeing)
            {
                return;
            }
            _textCTS.Cancel();
            SkipedData newData = DataLoader.Instance.Skip(novelData, _nowParagraph, _nowDialogueNum, _passedParagraphID, _ParagraphName, _novelUI.GetNowBack());
            if (newData.next == Next.Choice)
            {
                _nowParagraph = novelData.paragraphList[newData.ParagraphIndex];
                _nowDialogueNum = _nowParagraph.dialogueList.Count;
                SetNextDialogue(newData.dialogue);
            }
            else
            {
                end();
            }

        }

        public void SkipNextNode()
        {
            if (!_isChoicing && !_isImageChangeing)
            {
                _textCTS.Cancel();

                NovelData.ParagraphData.Dialogue newData = DataLoader.Instance.SkipNextNode(novelData, _nowParagraph, _nowDialogueNum, _novelUI.GetNowBack());

                if (newData == null || (_nowParagraph.next == Next.Continue && _nowParagraph.nextParagraphIndex == -1))
                {
                    end();
                    return;
                }

                if (_nowParagraph.next == Next.Choice)
                {
                    _nowDialogueNum = _nowParagraph.dialogueList.Count;
                }
                else
                {
                    _nowParagraph = _novelData.paragraphList[_nowParagraph.nextParagraphIndex];
                    _nowDialogueNum = 0;
                    _ParagraphName.Add(_nowParagraph.nodeName);
                    if (ParagraphNodeChanged != null)
                        ParagraphNodeChanged(_nowParagraph.nodeName);
                }
                SetNextDialogue(newData);

            }
        }

        public NovelSaveData save()
        {
            return DataLoader.Instance.SaveDialogue(novelData, _nowParagraph.index, _nowDialogueNum, _passedParagraphID, ChoiceName, ParagraphName);
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
                case HowInput.Default:
                    _inputProvider = new DefaultInputProvider();
                    break;
            }

            _novelUI = GetComponent<NovelUIManager>();
            _novelUI.Init(_charaFadeTime, _nonameDialogueSprite, _dialogueSprite);
            SetDisplay(_isDisplay);
            _audioPlayer = gameObject.AddComponent<AudioPlayer>();
            _audioPlayer.Init(_BGMVolume, _SEVolume);
            _choiceManager = GetComponentInChildren<ChoiceManager>();
            _choiceManager.Init(_choiceButton);

            if (_playOnAwake && _novelData != null)
            {
                Play(_novelData, _hideAfterPlay);
            }
        }

        void SetDisplay(bool isDisplay)
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
                Pause();
                _novelUI.SetDisplay(isDisplay);
                _isDisplay = false;
            }


        }

        //現在再生しているものをリセット
        void Reset()
        {
            _novelUI.Reset(_novelData.locations);
            //選択肢を全部消す
            _choiceManager.ResetChoice();
            _isChoicing = false;
            _isEnd = false;

            //今までのやつを消す
            _choiceName.Clear();
            _ParagraphName.Clear();

            SetDisplay(true);
        }

        void Update()
        {
            if (!_isPlaying || _isImageChangeing || !_isDisplay)
            {
                return;
            }

            if (_inputProvider.GetHideOrDisplay())
            {
                if (_isUIDisplay)
                    HideUI();
                else if (!_isUIDisplay)
                    DisplayUI();
            }

            if (_isChoicing || !_isUIDisplay)
            {
                return;
            }

            if (_inputProvider.GetNext())
            {
                //全部表示
                if (_isReading && _novelUI.canFlush)
                {
                    FlashText();
                }
                else if (!_isReading)
                {
                    SetNext();
                }

            }
            if (_inputProvider.GetSkip())
            {
                Skip();
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
                _nowParagraph = _novelData.paragraphList[nextIndex];
                _ParagraphName.Add(_nowParagraph.nodeName);
                if (ParagraphNodeChanged != null)
                    ParagraphNodeChanged(_nowParagraph.nodeName);
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
                list.Add(_novelData.choiceList[i]);
            }
            if (list.Count == 0)
            {
                end();
                return;
            }

            var ans = await _choiceManager.WaitChoice(list);
            _choiceName.Add(ans.nodeName);
            if (OnChoiced != null)
                OnChoiced(ans.nodeName);
            SetNextParagraph(ans.nextParagraphIndex);
            _isChoicing = false;
        }


        async void SetNextDialogue(NovelData.ParagraphData.Dialogue newData)
        {
            _isImageChangeing = true;
            _imageCTS = new CancellationTokenSource();
            _novelUI.DeleteText();
            _isImageChangeing = !await _novelUI.SetNextImage(newData, _imageCTS.Token);
            _audioPlayer.SetSound(newData);
            _textCTS = new CancellationTokenSource();
            _isReading = true;
            _novelUI.SetDefaultFont();
            _nowDialogueNum++;
            _isReading = !await _novelUI.SetNextText(newData, _textCTS.Token);
            if (OnDialogueChanged != null)
                OnDialogueChanged();
        }

        async void SetNextDialogue()
        {
            _isImageChangeing = true;
            _imageCTS = new CancellationTokenSource();
            _isImageChangeing = !await _novelUI.SetNextImage(_nowParagraph.dialogueList[_nowDialogueNum], _imageCTS.Token);
            _audioPlayer.SetSound(_nowParagraph.dialogueList[_nowDialogueNum]);
            _textCTS = new CancellationTokenSource();
            _isReading = true;
            _nowDialogueNum++;
            _isReading = !await _novelUI.SetNextText(_nowParagraph.dialogueList[_nowDialogueNum - 1], _textCTS.Token);
            if (OnDialogueChanged != null)
                OnDialogueChanged();
        }

        async void end()
        {
            if (_isEnd)
                return;
            _isEnd = true;
            if (_hideAfterPlay)
            {
                _isImageChangeing = true;
                _endFadeCTS = new CancellationTokenSource();
                _audioPlayer.AllStop();
                await _novelUI.FadeOut(_hideFadeTime, _endFadeCTS.Token);
                SetDisplay(false);
                if (OnEnd != null)
                    OnEnd();
                _isImageChangeing = false;
            }
            _isPlaying = false;
        }

        void FlashText()
        {
            _textCTS.Cancel();
            _novelUI.FlashText();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += _OnValidate;
        }

        private void _OnValidate()
        {
            UnityEditor.EditorApplication.delayCall -= _OnValidate;
            if (this == null) return;
            if (_audioPlayer != null)
            {
                _audioPlayer.SetSEVolume(_SEVolume);
                _audioPlayer.SetBGMVolume(_BGMVolume);
            }

            if (_novelUI == null)
            {
                GetComponent<CanvasGroup>().alpha = _isDisplay ? 1 : 0;
            }
            else
            {
                SetDisplay(_isDisplay);
            }

        }
#endif

        void OnDisable()
        {
            allCancel();
        }

        void OnDestroy()
        {
            allCancel();
        }

        void allCancel()
        {
            try
            {
                _audioPlayer.AllStop();
            }
            catch { }
            _textCTS.Cancel();
            _imageCTS.Cancel();
            _endFadeCTS.Cancel();
        }
        #endregion

    }


}

