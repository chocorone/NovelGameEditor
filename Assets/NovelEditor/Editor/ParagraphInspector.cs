using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using DialogueDesigner;
using UnityEngine.UIElements;
using System.Reflection;

namespace DialogueDesigner.Editor
{
    [CustomEditor(typeof(TempParagraph))]
    internal class ParagraphInspector : UnityEditor.Editor
    {
        internal static TempParagraph editingData;
        TempParagraph tmpdata;
        private int index;

        VisualElement root;

        void OnEnable()
        {
            tmpdata = target as TempParagraph;
            editingData = tmpdata;
            SerializedProperty data = serializedObject.FindProperty(nameof(tmpdata.data));
            index = tmpdata.data.index;
        }


        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            return Draw(root);
        }

        VisualElement Draw(VisualElement _root)
        {
            _root = new VisualElement();
            _root.styleSheets.Add(Resources.Load<StyleSheet>("DialogueUSS"));


            Label label = new Label();
            if (index == 0)
            {
                label.text = "最初に表示される会話です";
            }
            else
            {
                label.text = "！現在の立ち絵や背景に注意";
            }
            _root.Add(label);

            var list = new ListView();
            list.reorderable = true;
            list.showBorder = true;
            list.showAddRemoveFooter = true;
            list.bindingPath = "dialogueList";
            list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            list.itemIndexChanged += (index1, index2) =>
            {
                tmpdata.data.UpdateOrder(NovelEditorWindow.editingData);
            };
            list.itemsAdded += (a) =>
            {
                tmpdata.data.UpdateOrder(NovelEditorWindow.editingData);
            };

            list.itemsRemoved += (a) =>
            {
                tmpdata.data.UpdateOrder(NovelEditorWindow.editingData);
            };

            _root.Add(list);

            return _root;
        }

        internal static void UpdateValue()
        {
            editingData.data.UpdateOrder(NovelEditorWindow.editingData);
        }

    }
}