using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Core;

namespace UI {
    internal sealed class MsgBox: Singleton<MsgBox> {
        [SerializeField]
        private Text textField;

        [SerializeField]
        private Button closeButton;

        private protected override void Awake() {
            base.Awake();
            if (ReferenceEquals(Instance, this)) {
                AwakeInternal();
            }
        }

        private void AwakeInternal() {
            closeButton.onClick.AddListener(Close);
        }

        internal void Open(string text) {
            textField.text = text;
            gameObject.SetActive(true);
        }

        internal void Close() {
            gameObject.SetActive(false);
            textField.text = "";
        }
        
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (ReferenceEquals(textField, null)) {
                textField = GetComponentInChildren<Text>();
            }

            if (ReferenceEquals(closeButton, null)) {
                closeButton = GetComponentInChildren<Button>();
            }
        }

#endif
    }
}