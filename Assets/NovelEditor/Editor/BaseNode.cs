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

    // public override void OnSelected()
    // {
    //     try
    //     {
    //         TempChoice temp = ScriptableObject.CreateInstance<TempChoice>();
    //         //ノードのデータをインスペクターに反映
    //         temp.data = data;
    //         Selection.activeObject = temp;
    //     }
    //     catch
    //     {
    //         Selection.activeObject = null;
    //     }
    // }

    public override void OnUnselected()
    {
        Selection.activeObject = null;

        SetTitle();
    }

    internal abstract void AddNext(BaseNode nextNode, Port outPort);

}