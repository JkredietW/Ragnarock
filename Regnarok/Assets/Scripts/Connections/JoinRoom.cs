using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class JoinRoom : MonoBehaviour
{
    public string roomName;
    [SerializeField] TextMeshProUGUI textObject;

    public void GiveName()
    {
        textObject.text = roomName;
    }
    public void JoinRoompie()
    {
        if (roomName.Length > 3 && PhotonNetwork.NickName.Length > 3)
        {
            PhotonNetwork.JoinRoom(roomName);
            MenuManager.menuSwitch.ChangeMenu("Host");
        }
    }
}
