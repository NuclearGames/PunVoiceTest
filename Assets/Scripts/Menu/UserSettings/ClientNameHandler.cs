using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Core;
using Random = UnityEngine.Random;

namespace Menu.UserSettings {
    internal sealed class ClientNameHandler : MonoBehaviour {
        [SerializeField]
        private InputField userNameInputField;

        private void Start() {
            var nickName = $"User {Random.Range(0, 1000)}";
            userNameInputField.SetTextWithoutNotify(nickName);
            PhotonNetwork.NickName = nickName;

            SetUpSubscriber();
        }

        
        
#if UNITY_EDITOR
        private void SetUpSubscriber() {
            userNameInputField.onEndEdit.AddListener(OnEndEditName);
        }
        
        private void OnEndEditName(string value) {
            PhotonNetwork.NickName = value;
            
            Debug.Log($"New User NickName: '{PhotonNetwork.NickName}'");
        }
        
        private void OnValidate() {
            if (ReferenceEquals(userNameInputField, null)) {
                userNameInputField = GetComponentInChildren<InputField>();
            }
        }
#else

        private void SetUpSubscriber() {
            userNameInputField.onValueChanged.AddListener(OnValueChanged);
        }
        
        private void OnValueChanged(string value) {
            PhotonNetwork.NickName = value;
        }
#endif
    }
}