using System;
using System.Collections;
using Menu.RoomConnect.UI;
using Menu.RoomSettings;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.RoomConnect {
    internal sealed class PhotonConnectionHandler : MonoBehaviourPunCallbacks {
        [Header("Settings")]
        [SerializeField]
        private string version = "0.0.1";

        [Space]
        [SerializeField]
        private float connectToMasterServerTime = 2f;

        [SerializeField]
        private float connectToLobbyTime = 1f;

        [Header("Components")]
        [SerializeField]
        private NewRoomPlayersCountHandler playersCountHandler;

        [Space]
        [SerializeField]
        private Button joinRandomRoomButton;
        [SerializeField]
        private Button createRoomButton;

        private bool _masterConnected;
        private bool _lobbyConnected;
        private Func<bool> _onSuccessConnectAction;

        private readonly RoomOptions _createRoomOptions = new RoomOptions {
            IsOpen = true,
            IsVisible = true,
            PlayerTtl = 2000,
            EmptyRoomTtl = 1000,
            SuppressPlayerInfo = false,
            PublishUserId = true,
            BroadcastPropsChangeToAll = true,
            SuppressRoomEvents = false
        };

#region Monobehaviour

        private void Awake() {
            SetUp();
                
            playersCountHandler.onPlayersCountChanged += OnPlayersCountChanged;
            _createRoomOptions.MaxPlayers = playersCountHandler.PlayersCount;
            
            joinRandomRoomButton.onClick.AddListener(ConnectToExistedRoom);
            createRoomButton.onClick.AddListener(CreateNewRoom);
        }

#endregion
        
        /// <summary>
        /// Пытается подключиться к случайной существующей комнате
        /// </summary>
        private void ConnectToExistedRoom() {
            _onSuccessConnectAction = PhotonNetwork.JoinRandomRoom;

            ConnectToPhoton();
            StartCoroutine(ConnectionStateHandler());
        }

        /// <summary>
        /// Пытается создать комнату
        /// </summary>
        private void CreateNewRoom() {
            _onSuccessConnectAction = CreateRoom;

            ConnectToPhoton();
            StartCoroutine(ConnectionStateHandler());
        }

        /// <summary>
        /// Выполняет подключение к фотону
        /// </summary>
        private void ConnectToPhoton() {
            if (PhotonNetwork.IsConnected) {
                throw new Exception("Client has been already connected!");
            }

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = version;
        }

        /// <summary>
        /// Функция создания комнаты с настройками
        /// </summary>
        private bool CreateRoom() {
            return PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), _createRoomOptions);
        }

#region PhotonNetwork

        private void SetUp() {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true;
        }

        private void ResetConnectionStates() {
            _masterConnected = false;
            _lobbyConnected = false;
            _onSuccessConnectAction = null;
        }

#region Server connection callbacks

        /// <summary>
        /// Фононвый контрроллер состояния подключения
        /// </summary>
        private IEnumerator ConnectionStateHandler() {
            float startTime = Time.time;

            yield return new WaitWhile(() => (Time.time - startTime) < connectToMasterServerTime || !_masterConnected);
            if (!_masterConnected) {
                MsgBox.Instance.Open("Fail connect to master server!");
                PhotonNetwork.Disconnect();

                yield break;
            }

            yield return new WaitWhile(() => (Time.time - startTime) < connectToLobbyTime || !_lobbyConnected);
            if (!_masterConnected) {
                MsgBox.Instance.Open("Fail connect to lobby!");
                PhotonNetwork.Disconnect();

                yield break;
            }
        }

        /// <summary>
        /// При подключении к мастер-серверу
        /// </summary>
        public override void OnConnectedToMaster() {
            Debug.Log(
                "OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion + "] and can join a lobby/room. Calling: PhotonNetwork.JoinLobby();");

            if (_masterConnected) {
                return;
            }
            
            _masterConnected = true;
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// При подключении к лобби
        /// </summary>
        public override void OnJoinedLobby() {
            Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion +"].");

            if (_lobbyConnected) {
                return;
            }
            
            _lobbyConnected = true;
            
            if (_onSuccessConnectAction == null) {
                MsgBox.Instance.Open("Can't execute empty room-connect action!");
                PhotonNetwork.Disconnect();

                return;
            }

            if (!_onSuccessConnectAction()) {
                MsgBox.Instance.Open("Failed join random or create room");
                PhotonNetwork.Disconnect();

                return;
            }
        }

        /// <summary>
        /// При успешном подключении к комнате
        /// </summary>
        public override void OnJoinedRoom() {
            ConnectionWindow.Instance.Open();
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            MsgBox.Instance.Open($"Не удалось подключиться к случайной комнате: {message}");
            PhotonNetwork.Disconnect();
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            MsgBox.Instance.Open($"Не удалось создать комнату: {message}");
            PhotonNetwork.Disconnect();
        }

        public override void OnLeftRoom() {
            PhotonNetwork.Disconnect();
        }


        /// <summary>
        /// При отключении от фотона
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause) {
            ResetConnectionStates();
        }

#endregion

#endregion

#region Subscriptions

        /// <summary>
        /// При изменении кол-ва игроков
        /// </summary>
        private void OnPlayersCountChanged(int playersCount) {
            _createRoomOptions.MaxPlayers = Convert.ToByte(playersCount);
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