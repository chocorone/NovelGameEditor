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

        CharaSetting(root, data);

        //データバインド
        BindData(root, data);

        SetUpUIByValue(root, data);

        return root;
    }

    void CharaSetting(VisualElement root, SerializedProperty data)
    {
        //立ち絵の数に応じて作成
        var charaImageBox = root.Q<Box>("charaImage");
        var charaUXML = Resources.Load<VisualTreeAsset>("CharaSettingUXML");

        int charaNum = NovelEditorWindow.editingData.locations.Count;
        for (int i = 0; i < charaNum; i++)
        {
            VisualElement charaTree = new VisualElement();
            charaUXML.CloneTree(charaTree);
            charaTree.Q<EnumField>().label = NovelEditorWindow.editingData.locations[i].name;
            //charaTree.Q<EnumField>().BindProperty(data.FindPropertyRelative("CharaChangeStyle").GetArrayElementAtIndex(i));
            //charaTree.Q<ObjectField>().BindProperty(data.FindPropertyRelative("charas").GetArrayElementAtIndex(i));
            charaImageBox.Add(charaTree);
        }

        var charaEffectBox = root.Q<Box>("charaEffect");
        var charaEffectUXML = Resources.Load<VisualTreeAsset>("CharaSettingUXML");
        for (int i = 0; i < charaNum; i++)
        {
            VisualElement charaTree = new VisualElement();
            charaUXML.CloneTree(charaTree);
            charaTree.Q<EnumField>().label = "キャラ" + (i + 1);
            charaEffectBox.Add(charaTree);
        }
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

        var fontSize = root.Q<IntegerField>("fontSize");
        fontSize.BindProperty(data.FindPropertyRelative("fontSize"));

        var changeNameFont = root.Q<Toggle>("changeNameFont");
        changeNameFont.BindProperty(data.FindPropertyRelative("changeNameFont"));

        var nameFont = root.Q<ObjectField>("nameFont");
        nameFont.BindProperty(data.FindPropertyRelative("nameFont"));

        var nameColor = root.Q<ColorField>("nameColor");
        nameColor.BindProperty(data.FindPropertyRelative("nameColor"));


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

    void SetUpUIByValue(VisualElement root, SerializedProperty data)
    {
        //背景設定
        var howBack = root.Q<EnumField>("howBack");
        howBack.RegisterValueChangedCallback(x =>
        {
            BackChangeStyle value = (BackChangeStyle)data.FindPropertyRelative("howBack").enumValueIndex;

            var changeBackBox = root.Q<Box>("changeBackBox");
            var backSprite = root.Q<ObjectField>("backSprite");

            changeBackBox.style.display = DisplayStyle.None;
            backSprite.style.display = DisplayStyle.None;

            if (value != BackChangeStyle.UnChange)
            {
                backSprite.style.display = DisplayStyle.Flex;
                if (value != BackChangeStyle.Quick && value != BackChangeStyle.dissolve)
                    changeBackBox.style.display = DisplayStyle.Flex;
            }
        });

        //フォント設定
        var changeFont = root.Q<Toggle>("changeFont");
        changeFont.RegisterValueChangedCallback(x =>
        {

            var changeFontBox = root.Q<Box>("changeFontBox");

            bool flag = data.FindPropertyRelative("changeFont").boolValue;

            changeFontBox.style.display = flag ? DisplayStyle.Flex : DisplayStyle.None;
        });

        var changeNameFont = root.Q<Toggle>("changeNameFont");
        changeNameFont.RegisterValueChangedCallback(x =>
        {
            var changeNameFontBox = root.Q<Box>("changeNameFontBox");

            bool flag = data.FindPropertyRelative("changeNameFont").boolValue;

            changeNameFontBox.style.display = flag ? DisplayStyle.Flex : DisplayStyle.None;
        });

        //サウンド設定
        var BGMStyle = root.Q<EnumField>("BGMStyle");
        BGMStyle.RegisterValueChangedCallback(x =>
        {
            SoundStyle PlayStyleValue = (SoundStyle)data.FindPropertyRelative("BGMStyle").enumValueIndex;

            var BGMPlayBox = root.Q<Box>("BGMPlayBox");
            if (PlayStyleValue == SoundStyle.Play)
            {
                BGMPlayBox.style.display = DisplayStyle.Flex;
            }
            else
            {
                BGMPlayBox.style.display = DisplayStyle.None;
            }

        });

        var BGMLoop = root.Q<EnumField>("BGMLoop");
        BGMLoop.RegisterValueChangedCallback(x =>
        {
            var LoopStyleValue = (LoopMode)data.FindPropertyRelative("BGMLoop").enumValueIndex;
            var BGMEndFadeTime = root.Q<FloatField>("BGMEndFadeTime");
            var BGMCount = root.Q<IntegerField>("BGMCount");
            var BGMSecond = root.Q<FloatField>("BGMSecond");


            switch (LoopStyleValue)
            {
                case LoopMode.Endless:
                    BGMEndFadeTime.style.display = DisplayStyle.None;
                    BGMCount.style.display = DisplayStyle.None;
                    BGMSecond.style.display = DisplayStyle.None;
                    break;

                case LoopMode.Count:
                    BGMEndFadeTime.style.display = DisplayStyle.Flex;
                    BGMCount.style.display = DisplayStyle.Flex;
                    BGMSecond.style.display = DisplayStyle.None;
                    break;

                case LoopMode.Second:
                    BGMEndFadeTime.style.display = DisplayStyle.Flex;
                    BGMCount.style.display = DisplayStyle.None;
                    BGMSecond.style.display = DisplayStyle.Flex;
                    break;
            }
        });

        var SEStyle = root.Q<EnumField>("SEStyle");
        SEStyle.RegisterValueChangedCallback(x =>
        {
            SoundStyle PlayStyleValue = (SoundStyle)data.FindPropertyRelative("SEStyle").enumValueIndex;

            var SEPlayBox = root.Q<Box>("SEPlayBox");
            if (PlayStyleValue == SoundStyle.Play)
            {
                SEPlayBox.style.display = DisplayStyle.Flex;
            }
            else
            {
                SEPlayBox.style.display = DisplayStyle.None;
            }

        });

        var SELoop = root.Q<EnumField>("SELoop");
        SELoop.RegisterValueChangedCallback(x =>
        {
            var LoopStyleValue = (LoopMode)data.FindPropertyRelative("SELoop").enumValueIndex;
            var SEEndFadeTime = root.Q<FloatField>("SEEndFadeTime");
            var SECount = root.Q<IntegerField>("SECount");
            var SESecond = root.Q<FloatField>("SESecond");


            switch (LoopStyleValue)
            {
                case LoopMode.Endless:
                    SEEndFadeTime.style.display = DisplayStyle.None;
                    SECount.style.display = DisplayStyle.None;
                    SESecond.style.display = DisplayStyle.None;
                    break;

                case LoopMode.Count:
                    SEEndFadeTime.style.display = DisplayStyle.Flex;
                    SECount.style.display = DisplayStyle.Flex;
                    SESecond.style.display = DisplayStyle.None;
                    break;

                case LoopMode.Second:
                    SEEndFadeTime.style.display = DisplayStyle.Flex;
                    SECount.style.display = DisplayStyle.None;
                    SESecond.style.display = DisplayStyle.Flex;
                    break;
            }
        });

        //エフェクト設定
        var backEffect = root.Q<EnumField>("backEffect");
        backEffect.RegisterValueChangedCallback(x =>
        {

            var value = (Effect)data.FindPropertyRelative("backEffect").enumValueIndex;
            var backEffectStrength = root.Q<SliderInt>("backEffectStrength");
            if (value == Effect.None || value == Effect.UnChange)
            {
                backEffectStrength.style.display = DisplayStyle.None;
            }
            else
            {
                backEffectStrength.style.display = DisplayStyle.Flex;
            }
        });

        var FrontEffect = root.Q<EnumField>("FrontEffect");
        FrontEffect.RegisterValueChangedCallback(x =>
        {

            var value = (Effect)data.FindPropertyRelative("FrontEffect").enumValueIndex;
            var FrontEffectStrength = root.Q<SliderInt>("FrontEffectStrength");
            if (value == Effect.None || value == Effect.UnChange)
            {
                FrontEffectStrength.style.display = DisplayStyle.None;
            }
            else
            {
                FrontEffectStrength.style.display = DisplayStyle.Flex;
            }
        });

        var AllEffect = root.Q<EnumField>("AllEffect");
        AllEffect.RegisterValueChangedCallback(x =>
        {

            var value = (Effect)data.FindPropertyRelative("AllEffect").enumValueIndex;
            var AllEffectStrength = root.Q<SliderInt>("AllEffectStrength");
            if (value == Effect.None || value == Effect.UnChange)
            {
                AllEffectStrength.style.display = DisplayStyle.None;
            }
            else
            {
                AllEffectStrength.style.display = DisplayStyle.Flex;
            }
        });
    }
}