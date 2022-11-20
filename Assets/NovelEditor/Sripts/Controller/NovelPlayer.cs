using System.Collections.Generic;
using UnityEngine;
using static NovelData;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(NovelUIManager))]
public class NovelPlayer : MonoBehaviour
{
    [SerializeField] private NovelData _noveldata;

    [SerializeField] private bool _playOnAwake = true;
    [SerializeField] private bool _hideAfterPlay = false;
    [SerializeField] private bool _isDisplay = true;

    [SerializeField] private HowInput _inputSystem;
    [SerializeField] private KeyCode[] _nextButton;
    [SerializeField] private KeyCode[] _skipButton;
    [SerializeField] private KeyCode[] _hideOrDisplayButton;
    [SerializeField] private KeyCode[] _stopOrStartButton;

    [SerializeField] public float SEVolume;
    [SerializeField] public float BGMVolume;

    NovelInputProvider _inputProvider;

    public bool IsStop { get; private set; } = false;
    public bool IsPlaying { get; private set; } = false;
    public bool IsChoicing { get; private set; } = false;
    public bool IsDisplay => _isDisplay;

    public int nowDialogueNum { get; private set; } = 0;

    private NovelUIManager novelUI;
    private AudioPlayer audioPlayer;
    private ParagraphData _nowParagraph;
    private bool _isReading = false;
    private bool _isImageChangeing = false;

    CancellationTokenSource textCTS = new CancellationTokenSource();
    CancellationTokenSource imageCTS = new CancellationTokenSource();

    void Awake()
    {
        switch (_inputSystem)
        {
            case HowInput.Default:
                _inputProvider = new DefaultInputProvider();
                break;
            case HowInput.UserSetting:
                _inputProvider = new CustomInputProvider(_nextButton, _skipButton, _hideOrDisplayButton, _stopOrStartButton);
                Debug.Log("aaa");
                break;
        }

        novelUI = GetComponent<NovelUIManager>();
        audioPlayer = gameObject.AddComponent<AudioPlayer>();

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
        // if (IsStop)
        // {
        //     novelUI.SetStop();
        //     IsStop = true;
        // }else{

        // }
    }

    public void SetDisplay(bool isDisplay)
    {
        if (isDisplay)
        {
            SetStop(false);
            //novelUI.SetDisplay(isDisplay);
            _isDisplay = true;
        }
        else
        {
            SetStop(true);
            //novelUI.SetDisplay(isDisplay);
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
            novelUI.StopOrStartText();
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
        //audioPlayer.PlaySound(nowParagraph.dialogueList[nowDialogueNum]);
        textCTS.Dispose();
        textCTS = new CancellationTokenSource();
        _isReading = true;
        _isReading = !await novelUI.SetNextText(_nowParagraph.dialogueList[nowDialogueNum], textCTS.Token);
        nowDialogueNum++;
    }

    void end()
    {
        IsPlaying = false;
    }

    void FlashText()
    {
        textCTS.Cancel();
    }

}