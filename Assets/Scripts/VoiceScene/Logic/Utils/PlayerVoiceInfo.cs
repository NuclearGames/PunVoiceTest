using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

namespace VoiceScene.Logic.Utils {
    /// <summary>
    /// Сущность-агрегация, которая связывает конкретного Player и его PhotonView-компоненты
    /// </summary>
    internal sealed class PlayerVoiceInfo {

        internal event Action onInitialize;
        
        internal readonly Player Player;
        internal readonly int TeamIndex;
        internal readonly byte DefaultRecordGroup;
        
        internal bool IsInitialized { get; private set; }
        
        internal PlayerVoiceInfoComponents Components { get; private set; }
        
        internal PlayerVoiceInfo(Player player, int teamIndex, byte defaultRecordGroup) {
            Player = player;
            TeamIndex = teamIndex;
            DefaultRecordGroup = defaultRecordGroup;
        }

        internal void Initialize(PlayerVoiceInfoComponents components) {
            Components = components;
            onInitialize?.Invoke();

            IsInitialized = true;
        }
        
#region Utils

        internal class PlayerVoiceInfoComponents {
            internal PhotonView PhotonView { get; set; }
            internal PhotonVoiceView PhotonVoiceView { get; set; }
            internal AudioSource AudioSource { get; set; }
            
            internal Recorder Recorder {
                get {
                    if (ReferenceEquals(_recorder, null)) {
                        if (ReferenceEquals(PhotonVoiceView, null)) {
                            throw new NullReferenceException("Is not Initialized yet!");
                        }

                        _recorder = PhotonVoiceView.RecorderInUse;
                    }

                    return _recorder;
                }
            }

            private Recorder _recorder;
        }

#endregion
    }
}