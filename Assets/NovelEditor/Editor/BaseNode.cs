using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static NovelData;
using UnityEditor;

internal abstract class BaseNode : Node
{
    public NodeData nodeData;
    internal Port InputPort { get; private protected set; }

    internal Port CountinuePort { get; private protected set; }

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
        nodeData.SetNodeDeleted();
    }

    public abstract void ResetNext(Edge edge);

    public override void OnSelected()
    {
        if (nodeData != null)
        {
            if (nodeData is ChoiceData)
            {
                TempChoice temp = ScriptableObject.CreateInstance<TempChoice>();
                temp.data = (ChoiceData)nodeData;
                Selection.activeObject = temp;
            }

            if (nodeData is ParagraphData)
            {
                TempParagraph temp = ScriptableObject.CreateInstance<TempParagraph>();
                temp.data = (ParagraphData)nodeData;
                temp.dialogueList = ((ParagraphData)nodeData).dialogueList;
                Selection.activeObject = temp;
            }
        }
        else
        {
            Selection.activeObject = null;
        }

    }

    public override void OnUnselected()
    {
        Selection.activeObject = null;

        SetTitle();
    }

    internal abstract void overrideNode(string pasteData);
    internal abstract void AddNext(BaseNode nextNode, Port outPort);

}