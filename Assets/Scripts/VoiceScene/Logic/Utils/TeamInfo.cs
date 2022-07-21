using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;

namespace VoiceScene.Logic.Utils {
    /// <summary>
    /// Сущность "команды". Предоставляет список Voice-инфы по каждому игроку
    /// </summary>
    internal sealed class TeamInfo : ICollection<PlayerVoiceInfo> {
        internal IReadOnlyCollection<PlayerVoiceInfo> Players => _playersCollection.Values;

        /// <summary>
        /// Максимальное кол-во участников в команде
        /// <para>От этого числа зависит группа интересов</para>
        /// </summary>
        internal readonly int MaxSize;
        
        internal bool IsLocal { get; private set; }
        
        private readonly Dictionary<string, PlayerVoiceInfo> _playersCollection;

        internal TeamInfo(int size = 0) {
            MaxSize = size;
            _playersCollection = new Dictionary<string, PlayerVoiceInfo>(size);
        }

        internal bool TryGetByName(string nickName, out PlayerVoiceInfo info) {
            return _playersCollection.TryGetValue(nickName, out info);
        }

#region ICollection

        public int Count => _playersCollection.Count;
        
        public bool IsReadOnly => false;
        
        public void Add(PlayerVoiceInfo playerInfo) {
            if (playerInfo == null) {
                throw new NullReferenceException("Can't add empty null-player!");
            }

            if (playerInfo.Player.IsLocal) {
                IsLocal = true;
            }
            
            _playersCollection.Add(playerInfo.Player.NickName, playerInfo);
        }

        public void Clear() {
            _playersCollection.Clear();
        }

        public bool Contains(PlayerVoiceInfo item) {
            if (item == null) {
                return false;
            }
            
            return _playersCollection.ContainsKey(item.Player.NickName);
        }

        public void CopyTo(PlayerVoiceInfo[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(PlayerVoiceInfo item) {
            if (item == null) {
                return false;
            }

            return _playersCollection.Remove(item.Player.NickName);
        }

#region Enumerator

        public IEnumerator<PlayerVoiceInfo> GetEnumerator() {
            return Players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#endregion

#endregion

        
    }
}