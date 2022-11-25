using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NovelEditor
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

        public void InitMaterial(Image image)
        {
            image.material = new Material(None);
        }

        public void copyShader(Image from, Image dest)
        {
            dest.material = GameObject.Instantiate(from.material);
        }

        public void SetEffect(Image image, Effect effect, float strength)
        {

            switch (effect)
            {
                case Effect.None:
                    image.material.shader = None;
                    break;
                case Effect.Noise:
                    image.material.shader = Noise;
                    break;
                case Effect.Mosaic:
                    image.material.shader = Mosaic;
                    break;
                case Effect.GrayScale:
                    image.material.shader = GrayScale;
                    break;
                case Effect.Sepia:
                    image.material.shader = Sepia;
                    break;
                case Effect.Jaggy:
                    image.material.shader = Jaggy;
                    break;
                case Effect.Holo:
                    image.material.shader = Holo;
                    break;
                case Effect.ChromaticAberration:
                    image.material.shader = ChromaticAberration;
                    break;
                case Effect.Blur:
                    image.material.shader = Blur;
                    break;
            }
            if (image.material.HasProperty("_Strength"))
            {
                image.material.SetFloat("_Strength", strength);
            }
        }

    }

}
