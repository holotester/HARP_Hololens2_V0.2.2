using LTA.Holoapp;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonLobby : MonoBehaviourPunCallbacks
    {
        public static PhotonLobby Lobby;

        private int roomNumber = 1;
        private int userIdCount;
        public TextMeshPro keyText = default;
        private void Awake()
        {
            if (Lobby == null)
            {
                Lobby = this;
            }
            else
            {
                if (Lobby != this)
                {
                    Destroy(Lobby.gameObject);
                    Lobby = this;
                }
            }

            DontDestroyOnLoad(gameObject);

            GenericNetworkManager.OnReadyToStartNetwork += StartNetwork;
        }

        public override void OnConnectedToMaster()
        {
            /*if (userName.uName != default)
            {
                
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.AuthValues = new AuthenticationValues();
                PhotonNetwork.AuthValues.UserId = userName.uName;
                userIdCount++;
                PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
            }
            else
            {
                var randomUserId = Random.Range(0, 999999);
                PhotonNetwork.AutomaticallySyncScene = true;
                PhotonNetwork.AuthValues = new AuthenticationValues();
                PhotonNetwork.AuthValues.UserId = randomUserId.ToString();
                userIdCount++;
                PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
            }*/

            var randomUserId = Random.Range(0, 999999);
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.AuthValues = new AuthenticationValues();
            //get the username entered by user from previous scene. name only start from the 8th letter
            Debug.Log("onconnectedtomaster keytext = " + keyText.text.Substring(7));
            var uname= keyText.text.Split(' ');
            PhotonNetwork.AuthValues.UserId = keyText.text.Substring(7);
            userIdCount++;
            PhotonNetwork.NickName = keyText.text.Substring(7);
            PhotonNetwork.JoinRandomRoom();

        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            PhotonNetwork.NickName = keyText.text;
            Debug.Log("\nPhotonLobby.OnJoinedRoom()");
            Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            CreateRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("\nPhotonLobby.OnCreateRoomFailed()");
            Debug.LogError("Creating Room Failed");
            CreateRoom();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            roomNumber++;
        }

        public void OnCancelButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void StartNetwork()
        {
            PhotonNetwork.ConnectUsingSettings();
            Lobby = this;
        }

        private void CreateRoom()
        {
            var roomOptions = new RoomOptions {IsVisible = true, IsOpen = true, MaxPlayers = 10, PublishUserId = true };
            
            PhotonNetwork.CreateRoom("Room " + Random.Range(1,3000), roomOptions);
        }
    }
}
