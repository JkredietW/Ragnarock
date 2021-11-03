using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public PhotonView pv;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SpawnPlayer(Vector3 position)
    {
        GameObject game = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Character"), position, Quaternion.identity);
        game.SetActive(true);
    }
}
