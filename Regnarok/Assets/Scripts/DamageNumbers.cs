using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class DamageNumbers : MonoBehaviour
{
    public float damageAmount;
    public GameObject mainObject;

    //privates
    public TextMeshProUGUI textMesh;
    private float disappearTime;
    private Color textColor;
    public List<GameObject> players;
    private Transform target;

    private void Start()
    {
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i].GetComponent<PhotonView>().IsMine)
			{
                target = players[i].transform;
            }
		}
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTime = 1f;
    }
    void Update()
    {
		if (!target )
		{
            return;
		}
        transform.LookAt(target);
        transform.rotation = Quaternion.LookRotation(target.forward);
        float moveYSpeed = 4;
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        disappearTime -= Time.deltaTime;
        if (disappearTime < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(mainObject);
            }
        }
    }
}