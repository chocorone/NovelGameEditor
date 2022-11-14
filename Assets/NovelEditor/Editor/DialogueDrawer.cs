using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static NovelData.ParagraphData;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Dialogue))]
internal class DialogueDrawer : PropertyDrawer
{

    public override VisualElement CreatePropertyGUI(SerializedProperty data)
    {
        //UI作成
        var root = new VisualElement();
        root.styleSheets.Add(Resources.Load<StyleSheet>("DialogueUSS"));

        var DialogueUXML = Resources.Load<VisualTreeAsset>("DialogueUXML");
        DialogueUXML.CloneTree(root);

        //立ち絵の数に応じて作成
        var charaImageBox = root.Q<Box>("charaImage");
        var charaUXML = Resources.Load<VisualTreeAsset>("CharaSettingUXML");

        int rand = UnityEngine.Random.Range(1, 5);
        for (int i = 0; i < rand; i++)
        {
            VisualElement charaTree = new VisualElement();
            charaUXML.CloneTree(charaTree);
            charaTree.Q<Label>("charaName").text = "キャラ" + (i + 1);
            charaImageBox.Add(charaTree);
        }

        var charaEffectBox = root.Q<Box>("charaEffect");
        var charaEffectUXML = Resources.Load<VisualTreeAsset>("CharaSettingUXML");
        for (int i = 0; i < rand; i++)
        {
            VisualElement charaTree = new VisualElement();
            charaUXML.CloneTree(charaTree);
            charaTree.Q<Label>("charaName").text = "キャラ" + (i + 1);
            charaEffectBox.Add(charaTree);
        }

        //データバインド
        BindData(root, data);

        return root;
    }

    void CharaSetting(VisualElement root, SerializedProperty data)
    {

    }

