using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoiceScene.UI.Speaker;

namespace VoiceScene.Logic.Controller {
    internal sealed class GroupSpeakerController : MonoBehaviour {
        [Header("Logic")]
        [SerializeField]
        private TeamsBuilder teamsBuilder;
        [SerializeField]
        private VoiceInterestGroupsHandler interestGroupsHandler;

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
            
            voiceInfoController.onExtendedMuteStateChanged += VoiceInfoControllerOnExtendedMuteStateChanged;
            VoiceInfoControllerOnExtendedMuteStateChanged(voiceInfoController, teamInfo.IsLocal, voiceInfoController.Muted);
        }

#region Mute actions

        private void SetMuteStateForTeammates(bool muteState) {
            SetMuteStateInternal(muteState, _teammates, _enemies);
        } 
        private void SetMuteStateForEnemies(bool muteState) {
            SetMuteStateInternal(muteState, _enemies, _teammates);
        }

        private void SetMuteStateForAll(bool muteState) {
            _lockedCallBack = true;
            teammateGroupUi.MuteExternal(muteState);
            enemyGroupUi.MuteExternal(muteState);
            
            if (muteState) {
                interestGroupsHandler.MuteAll();   
            }
            _lockedCallBack = false;
        }
        
        private void SetMuteStateInternal(bool muteState, List<PlayerVoiceInfoController> targetList, List<PlayerVoiceInfoController> anotherList) {
            _lockedCallBack = true;

            targetList.ForEach(voiceInfoController => {
                if (voiceInfoController.Muted != muteState) {
                    voiceInfoController.ChangeMuteState();
                }
            });
            
            if (muteState && !CheckAnyListeningState(anotherList)) {
                interestGroupsHandler.MuteAll();
                allGroupUi.SetImageExternal(true);
            } else if (!muteState) {
                interestGroupsHandler.Unmute();
                allGroupUi.SetImageExternal(false);
            }

            _lockedCallBack = false;
        }

#endregion
        
#region Subscriptions

        private bool _lockedCallBack;
        private void VoiceInfoControllerOnExtendedMuteStateChanged(PlayerVoiceInfoController controller, bool isLocal, bool muted) {
            if (_lockedCallBack) {
                return;
            }

            if (muted) {
                void CheckMuteInternal(List<PlayerVoiceInfoController> targetList, List<PlayerVoiceInfoController> anotherList, GroupSpeakerUiController targetGroupUi) {
                    if (CheckAnyListeningState(targetList)) {
                        return;
                    }

                    if (CheckAnyListeningState(anotherList)) {
                        targetGroupUi.MuteExternal(true);
                    } else {
                        SetMuteStateForAll(true);
                    }
                }

                if (isLocal) {
                    CheckMuteInternal(_teammates, _enemies, teammateGroupUi);
                } else {
                    CheckMuteInternal(_enemies, _teammates, enemyGroupUi);
                }
            } else {
                if (isLocal) {
                    teammateGroupUi.SetImageExternal(false);
                } else {
                    enemyGroupUi.SetImageExternal(false);
                }
                allGroupUi.SetImageExternal(false);
            }
        }

#endregion

#region Utils

        private bool CheckAnyListeningState(List<PlayerVoiceInfoController> list) => list.Any(el => !el.Muted);

#endregion
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (ReferenceEquals(teamsBuilder, null)) {
                teamsBuilder = FindObjectOfType<TeamsBuilder>();
            }
            if (ReferenceEquals(interestGroupsHandler, null)) {
                interestGroupsHandler = FindObjectOfType<VoiceInterestGroupsHandler>();
            }
            
        }

#endif
    }
}