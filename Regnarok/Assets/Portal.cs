using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PhotonView>().IsMine)
		{
			PhotonNetwork.LeaveRoom();
			SceneManager.LoadScene(0);
		}
	}
}
