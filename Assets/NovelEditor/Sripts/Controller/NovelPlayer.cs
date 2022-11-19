using System.Collections.Generic;
using UnityEngine;
using static NovelData;

[RequireComponent(typeof(NovelUIManager))]
public class NovelPlayer : MonoBehaviour
{
    [SerializeField] NovelData noveldata;

    [SerializeField] bool playOnAwake;
    [SerializeField] bool hideAfterPlay;
    [SerializeField] HowInput inputSystem;
    [SerializeField] KeyCode[] nextButton;
    [SerializeField] KeyCode[] skipButton;
    [SerializeField] KeyCode[] hideOrDisplayButton;
    [SerializeField] KeyCode[] stopOrStartButton;

    NovelInputProvider inputProvider;

    void Awake()
    {
        switch (inputSystem)
        {
            case HowInput.Auto:
                inputProvider = new DefaultInputProvider();
                break;
            case HowInput.UserSetting:
                inputProvider = new CustomInputProvider(nextButton, skipButton, hideOrDisplayButton, stopOrStartButton);
                Debug.Log("aaa");
                break;
        }
    }

    void Update()
    {
        if (inputProvider.GetNext())
        {
            Debug.Log("GetNext");
        }
        if (inputProvider.GetSkip())
        {
            Debug.Log("GetSkip");
        }
    }
}