using LTA.Holoapp;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        public static PhotonRoom Room;

        [SerializeField] private GameObject photonUserPrefab = default;
        [SerializeField] private GameObject roverExplorerPrefab = default;
        [SerializeField] private Transform roverExplorerLocation = default;

        // private PhotonView pv;
        private Player[] photonPlayers;
        private int playersInRoom;
        private int myNumberInRoom;
        private Player userID;
        // private GameObject module;
        // private Vector3 moduleLocation = Vector3.zero;
        public TextMeshPro keyText = default;
        public Material mat1;
        public Material mat2;
        public Material mat3;
        public Material mat4;
        public Material mat5;
        public List<Material> dict = new List<Material>(5);
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("OnPlayerEnteredRoom");
            base.OnPlayerEnteredRoom(newPlayer);
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;
            //photonUserPrefab.GetComponentInChildren<TextMeshPro>().text = "user" + playersInRoom;
            Debug.Log(newPlayer.NickName);
            photonUserPrefab.GetComponentInChildren<TextMeshPro>().text = newPlayer.NickName;
            Debug.Log(newPlayer.UserId);
            var num = Random.Range(0, 4);
            //add 5 material to the List to generate random material for each different avatar
            dict.Add(mat1);
            dict.Add(mat2);
            dict.Add(mat3);
            dict.Add(mat4);
            dict.Add(mat5);
            photonUserPrefab.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material = dict[num]; ;
            
            //photonUserPrefab.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

        private void Awake()
        {
            if (Room == null)
            {
                Room = this;
            }
            else
            {
                if (Room != this)
                {
                    Destroy(Room.gameObject);
                    Room = this;
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Start()
        {
            // pv = GetComponent<PhotonView>();

            // Allow prefabs not in a Resources folder
            if (PhotonNetwork.PrefabPool is DefaultPool pool)
            {
                if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);

                if (roverExplorerPrefab != null) pool.ResourceCache.Add(roverExplorerPrefab.name, roverExplorerPrefab);

                //Debug.Log(userName.uName);
            }
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("OnJoinedRoom");
            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom = photonPlayers.Length;
            myNumberInRoom = playersInRoom;
            /*PhotonNetwork.NickName = userName.uName;
            Debug.Log("on join room nickname: "+ PhotonNetwork.NickName);*/
            PhotonNetwork.NickName = keyText.text;
            //photonView.RPC("SetLobbyPlayersData", PhotonTargets.all);

            StartGame();
        }

  
        private void StartGame()
        {
            Debug.Log("StartGame");
            CreatPlayer();

            if (!PhotonNetwork.IsMasterClient) return;

            if (TableAnchor.Instance != null) CreateInteractableObjects();
        }

        private void CreatPlayer()
        {
            var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
        }

        private void CreateInteractableObjects()
        {
            Debug.Log("CreateInteractableObjects");
            var position = roverExplorerLocation.position;
            var positionOnTopOfSurface = new Vector3(position.x, position.y + roverExplorerLocation.localScale.y / 2,
                position.z);

            var go = PhotonNetwork.Instantiate(roverExplorerPrefab.name, positionOnTopOfSurface,
                roverExplorerLocation.rotation);
        }

        // private void CreateMainLunarModule()
        // {
        //     module = PhotonNetwork.Instantiate(roverExplorerPrefab.name, Vector3.zero, Quaternion.identity);
        //     pv.RPC("Rpc_SetModuleParent", RpcTarget.AllBuffered);
        // }
        //
        // [PunRPC]
        // private void Rpc_SetModuleParent()
        // {
        //     Debug.Log("Rpc_SetModuleParent- RPC Called");
        //     module.transform.parent = TableAnchor.Instance.transform;
        //     module.transform.localPosition = moduleLocation;
        // }
    }
}
