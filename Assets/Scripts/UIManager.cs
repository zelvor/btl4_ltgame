using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Scene Dependencies")]
    [SerializeField] private NetworkManager networkManager;

    [Header("Buttons")]
    [SerializeField] private Button playerOneButton;
    [SerializeField] private Button playerTwoButton;

    [Header("Texts")]
    [SerializeField] private Text resultText;
    [SerializeField] private Text connectionStatusText;

    [Header("Screen Gameobjects")]
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject connectionScreen;
    [SerializeField] private GameObject teamSelectionScreen;
    [SerializeField] private GameObject gameModeSelectionScreen;

    [Header("Other UI")]
    [SerializeField] private Dropdown gameLevelSelection;

    private void Awake(){
        OnGameLaunched();
    }

    private void OnGameLaunched(){
        gameoverScreen.SetActive(false);
        connectionScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameModeSelectionScreen.SetActive(true);
    }

    public void OnSingleplayerModeSelected(){
        DisableAllScreens();
    }

    private void DisableAllScreens(){
        gameoverScreen.SetActive(false);
        connectionScreen.SetActive(false);
        teamSelectionScreen.SetActive(false);
        gameModeSelectionScreen.SetActive(false);
    }

    public void OnMultiplayerModeSelected(){
        connectionStatusText.gameObject.SetActive(true);
        DisableAllScreens();
        connectionScreen.SetActive(true);
    }

    public void OnConnect()
    {
        networkManager.Connect();
    }

}
