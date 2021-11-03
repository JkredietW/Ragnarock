using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemieList", menuName = "ScriptableObjects/EnemieList", order = 1)]
public class EnemyList : ScriptableObject
{
    public List<string> enemieList;
}
