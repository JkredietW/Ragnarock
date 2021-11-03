using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;

    private void OnValidate()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        nameText = texts[0];
        valueText = texts[1];
    }
    private void Start()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        nameText = texts[0];
        valueText = texts[1];
    }
    public void GiveStats(string _value)
    {
        valueText.text = _value;
    }
}
