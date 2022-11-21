using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Temp : MonoBehaviour
{
    [SerializeField]
    private Text text;

    private PhotonView photonView;

    //if 2 players are in the room, disable the text
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            if (photonView.IsMine)
            {
                text.text = "1";
            }
            else
            {
                text.text = "2";
            }
        }
    }
}
