using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractEnviorment : MonoBehaviour
{
    public ProbScript item;
    public void Interact()
    {
        item.Interaction();
    }
}
