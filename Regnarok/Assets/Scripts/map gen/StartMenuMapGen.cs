using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StartMenuMapGen : MonoBehaviour
{
	public MapGenerator mg;
	private int seed;
	private void Start()
	{
		seed = PlayerPrefs.GetInt("Seed");
		mg.StartGenerating(seed);
	}
}
