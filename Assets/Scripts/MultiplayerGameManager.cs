using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameManager : MonoBehaviour
{
    //photon view
    private PhotonView photonView;

    public int[,] boardState; // 0 - empty, 1 - player1, 2 - player2

    private GameObject[,] objects;

    private int lastRow = 0;

    public const int row_count = 6;

    public const int column_count = 7;
    public GameObject player1;

    public GameObject player2;

    public Material

            color1,
            winColor1,
            color2,
            winColor2;

    public GameObject player1Ghost;

    public GameObject player2Ghost;

    public GameObject fallingPiece;


    public GameObject board;

    public GameObject[] spawnLocations;

    public bool isPlayer1Turn = true;

    public bool gameOver = false;


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //PhotonNetwork.Instantiate() can only instantiate objects with a PhotonView component. This prefab does not have one: Board
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
           photonView.RPC("InitBoard", RpcTarget.All);
        }
    }

    [PunRPC]
    private void InitBoard(){
        //setActive board
        board.SetActive(true);
    }
}
