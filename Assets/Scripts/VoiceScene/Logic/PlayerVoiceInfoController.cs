using System;
using UnityEngine;
using VoiceScene.Logic.Utils;

namespace VoiceScene.Logic {
    /// <summary>
    /// Элемнет управления воспроизведением для конкретного игрока
    /// <para>Биндит UI с <seealso cref="PlayerVoiceInfo"/></para>
    /// </summary>
    internal sealed class PlayerVoiceInfoController : MonoBehaviour {
        internal event Action<bool> onMuteStateChanged;
        
        /// <summary>
        /// Игрок
        /// </summary>
        internal PlayerVoiceInfo NetworkVoiceInfo { get; private set; }

        /// <summary>
        /// Прослушивается игрок или нет
        /// </summary>
        internal bool Muted {
            get => _muted;
            private set {
                if (_muted == value) {
                    return;
                }

                _muted = value;
                onMuteStateChanged?.Invoke(_muted);
            }
        }

        private bool _muted = false;
        private bool _isLocalTeam = false;
        private VoiceInterestGroupsHandler _voiceInterestGroupsHandler;
        
        /// <summary>
        /// Инициализация компонента
        /// </summary>
        internal void Initialize(VoiceInterestGroupsHandler voiceInterestGroupsHandler, PlayerVoiceInfo voiceInfo, bool isLocalTeam) {
            _voiceInterestGroupsHandler = voiceInterestGroupsHandler;
            NetworkVoiceInfo = voiceInfo;

            _isLocalTeam = isLocalTeam;
            
            onMuteStateChanged += OnMuteStateChanged;

            if (NetworkVoiceInfo.IsInitialized) {
                OnNetworkInfoInitialized();
            } else {
                NetworkVoiceInfo.onInitialize += OnNetworkInfoInitialized;
            }
        }

        /// <summary>
        /// Запрос измеения статуса
        /// </summary>
        internal void ChangeMuteState() {
            if (NetworkVoiceInfo.Player.IsLocal) {
                Debug.Log("Local player can't here it's own voice!");
                return;
            }
            
            Muted = !Muted;
        }

#region Subscription

        private void OnNetworkInfoInitialized() {
            Muted = NetworkVoiceInfo.Player.IsLocal;
            NetworkVoiceInfo.onInitialize -= OnNetworkInfoInitialized;
        }

        private void OnMuteStateChanged(bool muted) {
            // В любом случае изменяем состояние Мьюта на АудиоСорсе
            if (!ReferenceEquals(NetworkVoiceInfo.Components.AudioSource, null)) {
                NetworkVoiceInfo.Components.AudioSource.mute = muted;
            }

            // В том случае, если чел в нашей команде, то тогда изменяем соответсвующим образом группу интересов
            if (!_isLocalTeam || NetworkVoiceInfo.Player.IsLocal) {
                return;
            }

            Debug.Log($"Try change mute state to '{muted}' for player '{NetworkVoiceInfo.Player.ActorNumber}'");
            
            if (muted) {
                if (!_voiceInterestGroupsHandler.RemoveGroup(in NetworkVoiceInfo.DefaultRecordGroup)) {
                    Debug.LogError($"We've tried to remove group '{NetworkVoiceInfo.DefaultRecordGroup}', that wasn't in out group of interest!");
                }
            } else {
                if (!_voiceInterestGroupsHandler.AddGroup(in NetworkVoiceInfo.DefaultRecordGroup)) {
                    Debug.LogError($"We've tried to add group '{NetworkVoiceInfo.DefaultRecordGroup}', that has already been in our group of interest!");
                }
            }
        }

#endregion
    }
}