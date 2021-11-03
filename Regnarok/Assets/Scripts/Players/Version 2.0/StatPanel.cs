using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplays;
    [SerializeField] string[] statNames;

    private CharacterStats stats;

    private void OnValidate()
    {
        statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
        UpdateStatValues();
    }
    //must still get list of stats
    public void UpdateStatValues()
    {
        for (int i = 0; i < 0; i++)
        {
            //statDisplays[i].valueText.text = list of stat[i]
        }
    }
    public void UpdateStatNames()
    {
        for (int i = 0; i < 0; i++)
        {
            //statDisplays[i].nameText.text = list of stat[i]
        }
    }
}
