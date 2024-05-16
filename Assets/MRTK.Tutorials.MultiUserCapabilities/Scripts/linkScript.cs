using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

namespace LTA.Holoapp
{
    public class linkScript : MonoBehaviour
    {
        public TextMeshPro naming = default;
        private Player[] photonPlayers;
        private int playersInRoom;
        private int myNumberInRoom;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            naming.text = PhotonNetwork.AuthValues.UserId;
        }
    }
}
