using System;
using UnityEngine;
using UnityEngine.UI;
using VoiceScene.Logic;
using VoiceScene.Logic.Controller;

namespace VoiceScene.UI.Speaker {
    /// <summary>
    /// Контроллер управления мьютом игрока через UI
    /// </summary>
    internal sealed class SpeakerUiController : MonoBehaviour {
        [Header("UI")]
        [SerializeField]
        private Text userNameText;
        [SerializeField]
        private Button muteButton;
        [SerializeField]
        private Text muteButtonText;
        [Space]
        [SerializeField]
        private Image backgroundImage;

        [Space]
        [SerializeField]
        private string playTextValue;
        [SerializeField]
        private string muteTextValue;

        [Space]
        [SerializeField]
        private Color localColor;
        [SerializeField]
        private Color enemyColor;

        internal void Initialize(PlayerVoiceInfoController voiceInfoController, bool isLocal) {
            var player = voiceInfoController.NetworkVoiceInfo.Player;
            
            userNameText.text = player.NickName;
            SwitchMuteText(player.IsLocal);
            
            voiceInfoController.onMuteStateChanged += SwitchMuteText;
            muteButton.onClick.AddListener(voiceInfoController.ChangeMuteState);

            backgroundImage.color = isLocal ? localColor : enemyColor;
        }

        private void SwitchMuteText(bool muted) {
            muteButtonText.text = muted ? muteTextValue : playTextValue;
        }
    }
}