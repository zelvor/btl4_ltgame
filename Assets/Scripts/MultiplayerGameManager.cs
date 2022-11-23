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

    public bool isPlayer1 = false;

    public bool isPlayer2 = false;

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
        player1Ghost.SetActive(true);
        player1Ghost.transform.position = spawnLocations[3].transform.position;
        player2Ghost.SetActive(false);

        //master client, isPlayer1Turn = true
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master client");
            isPlayer1 = true;
            isPlayer2 = false;
        }
        else
        {
            Debug.Log("Not master client");
            isPlayer2 = true;
            isPlayer1 = false;
        }

        Debug.Log("isPlayer1: " + isPlayer1);
        Debug.Log("isPlayer2: " + isPlayer2);
    }

    private void player1TurnHover(int column)
    {
        player1Ghost.SetActive(true);
        player1Ghost.transform.position =
            spawnLocations[column].transform.position;
    }

    private void player2TurnHover(int column)
    {
        player2Ghost.SetActive(true);
        player2Ghost.transform.position =
            spawnLocations[column].transform.position;
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
            if (isPlayer1 && isPlayer1Turn)
            {
                player1TurnHover (column);
            }
            else if (isPlayer2 && !isPlayer1Turn)
            {
                player2TurnHover (column);
            }
        }
    }

    private void player1TurnSelect(int column)
    {
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        fallingPiece =
            PhotonNetwork
                .Instantiate("Player1",
                spawnLocations[column].transform.position,
                Quaternion.Euler(new Vector3(90, 0, 0)));
        fallingPiece.GetComponent<Rigidbody>().velocity =
            new Vector3(0, 0.1f, 0);
        objects[lastRow, column] = fallingPiece;
        photonView.RPC("checkGameOver", RpcTarget.All);
    }

    private void player2TurnSelect(int column)
    {
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        fallingPiece =
            PhotonNetwork
                .Instantiate("Player2",
                spawnLocations[column].transform.position,
                Quaternion.Euler(new Vector3(90, 0, 0)));
        fallingPiece.GetComponent<Rigidbody>().velocity =
            new Vector3(0, 0.1f, 0);
        objects[lastRow, column] = fallingPiece;
        photonView.RPC("checkGameOver", RpcTarget.All);
    }

    public void selectColumn(int column)
    {
        if (
            !gameOver &&
            (
            fallingPiece == null ||
            fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero
            )
        )
        {
            if (updateBoard(column))
            {
                Debug.Log("IsPlayer1Turn: " + isPlayer1Turn);
                if (isPlayer1 && isPlayer1Turn)
                {
                    player1TurnSelect (column);
                    photonView.RPC("SwitchTurn", RpcTarget.All);
                }
                else if (isPlayer2 && !isPlayer1Turn)
                {
                    player2TurnSelect (column);
                    photonView.RPC("SwitchTurn", RpcTarget.All);
                }
            }
        }
    }

    bool isDraw(int[,] board)
    {
        for (int i = 0; i < column_count; i++)
        if (board[0, i] == 0) return false;
        return true;
    }
    bool updateBoard(int column)
    {
        for (int i = row_count - 1; i >= 0; i--)
        {
            if (boardState[i, column] == 0)
            {
                boardState[i, column] = isPlayer1Turn ? 1 : 2;
                lastRow = i;
                Debug.Log("Piece fitted in (" + i + ", " + column + ")");
                return true;
            }
        }
        Debug.LogWarning("Column " + column + " is full!");
        return false;
    }

    bool winningMove(int[,] board, int player, bool realWin)
    {
        //horizontal
        for (int c = 0; c < column_count - 3; c++)
        for (int r = 0; r < row_count; r++)
        if (
            board[r, c] == player &&
            board[r, c + 1] == player &&
            board[r, c + 2] == player &&
            board[r, c + 3] == player
        )
        {
            return true;
        }

        //vertical
        for (int c = 0; c < column_count; c++)
        for (int r = 0; r < row_count - 3; r++)
        if (
            board[r, c] == player &&
            board[r + 1, c] == player &&
            board[r + 2, c] == player &&
            board[r + 3, c] == player
        )
        {
            return true;
        }

        // y=x
        for (int c = 0; c < column_count - 3; c++)
        for (int r = 0; r < row_count - 3; r++)
        if (
            board[r, c] == player &&
            board[r + 1, c + 1] == player &&
            board[r + 2, c + 2] == player &&
            board[r + 3, c + 3] == player
        )
        {
            return true;
        }

        // y=-x
        for (int c = 0; c < column_count - 3; c++)
        for (int r = 3; r < row_count; r++)
        if (
            board[r, c] == player &&
            board[r - 1, c + 1] == player &&
            board[r - 2, c + 2] == player &&
            board[r - 3, c + 3] == player
        )
        {
            return true;
        }
        return false;
    }

    [PunRPC]
    private void InitBoard()
    {
        PhotonNetwork
            .Instantiate("Board",
            new Vector3(0.524f, 0.411f, 0),
            Quaternion.identity);
    }

    [PunRPC]
    private void SwitchTurn()
    {
        isPlayer1Turn = !isPlayer1Turn;
        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
    }

    [PunRPC]
    private void checkGameOver()
    {
        if (winningMove(boardState, 1, true) || winningMove(boardState, 2, true)
        )
        {
            gameOver = true;
        }
        else if (isDraw(boardState))
        {
            gameOver = true;
        }
    }
}
