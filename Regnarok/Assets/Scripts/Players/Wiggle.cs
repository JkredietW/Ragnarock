using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Wiggle : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void StopWiggle()
    {
        anim.SetInteger("State", 0);
    }
    public void StartWiggle()
    {
        anim.SetInteger("State", 1);
    }
}
