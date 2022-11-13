using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Dialogue))]
internal class DialogueDrawer : PropertyDrawer
{

    [SerializeField] private Sprite[] tempSp = null;
    [SerializeField] private Sprite[] tempBack = new Sprite[1];
    int index;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var height = EditorGUIUtility.singleLineHeight;
        int elementNum = property.FindPropertyRelative("elementNum").intValue;
        height *= elementNum;

        return height;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var visualElement = new VisualElement();

        //var container = new IMGUIContainer(OnInspectorGUI);
        //visualElement.Add(container);
        var button = new Button()
        {
            text = "Example Button"
        };
        visualElement.Add(button);

        return visualElement;
    }
}