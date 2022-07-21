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
        private ClientState State => PhotonVoiceNetwork.Instance.ClientState;
        
        private bool _isDisabled = false;

        private protected override void Awake() {
            base.Awake();
        }

        private void Start() {
#if UNITY_EDITOR
            StartCoroutine(ClientStateHandlerCoroutine());
#endif
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
        
        private bool IsJoinedToRoom() {
            return State == ClientState.Joined;
        }

        private void OnDisable() {
            _isDisabled = true;
        }
        
        
#if UNITY_EDITOR
        
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
                    LogState();
                }
            }
        }

        private void LogState() {
            Debug.Log($"[VOICE] New state: {_clientState.ToString()}");
        }
#endif
    }
}