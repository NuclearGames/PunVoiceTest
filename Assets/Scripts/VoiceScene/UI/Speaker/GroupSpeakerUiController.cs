using System;
using UnityEngine;
using UnityEngine.UI;

namespace VoiceScene.UI.Speaker {
    internal sealed class GroupSpeakerUiController : MonoBehaviour {
        [SerializeField]
        private Button muteButton;
        [SerializeField]
        private Image muteButtonImage;
        
        [Space]
        [SerializeField]
        private Sprite playSprite;
        [SerializeField]
        private Sprite muteSprite;


        private bool _muteState = false;
        private Action<bool> _clickAction;

        /// <summary>
        /// Функция инициализации кнопки
        /// </summary>
        internal void Initialize(Action<bool> clickAction) {
            _clickAction = clickAction;
            SetImage(_muteState);
            
            muteButton.onClick.AddListener(ClickInternal);
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
        
        internal void SetImageExternal(bool muteState) {
            if (_muteState != muteState) {
                _muteState = muteState;
                SetImage(muteState);
            }
        }

        /// <summary>
        /// Сама функция мута
        /// </summary>
        private void ClickInternal() {
            _muteState = !_muteState;
            
            _clickAction.Invoke(_muteState);
            SetImage(_muteState);
        }

        private void SetImage(bool muteState) {
            muteButtonImage.sprite = muteState
                ? muteSprite
                : playSprite;
        }
        
#if UNITY_EDITOR

        private void OnValidate() {
            if (ReferenceEquals(muteButton, null)) {
                muteButton = GetComponentInChildren<Button>();
            }
            
            if (ReferenceEquals(muteButtonImage, null)) {
                muteButtonImage = muteButton.GetComponent<Image>();
            }

        }

#endif
    }
}