using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEditorPlugin
{
    public class EffectManager
    {
        static EffectManager instance;
        public static EffectManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EffectManager();
                }
                return instance;
            }
        }

        Shader None;
        Shader Noise;
        Shader Mosaic;
        Shader GrayScale;
        Shader Sepia;
        Shader Jaggy;
        Shader Holo;
        Shader ChromaticAberration;
        Shader Blur;

        public EffectManager()
        {
            None = Resources.Load<Shader>("DefaultEffect");
            Noise = Resources.Load<Shader>("NoiseEffect");
            Mosaic = Resources.Load<Shader>("MosaicEffect");
            GrayScale = Resources.Load<Shader>("GrayScaleEffect");
            Sepia = Resources.Load<Shader>("SepiaEffect");
            Jaggy = Resources.Load<Shader>("JaggyEffect");
            Holo = Resources.Load<Shader>("HoloEffect");
            ChromaticAberration = Resources.Load<Shader>("ChromaticEffect");
            Blur = Resources.Load<Shader>("BlurEffect");
        }

        public void SetEffect(Image image, Effect effect)
        {
            switch (effect)
            {
                case Effect.None:
                    break;
                case Effect.Noise:
                    break;
                case Effect.Mosaic:
                    break;
                case Effect.GrayScale:
                    break;
                case Effect.Sepia:
                    break;
                case Effect.Jaggy:
                    break;
                case Effect.Holo:
                    break;
                case Effect.ChromaticAberration:
                    break;
                case Effect.Blur:
                    break;
            }

            if (effect != Effect.None && effect != Effect.UnChange)
            {

            }
        }

    }

}
