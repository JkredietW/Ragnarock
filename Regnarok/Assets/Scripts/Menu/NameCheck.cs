using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameCheck : MonoBehaviour
{
    public Color ActiveColor, grayedOut;

    bool nameGiven, roomNameGiven;

    Image thisColor;

    public bool bothNeeded;

    private void Start()
    {
        thisColor = GetComponent<Image>();
        ColorCheck();
    }

    public void CheckName(string _givenName)
    {
        nameGiven = _givenName.Length > 2 ? true : false;
        ColorCheck();
    }
    public void CheckRoomName(string _givenName)
    {
        roomNameGiven = _givenName.Length > 2 ? true : false;
        ColorCheck();
    }
    void ColorCheck()
    {
        if (bothNeeded)
        {
            if (nameGiven && roomNameGiven)
            {
                thisColor.color = ActiveColor;
                GetComponent<Button>().interactable = true;
            }
            else
            {
                thisColor.color = grayedOut;
                GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            if (nameGiven)
            {
                thisColor.color = ActiveColor;
                GetComponent<Button>().interactable = true;
            }
            else
            {
                thisColor.color = grayedOut;
                GetComponent<Button>().interactable = false;
            }
        }
    }
}
