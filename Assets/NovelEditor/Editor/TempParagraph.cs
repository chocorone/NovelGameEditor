using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NovelEditorPlugin;
namespace NovelEditorPlugin.Editor
{
    //ParagraphDataを仮で表示するためのもの
    internal class TempParagraph : ScriptableObject
    {
        [HideInInspector] public NovelData.ParagraphData data;
        public List<NovelData.ParagraphData.Dialogue> dialogueList;
    }
}