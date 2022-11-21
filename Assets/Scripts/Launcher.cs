using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
#region Private Serializable Fields
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [
            Tooltip(
                "The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")
        ]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;


#endregion



#region Private Fields
        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";


#endregion



#region MonoBehaviour CallBacks
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            teamSelection.SetActive(false);
        }


#endregion



#region Public Fields
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;

        [
            Tooltip(
                "The UI Label to inform the user that the connection is in progress")
        ]
        [SerializeField]
        private GameObject progressLabel;

        [SerializeField]
        private GameObject teamSelection;

        [SerializeField]
        private Button team1Button;

        [SerializeField]
        private Button team2Button;


#endregion



#region Public Methods
        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            teamSelection.SetActive(false);

            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }


#endregion



#region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            PhotonNetwork.JoinRandomRoom();
            Debug
                .Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            teamSelection.SetActive(false);
            Debug
                .LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}",
                cause);
        }

        public override void OnJoinRandomFailed(
            short returnCode,
            string message
        )
        {
            Debug
                .Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork
                .CreateRoom(null,
                new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug
                .Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            ShowTeamSelectionScreen();
            PrepareTeamSelection();
            StartGame();
        }

        public void ShowTeamSelectionScreen()
        {
            Debug.Log("Show Team Selection");
            progressLabel.SetActive(false);
            controlPanel.SetActive(false);
            teamSelection.SetActive(true);
        }

        private void PrepareTeamSelection()
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
                if (firstPlayer.CustomProperties.ContainsKey("Team"))
                {
                    var occupiedTeam = firstPlayer.CustomProperties["Team"];
                    Debug.Log("Team " + occupiedTeam + " is occupied");
                    if (occupiedTeam.Equals(1))
                    {
                        team1Button.interactable = false;
                    }
                    else
                    {
                        team2Button.interactable = false;
                    }
                }
            }
        }

        public void ChooseTeam(int teamInt)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
                if (firstPlayer.CustomProperties.ContainsKey("Team"))
                {
                    var occupiedTeam = firstPlayer.CustomProperties["Team"];
                    teamInt = (int) occupiedTeam == 1 ? 2 : 1;
                }
            }
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"Team", teamInt}});
            Debug.Log("Team " + teamInt + " is chosen");
        }

        public void StartGame()
        {
            //if both 2 players are chosen, load scene
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                var firstPlayer = PhotonNetwork.CurrentRoom.GetPlayer(1);
                var secondPlayer = PhotonNetwork.CurrentRoom.GetPlayer(2);
                if (firstPlayer.CustomProperties.ContainsKey("Team") &&
                    secondPlayer.CustomProperties.ContainsKey("Team"))
                {
                    var firstPlayerTeam = firstPlayer.CustomProperties["Team"];
                    var secondPlayerTeam = secondPlayer.CustomProperties["Team"];
                    PhotonNetwork.LoadLevel("MultiplayerGameScene");
                }
            }



        }

#endregion

    }
}
