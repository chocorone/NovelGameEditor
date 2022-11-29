using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NovelEditor.Sample
{
    public class SaveSample : MonoBehaviour
    {


        [SerializeField] NovelPlayer player;
        NovelSaveData data;
        bool saved = false;

        public void Save()
        {
            data = player.save();
            saved = true;
        }

        public void Load()
        {
            if (saved)
                player.Load(data, true);
        }

    }

}