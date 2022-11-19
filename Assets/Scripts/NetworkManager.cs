using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Connected to {PhotonNetwork.CloudRegion} server");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join a room. Creating a new one. {message}");
        PhotonNetwork.CreateRoom(null);
    }

    public override void OnJoinedRoom()
    {
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.NickName} joined room {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogError($"Player {newPlayer.NickName} joined room {PhotonNetwork.CurrentRoom.Name}");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    #endregion
}
