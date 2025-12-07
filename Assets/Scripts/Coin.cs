using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public string Word;
    [SerializeField] private TMP_Text _wordText;

    public void SetWord(string word)
    {
        Word = word;
        _wordText.text = word;
    }

    public void HighlightPrefix(int length)
    {
        if (string.IsNullOrEmpty(Word))
            return;

        length = Mathf.Clamp(length, 0, Word.Length);

        string prefix = Word.Substring(0, length);
        string suffix = Word.Substring(length);

        // Change this color to whatever highlight you want
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _wordText.text = coloredPrefix + suffix;
    }

    public void ResetHighlight()
    {
        _wordText.text = Word;
    }
}
