using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GamePlay
{
    public class TextClick: MonoBehaviour,IPointerClickHandler
    {
        public TMP_Text text;
        public Word word;
        public void OnPointerClick(PointerEventData eventData)
        {
            word.wordText.text = GetComponent<TMP_Text>().text;
            if (word.wordType == 1)
            {
                word.spaceYellow.SetActive(word.wordText.text == "<color=#00000000>空</color>");
            }
            word.RefreshBox2d();
            word.Close();
            word.page.layout.GetComponent<FlowLayoutGroupCentered>().Refresh();
            PygmalionGameManager.Instance.upperButtons.SetActive(true);
        }
    }
}