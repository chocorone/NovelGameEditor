using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NovelEditor;
using TMPro;

namespace NovelEditor.Sample
{
    public class ValueManager : MonoBehaviour
    {
        [SerializeField] NovelPlayer player;
        [SerializeField] Slider BGMslider;
        [SerializeField] Slider SEslider;
        [SerializeField] Slider Textslider;
        [SerializeField] Toggle muteToggle;
        [SerializeField] GameObject panel;
        [SerializeField] TextMeshProUGUI text;
        // Start is called before the first frame update
        void Start()
        {
            text.text = "";
            BGMslider.onValueChanged.AddListener((value) => { player.BGMVolume = value; });
            SEslider.onValueChanged.AddListener((value) => { player.SEVolume = value; });
            Textslider.onValueChanged.AddListener((value) => { player.textSpeed = (int)value; });
            muteToggle.onValueChanged.AddListener((value) => { player.mute = value; });
            player.OnBegin += () => { Debug.Log("begin"); };
            player.OnEnd += () => { Debug.Log("end"); };
            player.OnChoiced += (name) => { Debug.Log(name); };
            player.OnSkiped += () => { text.text = ""; };
            player.OnLoad += () => { text.text = ""; };
            player.OnBegin += () => { text.text = ""; };
            player.OnDialogueChanged += (data) => { text.text += data.Name + "　　<indent=15%> 「" + data.text + "」</indent>\n\n"; };
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (panel.activeSelf)
                {
                    player.UnPause();
                    panel.SetActive(false);

                }
                else
                {
                    player.Pause();
                    panel.SetActive(true);
                }
            }
        }
    }

}
