using System.Collections.Generic;
using UnityEngine;
using Utilities.Extensions;
using VoiceScene.Logic;
using VoiceScene.Logic.Controller;
using VoiceScene.Logic.Utils;

namespace VoiceScene.UI.Speaker {
    /// <summary>
    /// Отрисовщик контроллеров управления мьютом через UI одной команды
    /// </summary>
    internal sealed class TeamSpeakersDrawer : MonoBehaviour {
        [SerializeField]
        private VoiceInterestGroupsHandler voiceInterestGroupsHandler;
        
        [Space]
        [SerializeField]
        private GameObject playerUiContainer;

        internal void DrawSpeaker(PlayerVoiceInfoController voiceInfoController) {
            var go = Instantiate(playerUiContainer, transform);
            
            var uiController = go.GetComponentInChildren<SpeakerUiController>(true);
            uiController.Initialize(voiceInfoController);
            
            go.name = voiceInfoController.NetworkVoiceInfo.Player.NickName;
        }
    }
}