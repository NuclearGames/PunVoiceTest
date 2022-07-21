using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Core;
using Utilities.Networking;

namespace Menu.RoomConnect.UI {
    internal sealed class ConnectionWindow : Singleton<ConnectionWindow>, IInRoomCallbacks {
        [SerializeField]
        private Text connectedPlayerText;

        [Header("Buttons")]
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private Button exitButton;

        private readonly List<string> _playersNames = new List<string>();

        internal void Open() {
            gameObject.SetActive(true);
        }

        internal void Close() {
            PhotonNetwork.LeaveRoom(false);
            gameObject.SetActive(false);
        }

        private void SetTextNames() {
            connectedPlayerText.text = string.Join(Environment.NewLine, _playersNames);
        }
        
        

#region Monobehaviour

        private protected override void Awake() {
            base.Awake();

            if (ReferenceEquals(Instance, this)) {
                AwakeInternal();
            }
        }

        private void AwakeInternal() {
            startButton.onClick.AddListener(LoadScene);
            exitButton.onClick.AddListener(Close);
        }

        private void OnEnable() {
            _playersNames.AddRange(PhotonNetwork.CurrentRoom.Players.Values.Select(player => player.NickName));
            SetTextNames();
            
            startButton.interactable = PhotonNetwork.IsMasterClient;
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable() {
            PhotonNetwork.RemoveCallbackTarget(this);
            
            _playersNames.Clear();
            connectedPlayerText.text = "";

            startButton.interactable = false;
        }

#endregion

#region Network events

        private void LoadScene() {
            if (PhotonNetwork.IsMasterClient) {
                
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel(SceneConstants.VOICE_SCENE);
            }
        }

#endregion
        

#region IInRoomCallbacks

        public void OnPlayerEnteredRoom(Player newPlayer) {
            _playersNames.Add(newPlayer.NickName);
            SetTextNames();
        }

        public void OnPlayerLeftRoom(Player otherPlayer) {
            _playersNames.Remove(otherPlayer.NickName);
            SetTextNames();
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }

        public void OnMasterClientSwitched(Player newMasterClient) {
            startButton.interactable = newMasterClient.IsLocal;
        }

#endregion
    }
}