using System;
using System.Collections;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using Utilities.Core;

namespace Networks {
    internal sealed class VoiceStateHandler : Singleton<VoiceStateHandler> {
        internal event Action onConnected;
        internal event Action onDisonnected;
        
        private ClientState State => PhotonVoiceNetwork.Instance.ClientState;
        
        private bool _isDisabled = false;

        private void Start() {
            StartCoroutine(ClientStateHandlerCoroutine());
        }
        
        private void OnDisable() {
            _isDisabled = true;
        }

        /// <summary>
        /// Выполняет действие с <seealso cref="PhotonVoiceNetwork"/>, дожидаясь подключения к комнате
        /// </summary>
        internal IEnumerator PerformAction(Action action) {
            if (PhotonVoiceNetwork.Instance.Client.IsConnectedAndReady) {
                if (!IsJoinedToRoom()) {
                    yield return new WaitUntil(IsJoinedToRoom);
                }
            } else if(!PhotonVoiceNetwork.Instance.Client.IsConnected) {
                PhotonVoiceNetwork.Instance.ConnectAndJoinRoom();
                yield return new WaitUntil(IsJoinedToRoom);
            }
            
            action.Invoke();
        }
        
        private ClientState _clientState;
        
        private IEnumerator ClientStateHandlerCoroutine() {
            yield return new WaitForFixedUpdate();

            _clientState = PhotonVoiceNetwork.Instance.ClientState;
            LogState();
            
            while (!_isDisabled) {
                yield return new WaitForFixedUpdate();
                
                var newState = PhotonVoiceNetwork.Instance.ClientState;

                if (_clientState != newState) {
                    _clientState = newState;
                    CheckEventState();
                    
                    LogState();
                }
            }
        }

#region States

        private bool IsJoinedToRoom() {
            return State == ClientState.Joined;
        }

        private void CheckEventState() {
            if (_clientState == ClientState.Joined) {
                onConnected?.Invoke();
            } else if (_clientState == ClientState.Disconnected) {
                onDisonnected?.Invoke();
            }
        }

#endregion

        private void LogState() {
#if UNITY_EDITOR
            Debug.Log($"[VOICE] New state: {_clientState.ToString()}");
#endif
        }
    }
}