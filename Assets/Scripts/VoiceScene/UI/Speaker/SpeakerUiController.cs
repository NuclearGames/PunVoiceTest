using System;
using UnityEngine;
using UnityEngine.UI;
using VoiceScene.Logic;

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
        private string playTextValue;
        [SerializeField]
        private string muteTextValue;

        internal void Initialize(PlayerVoiceInfoController voiceInfoController) {
            var player = voiceInfoController.NetworkVoiceInfo.Player;
            
            userNameText.text = player.NickName;
            SwitchMuteText(player.IsLocal);
            
            voiceInfoController.onMuteStateChanged += SwitchMuteText;
            muteButton.onClick.AddListener(voiceInfoController.ChangeMuteState);
        }

        private void SwitchMuteText(bool muted) {
            muteButtonText.text = muted ? muteTextValue : playTextValue;
        }
    }
}