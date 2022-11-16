using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TempChoice))]
internal class ChoiceInspector : Editor
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
        if (NovelEditorWindow.Compiled)
        {
            label.text = "ノードをクリックし直してください";
            label.style.color = new StyleColor(Color.red);
            label.style.fontSize = 20;
            root.Add(label);
            return root;
        }

        var container = new IMGUIContainer(OnInspectorGUI);
        root.Add(container);


        return root;
    }
}