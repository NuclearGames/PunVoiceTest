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
            
            userNameInputField.onEndEdit.AddListener(OnEndEditName);
        }

        private void OnEndEditName(string value) {
            PhotonNetwork.NickName = value;
            
            Debug.Log($"New User NickName: '{PhotonNetwork.NickName}'");
        }
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (ReferenceEquals(userNameInputField, null)) {
                userNameInputField = GetComponentInChildren<InputField>();
            }
        }
#endif
    }
}