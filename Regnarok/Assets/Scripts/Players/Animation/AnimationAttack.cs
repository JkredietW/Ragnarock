using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttack : MonoBehaviour
{
    public void DoneAttack()
    {
        print(1);
        //GetComponentInParent<PlayerController>().AttackStuckFix2();
    }
    public void DoDamage()
    {
        GetComponentInParent<PlayerController>().Attack();
    }
}
