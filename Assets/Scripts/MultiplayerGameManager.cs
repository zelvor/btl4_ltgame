using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MultiplayerGameManager : MonoBehaviour
{
    public bool selectedPlayer1 = true;
    public int difficulty = 0; // 0 - easy, 1- medium,  2 - hard

    public bool playwithAI = false;
    
    public string debugMessage;
    public bool player1Turn = true;
    public bool gameOver = false;
    public int[,] boardState; // 0 - empty, 1 - player1, 2 - player2
    private GameObject[,] objects;
    private int lastRow = 0;
    public const int row_count = 6;
    public const int column_count = 7;

    public GameObject player1;
    public GameObject player2;

    public Material color1, winColor1, color2, winColor2;

    public GameObject player1Ghost;
    public GameObject player2Ghost;

    public GameObject fallingPiece;

    public GameObject[] spawnLocations;


    void Start()
    {
        selectedPlayer1 = GameInfo.getSelectedPlayer1();
        boardState = new int[row_count, column_count];
        player1Turn = true;
        gameOver = false;
        objects = new GameObject[row_count, column_count];
        player2Ghost.SetActive(false);
        player1Ghost.transform.position = spawnLocations[3].transform.position;
        player1Ghost.SetActive(true);
    }

    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void exitToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    
    public void hoverColumn(int column)
    {
        if(boardState[0,column] == 0 && !gameOver && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero))
        {
            if (player1Turn)
            {
                player1Ghost.SetActive(true);
                player1Ghost.transform.position = spawnLocations[column].transform.position;
            }
            else
            {
                player2Ghost.SetActive(true);
                player2Ghost.transform.position = spawnLocations[column].transform.position;
            }
        }
    }

    public void selectColumn(int column)
    {
        if(!gameOver && (fallingPiece == null || fallingPiece.GetComponent<Rigidbody>().velocity == Vector3.zero))
        {
            Debug.Log("GameManager column: " + column);
            TakeTurn(column);
        }
    }

    
    bool winningMove(int[,] board, int player, bool realWin)
    {

        //horizontala
        for (int c = 0; c < column_count - 3; c++)
            for (int r = 0; r < row_count; r++)
                if (board[r, c] == player && board[r, c + 1] == player && board[r, c + 2] == player && board[r, c + 3] == player)
                {
                    if (realWin)
                    {
                        StartCoroutine(BlinkGameObject(objects[r, c]));
                        StartCoroutine(BlinkGameObject(objects[r, c + 1]));
                        StartCoroutine(BlinkGameObject(objects[r, c + 2]));
                        StartCoroutine(BlinkGameObject(objects[r, c + 3]));
                    }
                    return true;
                }
                    
        //vertikala
        for (int c = 0; c < column_count ; c++)
            for (int r = 0; r < row_count - 3; r++)
                if (board[r, c] == player && board[r + 1, c] == player && board[r + 2, c] == player && board[r + 3, c] == player)
                {
                    if (realWin)
                    {
                        StartCoroutine(BlinkGameObject(objects[r, c]));
                        StartCoroutine(BlinkGameObject(objects[r + 1, c]));
                        StartCoroutine(BlinkGameObject(objects[r + 2, c]));
                        StartCoroutine(BlinkGameObject(objects[r + 3, c]));
                    }
                    return true;
                }

        //dijagonala y=x
        for (int c = 0; c < column_count - 3; c++)
            for (int r = 0; r < row_count - 3; r++)
                if (board[r, c] == player && board[r + 1, c + 1] == player && board[r + 2, c + 2] == player && board[r + 3, c + 3] == player)
                {
                    if (realWin)
                    {
                        StartCoroutine(BlinkGameObject(objects[r, c]));
                        StartCoroutine(BlinkGameObject(objects[r + 1, c + 1]));
                        StartCoroutine(BlinkGameObject(objects[r + 2, c + 2]));
                        StartCoroutine(BlinkGameObject(objects[r + 3, c + 3]));
                    }
                    return true;
                }

        //dijagonala y=-x
        for (int c = 0; c < column_count - 3; c++)
            for (int r = 3; r < row_count; r++)
                if (board[r, c] == player && board[r - 1, c + 1] == player && board[r - 2, c + 2] == player && board[r - 3, c + 3] == player)
                {
                    if (realWin)
                    {
                        StartCoroutine(BlinkGameObject(objects[r, c]));
                        StartCoroutine(BlinkGameObject(objects[r - 1, c + 1]));
                        StartCoroutine(BlinkGameObject(objects[r - 2, c + 2]));
                        StartCoroutine(BlinkGameObject(objects[r - 3, c + 3]));
                    }
                    return true;
                }

        return false;
    }


    void TakeTurn(int column)
    {
        if (updateBoard(column))
        {
            player1Ghost.SetActive(false);
            player2Ghost.SetActive(false);

            fallingPiece = Instantiate(player1Turn ? player1 : player2, spawnLocations[column].transform.position, Quaternion.Euler(new Vector3(90, 0, 0)));
            fallingPiece.GetComponent<Rigidbody>().velocity = new Vector3(0, 0.1f, 0);
            objects[lastRow, column] = fallingPiece;
            gameOver = winningMove(boardState, player1Turn ? 1 : 2, true);
            
            if (gameOver)
            {
                Debug.LogWarning("Player " + (player1Turn ? 1 : 2) +" Won!");
            }
            else if (isDraw(boardState))
            {
                gameOver = true;
                Debug.LogWarning("Game is Draw!");
            } 
            player1Turn = !player1Turn;
        }
    }

    bool updateBoard(int column)
    {
        for (int i = row_count - 1; i >= 0; i--)
        {
            if(boardState[i,column] == 0)
            {
                boardState[i, column] = player1Turn ? 1 : 2;
                lastRow = i;
                Debug.Log("Piece fitted in (" + i + ", " + column + ")");
                return true;
            }
        }
        Debug.LogWarning("Column " + column + " is full!");
        return false;
    }

    bool isDraw(int[,] board)
    {
        for (int i = 0; i < column_count; i++)
            if (board[0, i] == 0)
                return false;
        return true;
    }

    List<int> availableColumns(int[,] board)
    {
        List<int> availableColumns = new List<int>();
        for (int i = 0; i < column_count; i++)
            if (board[0, i] == 0)
                availableColumns.Add(i);
        return availableColumns;
    }

    int getOpenRow(int[,] board, int column)
    {
        for (int i = row_count - 1; i >= 0; i--)
            if (board[i, column] == 0)
                return i;
        return -1;
    }

    public IEnumerator BlinkGameObject(GameObject gameObject)
    {
        Material winColor, regularColor;
        if (player1Turn)
        {
            regularColor = color1;
            winColor = winColor1;
        }
        else
        {
            regularColor = color2;
            winColor = winColor2;
        }

        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < 60; i++)
        {
            gameObject.GetComponent<MeshRenderer>().material = ((i % 2) == 0 ? winColor : regularColor);
            yield return new WaitForSeconds(0.5f);
        }
        gameObject.GetComponent<MeshRenderer>().material = winColor;
        gameObject.SetActive(true);
    }

}
