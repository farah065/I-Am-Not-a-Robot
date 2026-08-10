using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public void ChangeButtonColour()
    {
        Button button = GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = Color.red;
        button.colors = cb;
    }
}
