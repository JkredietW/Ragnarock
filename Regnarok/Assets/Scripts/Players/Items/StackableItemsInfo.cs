using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StackableItemsInfo : MonoBehaviour
{
    public string itemName;
    [TextArea]
    public string itemDestription;

    public TextMeshProUGUI nameObject, description, otherOne;

    //info stackable items
    public void On()
    {
        nameObject.text = itemName;
        description.text = itemDestription;
        otherOne.text = string.Empty;
    }
    public void Off()
    {
        nameObject.text = string.Empty;
        description.text = string.Empty;
    }
}
