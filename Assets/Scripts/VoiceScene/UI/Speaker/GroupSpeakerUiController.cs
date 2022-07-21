using System;
using UnityEngine;
using UnityEngine.UI;

namespace VoiceScene.UI.Speaker {
    internal sealed class GroupSpeakerUiController : MonoBehaviour {
        [SerializeField]
        private Button muteButton;
        [SerializeField]
        private Text muteButtonText;
        
        [Space]
        [SerializeField]
        private string playTextValue;
        [SerializeField]
        private string muteTextValue;


        private bool _muteState = false;
        private Action<bool> _clickAction;

        /// <summary>
        /// Функция инициализации кнопки
        /// </summary>
        internal void Initialize(Action<bool> clickAction) {
            _clickAction = clickAction;
            SetText(_muteState);
        }
        
        /// <summary>
        /// Функция вызова Мута из внешний источников
        /// <para>Как правило, вызывается только в случае, если необходимо применить новое состояние толко для всех игроков</para>
        /// </summary>
        internal void MuteExternal(bool muteState) {
            if (_muteState != muteState) {
                muteButton.onClick.Invoke();
            }
        }

        /// <summary>
        /// Сама функция мута
        /// </summary>
        private void ClickInternal() {
            _muteState = !_muteState;
            
            _clickAction.Invoke(_muteState);
            SetText(_muteState);
        }

        private void SetText(bool muteState) {
            muteButtonText.text = muteState
                ? muteTextValue
                : playTextValue;
        }
    }
}