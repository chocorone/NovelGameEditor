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

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        root.styleSheets.Add(Resources.Load<StyleSheet>("DialogueUSS"));

        var DialogueUXML = Resources.Load<VisualTreeAsset>("DialogueUXML");
        DialogueUXML.CloneTree(root);

        var imageFold = root.Q<Foldout>("ImageFold");
        var charaUXML = Resources.Load<VisualTreeAsset>("CharaSettingUXML");

        int rand = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < rand; i++)
        {
            VisualElement charaTree = new VisualElement();
            charaUXML.CloneTree(charaTree);
            imageFold.Add(charaTree);
        }




        return root;
    }
}