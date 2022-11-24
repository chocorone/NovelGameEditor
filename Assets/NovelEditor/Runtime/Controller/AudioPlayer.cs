using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using static NovelEditorPlugin.NovelData.ParagraphData;

namespace NovelEditorPlugin
{
    public class AudioPlayer : MonoBehaviour
    {
        AudioSource _BGM;
        AudioSource _SE;

        private float _SEVolume;
        private float _BGMVolume;

        CancellationTokenSource BGMcancellation = new CancellationTokenSource();
        CancellationTokenSource SEcancellation = new CancellationTokenSource();

        bool _isFading = false;

        public void Init(float bgmVolume, float seVolume)
        {
            _BGM = gameObject.AddComponent<AudioSource>();
            _BGM.playOnAwake = false;

            _SE = gameObject.AddComponent<AudioSource>();
            _SE.playOnAwake = false;

            SetSEVolume(seVolume);
            SetBGMVolume(bgmVolume);
        }

        internal void SetSound(Dialogue data)
        {
            if (data.BGMStyle != SoundStyle.UnChange)
            {
                SetBGM(data);
            }

            if (data.SEStyle != SoundStyle.UnChange)
            {
                SetSE(data);
            }
        }

        internal void AllStop()
        {
            BGMcancellation.Cancel();
            SEcancellation.Cancel();
            _BGM.Stop();
            _SE.Stop();
        }

        void SetBGM(Dialogue data)
        {
            switch (data.BGMStyle)
            {
                case SoundStyle.Play:
                    Stop(_BGM, BGMcancellation);
                    BGMcancellation = new CancellationTokenSource();
                    SoundData soundData = new SoundData(data.BGM, data.BGMLoop, data.BGMCount, data.BGMSecond, data.BGMFadeTime, data.BGMEndFadeTime);
                    var t = Play(soundData, _BGMVolume, _BGM, BGMcancellation.Token);
                    break;
                case SoundStyle.Stop:
                    Stop(_BGM, BGMcancellation);
                    break;
            }
        }

        void SetSE(Dialogue data)
        {
            switch (data.SEStyle)
            {
                case SoundStyle.Play:
                    Stop(_SE, SEcancellation);
                    SEcancellation = new CancellationTokenSource();
                    SoundData soundData = new SoundData(data.SE, data.SELoop, data.SECount, data.SESecond, data.SEFadeTime, data.SEEndFadeTime);
                    var t = Play(soundData, _SEVolume, _SE, SEcancellation.Token);
                    break;
                case SoundStyle.Stop:
                    Stop(_SE, SEcancellation);
                    break;
            }
        }

        void Stop(AudioSource player, CancellationTokenSource cancel)
        {
            cancel.Cancel();
            cancel.Dispose();
            player.Stop();
        }

        async UniTask<bool> Play(SoundData data, float defaultVolume, AudioSource player, CancellationToken token)
        {
            player.clip = data.clip;
            player.volume = 0;
            player.Play();
            await FadeVolume(0, defaultVolume, data.FadeTime, player, token);
            switch (data.Loop)
            {
                case LoopMode.Count:
                    await UniTask.Delay((int)(data.clip.length * data.Count * 1000), cancellationToken: token);
                    await FadeVolume(defaultVolume, 0, data.EndFadeTime, player, token);
                    player.Stop();
                    break;
                case LoopMode.Second:
                    await UniTask.Delay((int)(data.Second * 1000), cancellationToken: token);
                    await FadeVolume(defaultVolume, 0, data.EndFadeTime, player, token);
                    player.Stop();
                    break;
            }
            return true;
        }

        internal async void SetSEVolume(float seVolume)
        {
            if (_isFading)
                await UniTask.WaitWhile(() => _isFading);
            _SEVolume = seVolume;
            _SE.volume = _SEVolume;
        }

        internal async void SetBGMVolume(float bgmVolume)
        {
            if (_isFading)
                await UniTask.WaitWhile(() => _isFading);
            _BGMVolume = bgmVolume;
            _BGM.volume = _BGMVolume;
        }

        async UniTask<bool> FadeVolume(float from, float dest, float time, AudioSource player, CancellationToken token)
        {
            float value = 0;
            float volumeSpeed = 0.01f;

            if (time < 0.5)
            {
                volumeSpeed = 0.1f;
            }
            _isFading = true;
            player.volume = from;
            from = Mathf.Clamp(from, 0, 1);
            dest = Mathf.Clamp(dest, 0, 1);

            try
            {
                while (value < 1)
                {
                    player.volume = from + (dest - from) * value;
                    await UniTask.Delay(TimeSpan.FromSeconds(time * volumeSpeed), cancellationToken: token);
                    value += volumeSpeed;
                }
            }
            catch (OperationCanceledException)
            { }
            finally
            {
                _isFading = false;
                player.volume = dest;
            }
            return true;
        }


        struct SoundData
        {
            public SoundData(AudioClip _clip, LoopMode _mode, int _count, float _second, float _fadeTime, float _endFadeTime)
            {
                clip = _clip;
                Loop = _mode;
                Count = _count;
                Second = _second;
                FadeTime = _fadeTime;
                EndFadeTime = _endFadeTime;
            }
            public AudioClip clip { get; private set; }
            public LoopMode Loop { get; private set; }
            public int Count { get; private set; }
            public float Second { get; private set; }
            public float FadeTime { get; private set; }
            public float EndFadeTime { get; private set; }
        }
    }
}