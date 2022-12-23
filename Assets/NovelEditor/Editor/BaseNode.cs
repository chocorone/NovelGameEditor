using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using NovelEditor;

namespace NovelEditor.Editor
{
    internal abstract class BaseNode : Node
    {
        public NovelData.NodeData nodeData;
        internal Port InputPort { get; private protected set; }

        internal Port CountinuePort { get; private protected set; }

        public static BaseNode nowSelection { get; private set; }

        private protected virtual void NodeSet()
        {
            titleButtonContainer.Clear(); // デフォルトのCollapseボタンを削除

            RegisterCallback<MouseDownEvent>(MouseDowned);
        }

        private protected void MouseDowned(MouseEventBase<MouseDownEvent> evt)
        {
            OnSelected();
        }

        public override void SetPosition(Rect rect)
        {
            base.SetPosition(rect);
            SavePosition(rect);
        }

        void SavePosition(Rect rect)
        {
            nodeData.SavePosition(rect);
        }

        public void SaveCurrentPosition()
        {
            SavePosition(GetPosition());
        }

        protected abstract void SetTitle();

        public void DeleteNode()
        {
            nodeData.SetNodeDeleted(NovelEditorWindow.editingData);
        }

        public abstract void ResetNext(Edge edge);

        public override void OnSelected()
        {
            if (nodeData != null)
            {
                nowSelection = this;
            }
            else
            {
                Selection.activeObject = null;
                nowSelection = null;
            }

        }

        public override void OnUnselected()
        {
            Selection.activeObject = null;
            nowSelection = null;
            SetTitle();
        }

        internal abstract void overrideNode(string pasteData);
        internal abstract void AddNext(BaseNode nextNode, Port outPort);

    }
}