using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager
{
    Shader shader;

    public EffectManager()
    {
        shader = Resources.Load<Shader>("EditorBackGround");
    }

}
