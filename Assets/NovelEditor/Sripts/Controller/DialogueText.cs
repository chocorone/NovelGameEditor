using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NovelData.ParagraphData;

public class DialogueText : MonoBehaviour
{
    public void textUpdate(Dialogue data)
    {
        Debug.Log(data.text);
    }

    void PlayText(string text)
    {

    }
}
