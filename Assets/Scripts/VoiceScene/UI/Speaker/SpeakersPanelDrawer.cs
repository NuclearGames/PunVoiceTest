using UnityEngine;
using VoiceScene.Logic;

namespace VoiceScene.UI.Speaker {

    /// <summary>
    /// Отрисовщик контроллеров управления мьютом через UI для всех команд 
    /// </summary>
    internal sealed class SpeakersPanelDrawer : MonoBehaviour {
        [SerializeField]
        private TeamsBuilder teamsBuilder;

        [SerializeField]
        private TeamSpeakersDrawer[] teamSpeakersDrawers;

        internal void DrawSpeaker(PlayerVoiceInfoController voiceInfoController) {
            var voiceInfo = voiceInfoController.NetworkVoiceInfo;
            teamSpeakersDrawers[voiceInfo.TeamIndex].DrawSpeaker(voiceInfoController);
        }
    }
}