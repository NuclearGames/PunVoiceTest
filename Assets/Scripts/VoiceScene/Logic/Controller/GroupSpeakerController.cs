using System;
using System.Collections.Generic;
using UnityEngine;
using VoiceScene.UI.Speaker;

namespace VoiceScene.Logic.Controller {
    internal sealed class GroupSpeakerController : MonoBehaviour {
        [Header("Logic")]
        [SerializeField]
        private TeamsBuilder teamsBuilder;

        [Header("UI")]
        [SerializeField]
        private GroupSpeakerUiController teammateGroupUi;
        [SerializeField]
        private GroupSpeakerUiController enemyGroupUi;
        [SerializeField]
        private GroupSpeakerUiController allGroupUi;

        private readonly List<PlayerVoiceInfoController> _teammates = new List<PlayerVoiceInfoController>();
        private readonly List<PlayerVoiceInfoController> _enemies = new List<PlayerVoiceInfoController>();

        private void Awake() {
            teammateGroupUi.Initialize(SetMuteStateForTeammates);
            enemyGroupUi.Initialize(SetMuteStateForEnemies);
            allGroupUi.Initialize(SetMuteStateForAll);
        }

        /// <summary>
        /// Регистрирует войс-контроллер
        /// </summary>
        internal void RegisterVoiceController(PlayerVoiceInfoController voiceInfoController) {
            var voiceInfo = voiceInfoController.NetworkVoiceInfo;

            if (!teamsBuilder.TryGetTeamByPlayer(voiceInfo.Player.ActorNumber, out var teamInfo)) {
                throw new Exception($"Can't find team of player '{voiceInfo.Player.NickName}' !");
            }

            if (teamInfo.IsLocal) {
                _teammates.Add(voiceInfoController);
            } else {
                _enemies.Add(voiceInfoController);
            }
        }

#region Mute actions

        private void SetMuteStateForTeammates(bool muteState) {
            _teammates.ForEach(voiceInfoController => {
                if (voiceInfoController.Muted != muteState) {
                    voiceInfoController.ChangeMuteState();
                }
            });
        } 
        private void SetMuteStateForEnemies(bool muteState) {
            _teammates.ForEach(voiceInfoController => {
                if (voiceInfoController.Muted != muteState) {
                    voiceInfoController.ChangeMuteState();
                }
            });
        }

        private void SetMuteStateForAll(bool muteState) {
            teammateGroupUi.MuteExternal(muteState);
            enemyGroupUi.MuteExternal(muteState);
        }

#endregion
    }
}