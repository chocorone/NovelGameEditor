using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueDesigner.Editor;

namespace DialogueDesigner.Editor
{
    [CustomEditor(typeof(TempChoice))]
    internal class ChoiceInspector : UnityEditor.Editor
    {
        TempChoice tmpdata;

        void OnEnable()
        {
            tmpdata = target as TempChoice;

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SerializedProperty data = serializedObject.FindProperty("data");
            SerializedProperty text = data.FindPropertyRelative("text");

            text.stringValue = EditorGUILayout.TextField("選択肢のテキスト", text.stringValue);

            serializedObject.ApplyModifiedProperties();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            Label label = new Label();

            var container = new IMGUIContainer(OnInspectorGUI);
            root.Add(container);


            return root;
        }
    }
}