using System;
using System.Collections;
using Menu.RoomConnect.UI;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

namespace Menu.Networking {
    internal abstract class BasePhotonConnectionHandler : MonoBehaviourPunCallbacks {
        [Header("Settings")]
        [SerializeField]
        private string version = "0.0.1";

        [Space]
        [SerializeField]
        private float connectToMasterServerTime = 2f;

        [SerializeField]
        private float connectToLobbyTime = 1f;
        
        

        private bool _masterConnected;
        private bool _lobbyConnected;
        
        private bool _invokeRequested;
        private protected Func<bool> OnSuccessConnectAction;

        private protected readonly RoomOptions CreateRoomOptions = new RoomOptions {
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
            InitializeInternal();
        }

#endregion

        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize() {
            if (_isInitialized) {
                return;
            }
            
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.UseByteArraySlicePoolForEvents = true;

            _isInitialized = true;
        }
        
        private protected abstract void InitializeInternal();
        
        private void ResetConnectionStates() {
            _invokeRequested = false;
            
            _masterConnected = false;
            _lobbyConnected = false;
            OnSuccessConnectAction = null;

            ResetConnectionStatesInternal();
        }

        private protected virtual void ResetConnectionStatesInternal() { }

#region PhotonNetwork
        
        /// <summary>
        /// Выполняет подключение к фотону
        /// </summary>
        private protected void ConnectToPhoton() {
            if (PhotonNetwork.IsConnected) {
                throw new Exception("Client has been already connected!");
            }
            
            _invokeRequested = true;

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = version;
        }

        /// <summary>
        /// Функция создания комнаты с настройками
        /// </summary>
        private protected bool CreateRoomInternal(string roomName) {
            return PhotonNetwork.CreateRoom(roomName, CreateRoomOptions);
        }


#region Server connection callbacks

        /// <summary>
        /// Фононвый контрроллер состояния подключения
        /// </summary>
        private protected IEnumerator ConnectionStateHandler() {
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
            if (!_invokeRequested) {
                return;
            }
            
            Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" + PhotonNetwork.CloudRegion + "] and can join a lobby/room. Calling: PhotonNetwork.JoinLobby();");

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
            if (!_invokeRequested) {
                return;
            }
            
            Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion +"].");

            if (_lobbyConnected) {
                return;
            }
            
            _lobbyConnected = true;
            
            if (OnSuccessConnectAction == null) {
                MsgBox.Instance.Open("Can't execute empty room-connect action!");
                PhotonNetwork.Disconnect();

                return;
            }

            if (!OnSuccessConnectAction()) {
                MsgBox.Instance.Open("Failed join random or create room");
                PhotonNetwork.Disconnect();

                return;
            }
        }

        /// <summary>
        /// При успешном подключении к комнате
        /// </summary>
        public override void OnJoinedRoom() {
            if (!_invokeRequested) {
                return;
            }

            OnJoinedRoomInternal();
        }

        private protected abstract void OnJoinedRoomInternal();

        public override void OnJoinRandomFailed(short returnCode, string message) {
            if (!_invokeRequested) {
                return;
            }
            
            MsgBox.Instance.Open($"Не удалось подключиться к случайной комнате: {message}");
            PhotonNetwork.Disconnect();
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
            if (!_invokeRequested) {
                return;
            }
            
            MsgBox.Instance.Open($"Не удалось создать комнату: {message}");
            PhotonNetwork.Disconnect();
        }

        public override void OnLeftRoom() {
            if (!_invokeRequested) {
                return;
            }
            
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
    }
}