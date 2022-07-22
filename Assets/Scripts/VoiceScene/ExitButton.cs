using System;
using Networks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoiceScene {
    internal sealed class ExitButton : MonoBehaviour {
        private bool _exitRequested = false;
        
        private void Start() {
            PunStateHandler.Instance.onDisonnected += ExistScene;
        }

        private void Update() {
            if (Input.GetKey(KeyCode.Escape)) {
                ExitPhoton();
            }
        }

        private void ExitPhoton() {
            _exitRequested = true;
            PhotonNetwork.Disconnect();
        }

        private void ExistScene() {
            if (!_exitRequested) {
                return;
            }

            PunStateHandler.Instance.onDisonnected -= ExistScene;
            SceneManager.LoadScene(0);
        }
    }
}