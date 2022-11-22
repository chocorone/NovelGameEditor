using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelEditorPlugin
{
    public class EffectManager
    {
        Shader shader;

        public EffectManager()
        {
            shader = Resources.Load<Shader>("EditorBackGround");
        }

    }

}
