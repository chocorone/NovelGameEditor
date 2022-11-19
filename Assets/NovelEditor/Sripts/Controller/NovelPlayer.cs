using System.Collections.Generic;
using UnityEngine;
using static NovelData;

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
    public bool IsPlay { get; private set; } = false;
    public bool IsChoicing { get; private set; } = false;
    public bool IsDisplay => _isDisplay;

    public int nowDialogueNum { get; private set; } = 0;

    private NovelUIManager novelUI;
    private AudioPlayer audioPlayer;
    private ParagraphData _nowParagraph;

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
        nextDialogueSet();

        SetStop(false);
        IsPlay = true;
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
        if (_inputProvider.GetNext())
        {
            Debug.Log("GetNext");
        }
        if (_inputProvider.GetSkip())
        {
            Debug.Log("GetSkip");
        }
    }

    void nextParagraphSet()
    {
        if (_nowParagraph.nextParagraphIndex == -1)
        {
            //end();
        }
        else
        {
            _nowParagraph = _noveldata.paragraphList[_nowParagraph.nextParagraphIndex];
            nowDialogueNum = 0;

            nextDialogueSet();
        }
    }

    void nextDialogueSet()
    {
        //audioPlayer.PlaySound(nowParagraph.dialogueList[nowDialogueNum]);
        novelUI.SetNextDialogue(_nowParagraph.dialogueList[nowDialogueNum]);
        nowDialogueNum++;
    }

}