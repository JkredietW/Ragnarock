using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemListScript", menuName = "ScriptableObjects/ItemListScript", order = 1)]
public class ItemListScript : ScriptableObject
{
    public List<Objects> common;
    public List<Objects> rare;
    public List<Objects> epic;
    public List<Objects> legendary;
    public List<Objects> mythic;

    [System.Serializable]
    public struct Objects
    {
        public string name;
    }
}
