using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;

[CustomEditor(typeof(NovelData))]
internal class NovelDataInspector : Editor
{
    NovelData noveldata;

    ProgressBar bar;
    Label label;

    void OnEnable()
    {
        noveldata = target as NovelData;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var visualElement = new VisualElement();
        visualElement.styleSheets.Add(Resources.Load<StyleSheet>("NovelDataUSS"));

        var container = new IMGUIContainer(OnInspectorGUI);
        visualElement.Add(container);
        var visualTree = Resources.Load<VisualTreeAsset>("NovelDataUXML");
        visualTree.CloneTree(visualElement);

        var button = visualElement.Q<Button>("open_button");
        button.clickable.clicked += OpenEditor;

        var prefabButton = visualElement.Q<Button>("prefab_button");
        prefabButton.clickable.clicked += ChangePrefab;

        label = visualElement.Q<Label>();

        bar = visualElement.Q<ProgressBar>();
        bar.style.display = DisplayStyle.None;


        return visualElement;
    }

    void OpenEditor()
    {
        if (noveldata.newData)
        {
            noveldata.ResetData();
        }
        NovelEditor.Open(noveldata);
    }

    void ChangePrefab()
    {
        label.text = "処理中";
        bar.value = 0;
        bar.style.display = DisplayStyle.Flex;

        float perParagraph = 100 / noveldata.paragraphsList.Count;

        foreach (ParagraphData pdata in noveldata.paragraphsList)
        {
            float perDialogue = perParagraph / pdata.dialogueList.Count;
            foreach (Dialogue dialogue in pdata.dialogueList)
            {
                dialogue.charas = new Sprite[noveldata.locations.Count];
                dialogue.howCharas = new CharaChangeStyle[noveldata.locations.Count];
                dialogue.charaFadeColor = new Color[noveldata.locations.Count];
                dialogue.charaEffects = new Effect[noveldata.locations.Count];
                dialogue.charaEffectStrength = new int[noveldata.locations.Count];

                bar.value += perDialogue;
                Debug.Log(bar.value);
            }
        }
        bar.value = 100;
        noveldata.havePreLocations = true;
        label.text = "処理完了";
    }

}