    void BindData(VisualElement root, SerializedProperty data)
    {
        root.Bind(data.serializedObject);


        var nameElement = root.Q<TextField>("name");
        nameElement.BindProperty(data.FindPropertyRelative("Name"));

        var textElement = root.Q<TextField>("serihu");
        textElement.BindProperty(data.FindPropertyRelative("text"));


        //背景
        var howBack = root.Q<EnumField>("howBack");
        howBack.Init((BackChangeStyle)data.FindPropertyRelative("howBack").enumValueIndex);
        howBack.BindProperty(data.FindPropertyRelative("howBack"));


        var BackSprite = root.Q<ObjectField>("backSprite");
        BackSprite.BindProperty(data.FindPropertyRelative("back"));

        var backFadeColor = root.Q<ColorField>("backFadeColor");
        backFadeColor.BindProperty(data.FindPropertyRelative("backFadeColor"));

        var backFadeSpeed = root.Q<FloatField>("backFadeSpeed");
        backFadeSpeed.BindProperty(data.FindPropertyRelative("backFadeSpeed"));


        //フォント
        var changeFont = root.Q<Toggle>("changeFont");
        changeFont.BindProperty(data.FindPropertyRelative("changeFont"));

        var font = root.Q<ObjectField>("font");
        font.BindProperty(data.FindPropertyRelative("font"));

        var fontColor = root.Q<ColorField>("fontColor");
        fontColor.BindProperty(data.FindPropertyRelative("fontColor"));

        var Style = root.Q<MaskField>("Style");
        //Style.BindProperty(data.FindPropertyRelative("Style"));

        var fontSize = root.Q<IntegerField>("fontSize");
        fontSize.BindProperty(data.FindPropertyRelative("fontSize"));

        var nameFont = root.Q<ObjectField>("nameFont");
        nameFont.BindProperty(data.FindPropertyRelative("nameFont"));

        var nameColor = root.Q<ColorField>("nameColor");
        nameColor.BindProperty(data.FindPropertyRelative("nameColor"));

        var nameFontStyle = root.Q<MaskField>("nameFontStyle");
        //nameFontStyle.BindProperty(data.FindPropertyRelative("nameFontStyle"));


        //BGM
        var BGMStyle = root.Q<EnumField>("BGMStyle");
        BGMStyle.Init((SoundStyle)data.FindPropertyRelative("BGMStyle").enumValueIndex);
        BGMStyle.BindProperty(data.FindPropertyRelative("BGMStyle"));

        var BGM = root.Q<ObjectField>("BGM");
        BGM.BindProperty(data.FindPropertyRelative("BGM"));

        var BGMLoop = root.Q<EnumField>("BGMLoop");
        BGMLoop.Init((LoopMode)data.FindPropertyRelative("BGMLoop").enumValueIndex);
        BGMLoop.BindProperty(data.FindPropertyRelative("BGMLoop"));

        var BGMCount = root.Q<IntegerField>("BGMCount");
        BGMCount.BindProperty(data.FindPropertyRelative("BGMCount"));

        var BGMSecond = root.Q<FloatField>("BGMSecond");
        BGMSecond.BindProperty(data.FindPropertyRelative("BGMSecond"));

        var BGMFadeTime = root.Q<FloatField>("BGMFadeTime");
        BGMFadeTime.BindProperty(data.FindPropertyRelative("BGMFadeTime"));

        var BGMEndFadeTime = root.Q<FloatField>("BGMEndFadeTime");
        BGMEndFadeTime.BindProperty(data.FindPropertyRelative("BGMEndFadeTime"));


        //SE
        var SEStyle = root.Q<EnumField>("SEStyle");
        SEStyle.Init((SoundStyle)data.FindPropertyRelative("SEStyle").enumValueIndex);
        SEStyle.BindProperty(data.FindPropertyRelative("SEStyle"));

        var SE = root.Q<ObjectField>("SE");
        SE.BindProperty(data.FindPropertyRelative("SE"));

        var SELoop = root.Q<EnumField>("SELoop");
        SELoop.Init((LoopMode)data.FindPropertyRelative("SELoop").enumValueIndex);
        SELoop.BindProperty(data.FindPropertyRelative("SELoop"));

        var SECount = root.Q<IntegerField>("SECount");
        SECount.BindProperty(data.FindPropertyRelative("SECount"));

        var SESecond = root.Q<FloatField>("SESecond");
        SESecond.BindProperty(data.FindPropertyRelative("SESecond"));

        var SEFadeTime = root.Q<FloatField>("SEFadeTime");
        SEFadeTime.BindProperty(data.FindPropertyRelative("SEFadeTime"));

        var SEEndFadeTime = root.Q<FloatField>("SEEndFadeTime");
        SEEndFadeTime.BindProperty(data.FindPropertyRelative("SEEndFadeTime"));


        //エフェクト
        var backEffect = root.Q<EnumField>("backEffect");
        backEffect.Init((Effect)data.FindPropertyRelative("backEffect").enumValueIndex);
        backEffect.BindProperty(data.FindPropertyRelative("backEffect"));

        var backEffectStrength = root.Q<SliderInt>("backEffectStrength");
        backEffectStrength.BindProperty(data.FindPropertyRelative("backEffectStrength"));

        var FrontEffect = root.Q<EnumField>("FrontEffect");
        FrontEffect.Init((Effect)data.FindPropertyRelative("FrontEffect").enumValueIndex);
        FrontEffect.BindProperty(data.FindPropertyRelative("FrontEffect"));

        var FrontEffectStrength = root.Q<SliderInt>("FrontEffectStrength");
        FrontEffectStrength.BindProperty(data.FindPropertyRelative("FrontEffectStrength"));

        var AllEffect = root.Q<EnumField>("AllEffect");
        AllEffect.Init((Effect)data.FindPropertyRelative("AllEffect").enumValueIndex);
        AllEffect.BindProperty(data.FindPropertyRelative("AllEffect"));

        var AllEffectStrength = root.Q<SliderInt>("AllEffectStrength");
        AllEffectStrength.BindProperty(data.FindPropertyRelative("AllEffectStrength"));
    }
}