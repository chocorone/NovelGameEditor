using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;

[CustomEditor(typeof(TempParagraph))]
internal class ParagraphInspector : Editor
{
    internal static bool dataChanged = false;
    TempParagraph tmpdata;
    private ReorderableList reorderableList;
    private SerializedProperty daialogueDataList;
    private int index;

    void OnEnable()
    {
        tmpdata = target as TempParagraph;

        SerializedProperty data = serializedObject.FindProperty(nameof(tmpdata.data));
        index = tmpdata.data.index;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var visualElement = new VisualElement();

        Label label = new Label();
        if (index == 0)
        {
            label.text = "最初に表示される会話です";
        }
        else
        {
            label.text = "！現在の立ち絵や背景に注意";
        }
        visualElement.Add(label);

        Toggle toggle = new Toggle();
        toggle.text = "詳細設定全部開く";
        visualElement.Add(toggle);

        var list = new ListView();
        list.reorderable = true;
        list.showBorder = true;
        list.showAddRemoveFooter = true;
        list.bindingPath = "dialogueList";
        //list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        list.fixedItemHeight = 80;

        visualElement.Add(list);


        return visualElement;
    }

}