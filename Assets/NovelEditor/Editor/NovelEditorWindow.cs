using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueDesigner;

namespace DialogueDesigner.Editor
{
    /// <summary>
    /// ウィンドウを表示するためのクラス
    /// </summary>
    public class NovelEditorWindow : EditorWindow
    {
        [SerializeField] NovelData _editingData;
        internal static NovelData editingData => Instance._editingData;
        private static NovelEditorWindow instance;
        public static NovelEditorWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = EditorWindow.GetWindow<NovelEditorWindow>("EditorWindow", typeof(UnityEditor.SceneView));
                    if (instance == null)
                    {
                        Debug.LogError("NullRefarenceException NovelEditorWindow SingltonError");
                    }
                }
                return instance;
            }
        }


        /// <summary>
        /// ウィンドウを表示する
        /// </summary>
        internal void Init(NovelData data)
        {
            _editingData = data;
            rootVisualElement.Clear();
            rootVisualElement.Bind(new SerializedObject(this));

            Draw();
        }

        void OnEnable()
        {
            Draw();

        }

        void Draw()
        {
            NovelData.NodeData data = BaseNode.nowSelection?.nodeData;

            GraphController controller = new GraphController();
            NovelGraphView graphView = controller.CreateGraph();
            rootVisualElement.Add(graphView);

            if (data != null)
            {
                Selection.activeObject = null;
                if (data is NovelData.ParagraphData && ParagraphNode.nodes.Count > data.index)
                {
                    ParagraphNode.nodes[data.index]?.OnSelected();
                }
                else if (ChoiceNode.nodes.Count > data.index)
                {
                    ChoiceNode.nodes[data.index]?.OnSelected();
                }
            }

            string name = "NoData";
            if (_editingData != null)
            {
                name = _editingData.name;
            }

            var box = new Box();
            box.Add(new Label() { text = name });
            rootVisualElement.Add(box);
        }

    }
}