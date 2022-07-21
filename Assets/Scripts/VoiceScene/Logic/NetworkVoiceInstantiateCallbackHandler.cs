using System;
using System.Collections;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using Utilities.Extensions;
using VoiceScene.Logic.Controller;
using VoiceScene.Logic.InstantiateArgs;
using VoiceScene.Logic.Utils;
using VoiceScene.UI.Speaker;

namespace VoiceScene.Logic {
    /// <summary>
    /// Компонент, который ловит факт создания сущности `NetworkVoice`, и привязывает ее к конкретному игроку
    /// </summary>
    internal sealed class NetworkVoiceInstantiateCallbackHandler : MonoBehaviour {
        [Header("Components")]
        [SerializeField]
        private PhotonView photonView;
        [SerializeField]
        private PhotonVoiceView photonVoiceView;
        [SerializeField]
        private AudioSource audioSource;

        [Space]
        [SerializeField]
        private PlayerVoiceInfoController voiceInfoController;
        
        private IEnumerator Start() {
            var teamBuilder = ObjectExtensions.FindSingleInScene<TeamsBuilder>();
            
            var voiceInfoComponents = new PlayerVoiceInfo.PlayerVoiceInfoComponents {
                PhotonView = transform.GetComponentInChildren<PhotonView>(true),
                PhotonVoiceView = transform.GetComponentInChildren<PhotonVoiceView>(true),
                AudioSource = transform.GetComponentInChildren<AudioSource>(true)
            };
            
            var instantiateProperties = (VoiceEntityInstantiateArgs)(byte[])voiceInfoComponents.PhotonView.InstantiationData[0];
            
            var player = PhotonNetwork.CurrentRoom.Players[instantiateProperties.PlayerId];
            var actorNumber = instantiateProperties.PlayerId;

            if (!teamBuilder.TryGetTeamByPlayer(actorNumber, out TeamInfo team)) {
                yield return new WaitUntil(() => teamBuilder.TryGetTeamByPlayer(actorNumber, out team));
            }

            if (!team.TryGetByName(player.NickName, out var playerVoiceInfo)) {
                throw new Exception($"Player info for player '{player.NickName}' wasn't found in team");
            }

            playerVoiceInfo.Initialize(voiceInfoComponents);
            
            transform.SetParent(teamBuilder.transform);
            gameObject.name = player.NickName;

            SetGroupOfInterest(team, playerVoiceInfo);
            RegisterInGroupSpeaker();
            
            DrawSpeaker(playerVoiceInfo);
        }

        private void SetGroupOfInterest(TeamInfo teamInfo, PlayerVoiceInfo voiceInfo) {
            var voiceInterestGroupsHandler = ObjectExtensions.FindSingleInScene<VoiceInterestGroupsHandler>();
            voiceInfoController.Initialize(voiceInterestGroupsHandler, voiceInfo, teamInfo.IsLocal);
            
            if (teamInfo.IsLocal && !voiceInfo.Player.IsLocal) {
                voiceInterestGroupsHandler.AddGroup(in voiceInfo.DefaultRecordGroup);
            }
        }

        private void RegisterInGroupSpeaker() {
            var groupSpeakerUi = ObjectExtensions.FindSingleInScene<GroupSpeakerController>();
            groupSpeakerUi.RegisterVoiceController(voiceInfoController);
        }
        
        private void DrawSpeaker(PlayerVoiceInfo voiceInfo) {
            var speakerDrawer = ObjectExtensions.FindSingleInScene<SpeakersPanelDrawer>();
            speakerDrawer.DrawSpeaker(voiceInfoController);
        }


#if UNITY_EDITOR

        private void OnValidate() {
            if (ReferenceEquals(photonView, null)) {
                photonView = transform.GetComponentInChildren<PhotonView>(true);
            }
            
            if (ReferenceEquals(photonVoiceView, null)) {
                photonVoiceView = transform.GetComponentInChildren<PhotonVoiceView>(true);
            }
            
            if (ReferenceEquals(audioSource, null)) {
                audioSource = transform.GetComponentInChildren<AudioSource>(true);
            }
            
            if (ReferenceEquals(voiceInfoController, null)) {
                voiceInfoController = transform.GetComponentInChildren<PlayerVoiceInfoController>(true);
            }
        }

#endif
    }
}