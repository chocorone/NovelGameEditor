using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NovelEditorPlugin;

namespace NovelEditorPlugin.Editor
{
    internal class TempChoice : ScriptableObject
    {
        [HideInInspector] public NovelData.ChoiceData data;

    }
}