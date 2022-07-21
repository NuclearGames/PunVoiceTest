using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networks {
    /// <summary>
    /// Служит только для мониторинга сосотояния подклчюения к комнате
    /// </summary>
    internal sealed class PunStateHandler: MonoBehaviour {

#if UNITY_EDITOR
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
                    LogState();
                }
            }
        }

        private void LogState() {
            Debug.Log($"[PUN] New state: {_clientState.ToString()}");
        }
#endif
    }
}