using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonTextDisplay : MonoBehaviour
{
    [SerializeField] Text text;
    public void DisplayStringOnHover(string textToDisplay)
    {
        text.text = textToDisplay;
    }
}
