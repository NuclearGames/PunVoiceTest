using System;
using Menu.Networking;
using Menu.RoomConnect.UI;
using Menu.RoomSettings;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.RoomConnect {
    internal sealed class RoomConnectionHandler : BasePhotonConnectionHandler {
        [Header("Components")]
        [SerializeField]
        private NewRoomPlayersCountHandler playersCountHandler;
        
        [Space]
        [SerializeField]
        private Button joinRandomRoomButton;
        [SerializeField]
        private Button createRoomButton;
        
        
        private protected override void InitializeInternal() {
            playersCountHandler.onPlayersCountChanged += OnPlayersCountChanged;
            CreateRoomOptions.MaxPlayers = playersCountHandler.PlayersCount;
            
            joinRandomRoomButton.onClick.AddListener(ConnectToExistedRoom);
            createRoomButton.onClick.AddListener(CreateNewRoom);
        }

#region Actions

        /// <summary>
        /// Пытается подключиться к случайной существующей комнате
        /// </summary>
        private void ConnectToExistedRoom() {
            OnSuccessConnectAction = PhotonNetwork.JoinRandomRoom;

            ConnectToPhoton();
            StartCoroutine(ConnectionStateHandler());
        }

        /// <summary>
        /// Пытается создать комнату
        /// </summary>
        private void CreateNewRoom() {
            OnSuccessConnectAction = CreateRoom;

            ConnectToPhoton();
            StartCoroutine(ConnectionStateHandler());
        }
        
#endregion

#region PhotonNetwork

        private bool CreateRoom() {
            return CreateRoomInternal(Guid.NewGuid().ToString());
        }

#region Callbacks

        private protected override void OnJoinedRoomInternal() {
            ConnectionWindow.Instance.Open();
        }

#endregion

#endregion
        
#region Subscriptions

        /// <summary>
        /// При изменении кол-ва игроков
        /// </summary>
        private void OnPlayersCountChanged(int playersCount) {
            CreateRoomOptions.MaxPlayers = Convert.ToByte(playersCount);
        }

#endregion
        
#if UNITY_EDITOR

        private void OnValidate() {
            if (ReferenceEquals(playersCountHandler, null)) {
                playersCountHandler = GetComponentInChildren<NewRoomPlayersCountHandler>();
            }
        }

#endif
    }
}