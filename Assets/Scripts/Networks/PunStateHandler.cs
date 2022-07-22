using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities.Core;

namespace Networks {
    /// <summary>
    /// Служит только для мониторинга сосотояния подклчюения к комнате
    /// </summary>
    internal sealed class PunStateHandler: Singleton<PunStateHandler> {
        
        internal event Action onConnected;
        internal event Action onDisonnected;
        
        private bool _isDisabled = false;
        private ClientState _clientState;
        
        private void Start() {
            StartCoroutine(ClientStateHandlerCoroutine());
        }
        
        private void OnDisable() {
            _isDisabled = true;
        }
        

        private IEnumerator ClientStateHandlerCoroutine() {
            yield return new WaitForFixedUpdate();

            _clientState = PhotonNetwork.NetworkingClient.State;
            LogState();
            
            while (!_isDisabled) {
                yield return new WaitForFixedUpdate();
                
                var newState = PhotonNetwork.NetworkingClient.State;

                if (_clientState != newState) {
                    _clientState = newState;

                    CheckEventState();
                    
                    LogState();
                }
            }
        }
        
#region States

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
            Debug.Log($"[PUN] New state: {_clientState.ToString()}");
#endif
        }
    }
}