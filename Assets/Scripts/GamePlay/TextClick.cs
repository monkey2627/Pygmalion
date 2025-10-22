using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay
{
    public class TextClick: MonoBehaviour,IPointerClickHandler
    {
        public Word word;
        public void OnPointerClick(PointerEventData eventData)
        {
            word.wordText.text = GetComponent<TMP_Text>().text;
            if (word.wordType == 1)
            {
                if (word.wordText.text == "<color=#00000000>空</color>")
                {
                    word.spaceYellow.SetActive(true);
                }
                else
                {
                    word.spaceYellow.SetActive(false);
                }
            }
            word.RefreshBox2d();
            word.sentence.layout.GetComponent<FlowLayoutGroupCentered>().Refresh();
        }
    }
}