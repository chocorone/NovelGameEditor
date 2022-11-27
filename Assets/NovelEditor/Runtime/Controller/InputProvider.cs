using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;


namespace NovelEditor
{
    public abstract class NovelInputProvider
    {

        protected bool OnUI()
        {
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            List<RaycastResult> RayResult = new List<RaycastResult>();
            pointData.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointData, RayResult);
            bool onUI = false;
            foreach (var raycastResult in RayResult)
            {
                if (!raycastResult.gameObject.GetComponent<TextMeshProUGUI>())
                    onUI = true;
            }
            return onUI;
        }
        public abstract bool GetNext();
        public abstract bool GetSkip();
        public abstract bool GetHideOrDisplay();
        public abstract bool GetStopOrStart();
    }

    internal class DefaultInputProvider : NovelInputProvider
    {
        public override bool GetNext()
        {
            return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) ||
                (Input.GetMouseButtonDown(0) && !OnUI());
        }
        public override bool GetSkip()
        {
            return Input.GetKeyDown(KeyCode.N);
        }
        public override bool GetHideOrDisplay()
        {
            return Input.GetKeyDown(KeyCode.H);
        }

        public override bool GetStopOrStart()
        {
            return Input.GetKeyDown(KeyCode.S);
        }

    }

    internal class CustomInputProvider : NovelInputProvider
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

        public override bool GetNext()
        {

            return _nextButton.Any(key =>
            {
                if (key == KeyCode.Mouse0)
                    return Input.GetKeyDown(key) && !OnUI();

                return Input.GetKeyDown(key);
            }
            );
        }
        public override bool GetSkip()
        {
            return _skipButton.Any(key => Input.GetKeyDown(key));
        }
        public override bool GetHideOrDisplay()
        {
            return _hideOrDisplayButton.Any(key => Input.GetKeyDown(key));
        }

        public override bool GetStopOrStart()
        {
            return _stopOrStartButton.Any(key => Input.GetKeyDown(key));
        }

    }
}


