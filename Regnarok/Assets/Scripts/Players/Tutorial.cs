using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI objectiveTextObject;
    [TextArea]
    public List<string> objectivesText;
    int currentObjective;

    //first
    int stickAmount, flintAmount;

    //third
    int woodAmount;

    private void Start()
    {
        NextObjective();
    }
    void NextObjective()
    {
        objectiveTextObject.text = objectivesText[currentObjective];
        currentObjective++;
    }
    private void Update()
    {
        if (currentObjective == 5)
        {
            if(Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.T))
            {
                NextObjective();
                Invoke("CloseTutorial", 10);
            }
        }
        if (Input.GetKey(KeyCode.X))
        {
            CloseTutorial();
        }
    }
    void CloseTutorial()
    {
        objectiveTextObject.transform.parent.gameObject.SetActive(false);
        //done, close tutorial tab
    }
    //sticks and flint
    public void FirstObjective(int stick, int flint)
    {
        if (currentObjective == 1)
        {
            stickAmount += stick;
            flintAmount += flint;
            if (stickAmount > 1 && flintAmount > 0)
            {
                NextObjective();
            }
        }
    }
    public void SecondObjective(int flintAxe)
    {
        if (currentObjective == 2)
        {
            if (flintAxe > 0)
            {
                NextObjective();
            }
        }
    }
    public void ThirdObjective(int oakWood)
    {
        if (currentObjective == 3)
        {
            woodAmount += oakWood;
            if (woodAmount > 4)
            {
                NextObjective();
            }
        }
    }
    public void FourthObjective(int craftingStation)
    {
        if (currentObjective == 4)
        {
            if (craftingStation > 0)
            {
                NextObjective();
            }
        }
    }
}
