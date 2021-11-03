using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Health>())
        {
          
        }
    }
}
