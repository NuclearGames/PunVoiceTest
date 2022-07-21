using UnityEngine;
using VoiceScene.Logic.Controller;

namespace VoiceScene.UI.Speaker {
    /// <summary>
    /// Отрисовщик контроллеров управления мьютом через UI одной команды
    /// </summary>
    internal sealed class TeamSpeakersDrawer : MonoBehaviour {
        [SerializeField]
        private GameObject playerUiContainer;

        internal void DrawSpeaker(PlayerVoiceInfoController voiceInfoController, bool isLocal) {
            var go = Instantiate(playerUiContainer, transform);
            
            var uiController = go.GetComponentInChildren<SpeakerUiController>(true);
            uiController.Initialize(voiceInfoController, isLocal);
            
            go.name = voiceInfoController.NetworkVoiceInfo.Player.NickName;
        }
    }
}