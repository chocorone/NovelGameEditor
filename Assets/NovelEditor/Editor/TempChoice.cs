using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NovelEditor;

namespace NovelEditor.Editor
{
    internal class TempChoice : ScriptableObject
    {
        [HideInInspector] public NovelData.ChoiceData data;

    }
}