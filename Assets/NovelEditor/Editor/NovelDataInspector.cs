using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using static NovelData;
using static NovelData.ParagraphData;
using UnityEngine.UIElements;
using UnityEditorInternal;

[CustomEditor(typeof(NovelData))]
internal class NovelDataInspector : Editor
{
    NovelData noveldata;
    ProgressBar bar;
    Label label;

    private ReorderableList reorderableList;

    void OnEnable()
    {
        noveldata = target as NovelData;

        LocationWrapper wrapper = new LocationWrapper() { locations = noveldata.locations };
        string json = JsonUtility.ToJson(wrapper);
        noveldata.newLocations = JsonUtility.FromJson<LocationWrapper>(json).locations;
        SetReorderableList();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        reorderableList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
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

        label = visualElement.Q<Label>("progressLabel");

        bar = visualElement.Q<ProgressBar>();
        bar.style.display = DisplayStyle.None;


        return visualElement;
    }


    void SetReorderableList()
    {
        reorderableList = new ReorderableList(this.serializedObject, this.serializedObject.FindProperty("newLocations"));
        reorderableList.drawElementCallback = (rect, index, active, focused) =>
        {
            EditorGUI.ObjectField(rect, this.serializedObject.FindProperty("newLocations").GetArrayElementAtIndex(index));
        };
        reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "立ち絵の位置");

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
        Dictionary<int, int> locationsKey = new Dictionary<int, int>();

        for (int i = 0; i < noveldata.locations.Count; i++)
        {
            locationsKey.Add(noveldata.locations[i].GetInstanceID(), i);
        }

        Debug.Log("before" + locationsKey.Count);
        //noveldata.locations.ForEach(x => Debug.Log(x.name + ":" + x.GetInstanceID()));

        noveldata.newLocations.RemoveAll(item => item == null);
        noveldata.newLocations = noveldata.newLocations.Distinct().ToList();

        noveldata.changeLocation(noveldata.newLocations);

        float perParagraph = 100 / noveldata.paragraphsList.Count;

        //データを初期化
        foreach (ParagraphData pdata in noveldata.paragraphsList)
        {
            float perDialogue = perParagraph / pdata.dialogueList.Count;
            foreach (Dialogue dialogue in pdata.dialogueList)
            {
                //データ保存
                Dialogue preData = JsonUtility.FromJson<Dialogue>(JsonUtility.ToJson(dialogue));

                dialogue.charas = new Sprite[noveldata.locations.Count];
                dialogue.howCharas = new CharaChangeStyle[noveldata.locations.Count];
                dialogue.charaFadeColor = new Color[noveldata.locations.Count];
                dialogue.charaEffects = new Effect[noveldata.locations.Count];
                dialogue.charaEffectStrength = new int[noveldata.locations.Count];

                //同じ名前のデータがあれば差し替え
                for (int i = 0; i < noveldata.locations.Count; i++)
                {
                    if (locationsKey.ContainsKey(noveldata.locations[i].GetInstanceID()))
                    {
                        int preIndex = locationsKey[noveldata.locations[i].GetInstanceID()];

                        dialogue.charas[i] = preData.charas[preIndex];
                        dialogue.howCharas[i] = preData.howCharas[preIndex];
                        dialogue.charaFadeColor[i] = preData.charaFadeColor[preIndex];
                        dialogue.charaEffects[i] = preData.charaEffects[preIndex];
                        dialogue.charaEffectStrength[i] = preData.charaEffectStrength[preIndex];
                    }
                }

                bar.value += perDialogue;
            }
        }

        bar.value = 100;
        noveldata.havePreLocations = true;
        label.text = "処理完了";

        EditorUtility.SetDirty(noveldata);
    }

    class LocationWrapper
    {
        public List<UnityEngine.UI.Image> locations;
    }
}
