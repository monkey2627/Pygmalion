using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay
{
    public class Page : MonoBehaviour
    {
        public Paragraph paragraph;
        public List<Word> words;
        public FlowLayoutGroupCentered layout;

        private void OnEnable()
        {
            foreach (var word in words)
            {
                word.GetComponent<Word>().RefreshBox2d();
            }
            layout.Refresh();
        }

        public void Fade(float time)
        {
            foreach (Word word in words)
            {
                word.Fade(time);
            }
        }
        public void ClosePanel()
        {
            foreach (Word word in words)
            {
                word.Close();
            }
        }
    }
}
