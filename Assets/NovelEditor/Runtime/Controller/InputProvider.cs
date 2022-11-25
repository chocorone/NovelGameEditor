using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NovelEditor
{
    interface NovelInputProvider
    {
        bool GetNext();
        bool GetSkip();
        bool GetHideOrDisplay();
        bool GetStopOrStart();
    }

    public class DefaultInputProvider : NovelInputProvider
    {
        public bool GetNext()
        {
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0);
        }
        public bool GetSkip()
        {
            return Input.GetKeyDown(KeyCode.S);
        }
        public bool GetHideOrDisplay()
        {
            return Input.GetKeyDown(KeyCode.H);
        }

        public bool GetStopOrStart()
        {
            return Input.GetKeyDown(KeyCode.S);
        }

    }

    public class CustomInputProvider : NovelInputProvider
    {
        KeyCode[] _nextButton;
        KeyCode[] _skipButton;
        KeyCode[] _hideOrDisplayButton;
        KeyCode[] _stopOrStartButton;

        public CustomInputProvider(KeyCode[] nextButton, KeyCode[] skipButton, KeyCode[] hideOrDisplayButton, KeyCode[] stopOrStartButton)
        {
            _nextButton = nextButton;
            _skipButton = skipButton;
            _hideOrDisplayButton = hideOrDisplayButton;
            _stopOrStartButton = stopOrStartButton;
        }

        public bool GetNext()
        {
            return _nextButton.Any(key => Input.GetKeyDown(key));
        }
        public bool GetSkip()
        {
            return _skipButton.Any(key => Input.GetKeyDown(key));
        }
        public bool GetHideOrDisplay()
        {
            return _hideOrDisplayButton.Any(key => Input.GetKeyDown(key));
        }

        public bool GetStopOrStart()
        {
            return _stopOrStartButton.Any(key => Input.GetKeyDown(key));
        }

    }
}


