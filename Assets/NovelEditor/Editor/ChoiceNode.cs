using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NovelEditorPlugin;
using NovelEditorPlugin.Editor;

namespace NovelEditorPlugin.Editor
{
    internal class ChoiceNode : BaseNode
    {
        //編集しているデータにあるChoiceDataから作られたノード
        public static List<ChoiceNode> nodes = new List<ChoiceNode>();
        public NovelData.ChoiceData data => (NovelData.ChoiceData)nodeData;

        //0から作られるとき
        public ChoiceNode()
        {
            //データを作成する
            nodeData = NovelEditorWindow.editingData.CreateChoice();
            NodeSet();
            nodes.Add(this);
        }

        //データをもとに作られるとき
        public ChoiceNode(NovelData.ChoiceData Cdata)
        {
            nodeData = Cdata;

            NodeSet();
            SetPosition(data.nodePosition);
            if (data.index < nodes.Count)
            {
                nodes[data.index] = this;
            }
            else
            {
                nodes.Add(this);
            }

        }
        internal override void overrideNode(string pasteData)
        {
            NovelData.ChoiceData newData = JsonUtility.FromJson<NovelData.ChoiceData>(pasteData);
            data.text = newData.text;
            SetTitle();
        }

        private protected override void NodeSet()
        {
            base.NodeSet();

            SetTitle();

            //InputPort作成
            InputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(ChoiceNode));
            InputPort.portColor = new Color(0.7f, 0.7f, 0.0f);
            InputPort.portName = "prev";
            inputContainer.Add(InputPort);

            //OutputPort作成
            CountinuePort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(BaseNode));
            CountinuePort.portName = "next";
            outputContainer.Add(CountinuePort);
        }

        protected override void SetTitle()
        {
            title = "Choice";
            if (data != null)
            {
                title = data.text.Substring(0, Math.Min(data.text.Length, 10));
            }
        }

        internal override void AddNext(BaseNode nextNode, Port outPort)
        {
            data.ChangeNextParagraph(((ParagraphNode)nextNode).data.index);
        }

        public override void ResetNext(Edge edge)
        {
            data.ChangeNextParagraph(-1);
        }

    }
}