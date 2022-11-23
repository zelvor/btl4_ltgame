using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardState : MonoBehaviour {
    public bool _selectedPlayer1;
    public int _difficulty;

    public bool _player1Turn;
    public bool _gameOver;

    public int[,] _boardState;

    private GameObject[,] _objects;

    private int _lastRow;

    public const int _row_count = 6;

    public const int _column_count = 7;

    public bool SelectedPlayer1 {
        get {
            return _selectedPlayer1;
        }
        set {
            _selectedPlayer1 = value;
        }
    }

    public int Difficulty {
        get {
            return _difficulty;
        }
        set {
            _difficulty = value;
        }
    }

    public bool Player1Turn {
        get {
            return _player1Turn;
        }
        set {
            _player1Turn = value;
        }
    }

    public bool GameOver {
        get {
            return _gameOver;
        }
        set {
            _gameOver = value;
        }
    }

    public int[,] Board {
        get {
            return _boardState;
        }
        set {
            _boardState = value;
        }
    }

    public GameObject[,] Objects {
        get {
            return _objects;
        }
        set {
            _objects = value;
        }
    }

    public int LastRow {
        get {
            return _lastRow;
        }
        set {
            _lastRow = value;
        }
    }

}