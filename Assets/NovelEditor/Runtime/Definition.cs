using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEditor
{
    public enum HowInput
    {
        Default,
        UserSetting,
        OverWrite
    }

    public enum Next
    {
        Continue,
        Choice,
        End
    }

    public enum CharaChangeStyle
    {
        UnChange,
        Quick,
        dissolve
    }

    public enum BackChangeStyle
    {
        UnChange,
        Quick,
        FadeAll,
        FadeFront,
        FadeBack,
        dissolve
    }

    public enum SoundStyle
    {
        UnChange,
        Play,
        Stop
    }

    public enum LoopMode
    {
        Endless,
        Count,
        Second,
    }

    public enum Effect
    {
        UnChange,
        None,
        Noise,
        Mosaic,
        GrayScale,
        Sepia,
        Jaggy,
        ChromaticAberration,
        Blur
    }

}
