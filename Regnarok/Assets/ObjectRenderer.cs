using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ObjectRenderer : MonoBehaviour
{
	public LayerMask objectsLayer;
	private void OnCollisionEnter(Collision collision)
	{
		print(collision.transform.name);
		if (collision.transform.gameObject.layer == objectsLayer)
		{
			collision.transform.gameObject.transform.GetChild(0).gameObject.SetActive(true);
		}
	}
	private void OnCollisionExit(Collision collision)
	{
		print(collision.transform.name);
		if (collision.transform.gameObject.layer == objectsLayer)
		{
			collision.transform.gameObject.transform.GetChild(0).gameObject.SetActive(false);
		}
	}
}