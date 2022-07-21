using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities.Extensions;
using Utilities.Networking;
using VoiceScene.Logic.InstantiateArgs;
using VoiceScene.Logic.Utils;

namespace VoiceScene.Logic {
    /// <summary>
    /// Контроллер, распределяющий игроков по командам
    /// </summary>
    internal sealed class TeamsBuilder : MonoBehaviourPunCallbacks {
        [SerializeField]
        private string voicePrefabName;
        
        internal IReadOnlyCollection<TeamInfo> Teams => _teams;

        private readonly List<TeamInfo> _teams = new List<TeamInfo>();
        private readonly Dictionary<int, int> _playerTeamsMap = new Dictionary<int, int>();

        /// <summary>
        /// Возвращает команду по <paramref name="actorNumber"/> ее участника
        /// </summary>
        internal bool TryGetTeamByPlayer(int actorNumber, out TeamInfo team) {
            if (!_playerTeamsMap.TryGetValue(actorNumber, out var teamIndex)) {
                team = null;
                return false;
            }

            team = _teams[teamIndex];
            return true;
        }

#region Build teams

        /// <summary>
        /// Собирает команду у Мастер-клиента и публикует данные о принадлежности команды в свойства комнаты
        /// </summary>
        private void MasterClientSetUpTeams(int teamsCount = 2) {
            var players = PhotonNetwork.PlayerList;
            var maxTeamSize = (players.Length / teamsCount) + 1;
            
            for (int i = 0; i < players.Length; i++) {
                var teamIndex = i % teamsCount;

                TeamInfo workTeam;
                if (teamIndex == _teams.Count) {
                    workTeam = new TeamInfo(maxTeamSize);
                    _teams.Add(workTeam);
                } else {
                    workTeam = _teams[teamIndex];
                }

                var player = players[i];

                // TeamIndex * MaxTeamSize + (PlayerInTeamIndex + 1)
                byte interestGroup = Convert.ToByte(teamIndex * maxTeamSize + workTeam.Count + 1);
                var voiceInfo = new PlayerVoiceInfo(player, teamIndex, interestGroup);
                workTeam.Add(voiceInfo);
                
                _playerTeamsMap.Add(player.ActorNumber, teamIndex);
            }

            var hashTable = new Hashtable(teamsCount) {
                { RoomPropertyNames.TEAMS_COUNT, teamsCount },
                { RoomPropertyNames.TEAM_MAX_SIZE, maxTeamSize }
            };
            _teams.ForEach((team, teamIndex) => {
                team.ForEach(voiceInfo => {
                    var player = voiceInfo.Player;
                    hashTable.Add(RoomPropertyNames.GetPlayerTeamIndexName(player.ActorNumber), teamIndex);
                    hashTable.Add(RoomPropertyNames.GetPlayerInterestGroupName(player.ActorNumber), voiceInfo.DefaultRecordGroup);
                });
            });

            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable);

            InstantiateVoice(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        /// <summary>
        /// Собирает команду у клиента из опубликованных данных
        /// </summary>
        private void ClientSetUpTeams(int teamsCount, Hashtable changedProps) {
            var players = PhotonNetwork.CurrentRoom.Players;
            var maxTeamSize = (int)changedProps[RoomPropertyNames.TEAM_MAX_SIZE];

            for (int teamIndex = 0; teamIndex < teamsCount; teamIndex++) {
                _teams.Add(new TeamInfo(maxTeamSize));
            }
            
            players.ForEach(kvp => {
                var actorNumber = kvp.Key;
                
                byte interestGroup = (byte)changedProps[RoomPropertyNames.GetPlayerInterestGroupName(actorNumber)];
                var teamIndex = (int)changedProps[RoomPropertyNames.GetPlayerTeamIndexName(actorNumber)];
                
                var voiceInfo = new PlayerVoiceInfo(kvp.Value, teamIndex, interestGroup);
                _teams[teamIndex].Add(voiceInfo);
                
                _playerTeamsMap.Add(actorNumber, teamIndex);
            });
            
            InstantiateVoice(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        

#endregion

#region Network Prefab Instantiation

        private void InstantiateVoice(int actorNumber) {
            var args = new object[] {
                (byte[])(new VoiceEntityInstantiateArgs(actorNumber))
            };
            PhotonNetwork.Instantiate(voicePrefabName, Vector3.zero, Quaternion.identity, 0, args);
        }

#endregion
        
#region Monobehaviour

        public override void OnEnable() {
            if(!PhotonNetwork.IsMasterClient) {
                PhotonNetwork.AddCallbackTarget(this);
                
                // Проверяем, если мы поздно подключились, тогда данные уже раскиданы
                OnRoomPropertiesUpdate(PhotonNetwork.CurrentRoom.CustomProperties);
            }
        }

        private void Start() {
            if (PhotonNetwork.IsMasterClient) {
                MasterClientSetUpTeams();   
            }
        }
        
        public override void OnDisable() {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

#endregion

#region PunCallbacks

        public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
            if (!changedProps.ContainsKey(RoomPropertyNames.TEAMS_COUNT)) {
                return;
            }
            
            ClientSetUpTeams((int)changedProps[RoomPropertyNames.TEAMS_COUNT], changedProps);
        }

        public override void OnMasterClientSwitched(Player newMasterClient) {
            if (newMasterClient.IsLocal) {
                PhotonNetwork.RemoveCallbackTarget(this);
            } else {
                PhotonNetwork.AddCallbackTarget(this);
                if (_teams.Count == 0) {
                    MasterClientSetUpTeams();
                }
            }
        }

#endregion
    }
}