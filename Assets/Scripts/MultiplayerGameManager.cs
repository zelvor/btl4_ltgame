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

    public bool isPlayer1Turn;

    public bool isPlayer1;

    public bool gameOver = false;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            photonView.RPC("InitBoard", RpcTarget.All);
            photonView.RPC("NewBoard", RpcTarget.All);
        }

    }

    [PunRPC]
    private void NewBoard()
    {
        boardState = new int[row_count, column_count];
        objects = new GameObject[row_count, column_count];
        isPlayer1Turn = true;
        gameOver = false;
        player1Ghost.SetActive(false);
        player1Ghost.transform.position = spawnLocations[3].transform.position;
        player2Ghost.SetActive(false);

        //master client, isPlayer1Turn = true
        if (PhotonNetwork.IsMasterClient)
        {
            isPlayer1 = true;
        }
        else
        {
            isPlayer1 = false;
        }
    }

    private void player1TurnHover(int column){
        player1Ghost.SetActive(true);
        player1Ghost.transform.position = spawnLocations[column].transform.position;
    }

    private void player2TurnHover(int column){
        player2Ghost.SetActive(true);
        player2Ghost.transform.position = spawnLocations[column].transform.position;
    }

    public void hoverColumn(int column)
    {
        if (
            boardState[0, column] == 0 &&
            !gameOver &&
            (
            fallingPiece == null ||
            fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero
            )
        )
        {
            if (isPlayer1 && isPlayer1Turn){
                player1TurnHover(column);
            } else if (!isPlayer1 && !isPlayer1Turn){
                player2TurnHover(column);
            }
        }
    }

    private void player1TurnSelect(int column){
        player1Ghost.SetActive(false);
        fallingPiece = PhotonNetwork.Instantiate("Player1", spawnLocations[column].transform.position, Quaternion.identity);
        fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
        objects[lastRow, column] = fallingPiece;
        isPlayer1Turn = false;
    }

    private void player2TurnSelect(int column){
        player2Ghost.SetActive(false);
        fallingPiece = PhotonNetwork.Instantiate("Player2", spawnLocations[column].transform.position, Quaternion.identity);
        fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
        objects[lastRow, column] = fallingPiece;
        isPlayer1Turn = true;
    }

    public void selectColumn(int column){
        if (isPlayer1 && isPlayer1Turn){
            player1TurnSelect(column);
        } else if (!isPlayer1 && !isPlayer1Turn){
            player2TurnSelect(column);
        }
    }


    [PunRPC]
    private void InitBoard()
    {
        PhotonNetwork
            .Instantiate("Board",
            new Vector3(0.524f, 0.411f, 0),
            Quaternion.identity);
    }

}
