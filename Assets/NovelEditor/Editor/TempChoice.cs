using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DialogueDesigner;

namespace DialogueDesigner.Editor
{
    internal class TempChoice : ScriptableObject
    {
        [HideInInspector] public NovelData.ChoiceData data;

    }
}