using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Networks;
using Photon.Voice.PUN;
using UnityEngine;
using Utilities.Extensions;

namespace VoiceScene.Logic {
    internal sealed class VoiceInterestGroupsHandler : MonoBehaviour {
        [SerializeField]
        private float repeatTime;

        private bool _isDisabled;
        private bool _wasModified;
        private byte[] _currentListenGroups;
        private readonly HashSet<byte> _groupsToListen = new HashSet<byte>();

        private WaitForSeconds _repeatWait;
        
        // internal void SetUpInitialInterestGroups(IEnumerable<byte> enableGroups) {
        //     _groupsToListen.Clear();
        //     enableGroups.ForEach(group => _groupsToListen.Add(group));
        //     
        //     _wasModified = true;
        // }

#region Monobehaviour

        private void Start() {
            _repeatWait = new WaitForSeconds(repeatTime);
            StartCoroutine(SetInterestGroupsCoroutine());
        }

        private void OnDisable() {
            _isDisabled = true;
        }

#endregion

#region Groups modify operation

        /// <summary>
        /// Добавить группу
        /// </summary>
        internal bool AddGroup(in byte group) {
            var wasModified = AddGroupInternal(in group);
            if (wasModified) {
                _wasModified = true;
            }

            return wasModified;
        }

        /// <summary>
        /// Добавить несколько групп
        /// </summary>
        internal bool AddGroups(params byte[] groups) {
            bool wasModified = false;
            groups.ForEach(group => {
                wasModified |= AddGroupInternal(in group);
            });

            if (wasModified) {
                _wasModified = true;   
            }

            return wasModified;
        }

        private bool AddGroupInternal(in byte group) {
            if (_groupsToListen.Contains(group)) {
                return false;
            }

            _groupsToListen.Add(group);

            return true;
        }

        /// <summary>
        /// Удаляет группу
        /// </summary>
        internal bool RemoveGroup(in byte group) {
            var wasModified = RemoveGroupInternal(in group);

            if (wasModified) {
                _wasModified = true;   
            }

            return wasModified;
        }

        /// <summary>
        /// Удаляет несколько групп
        /// </summary>
        internal bool RemoveGroups(params byte[] groups) {
            bool wasModified = false;
            groups.ForEach(group => {
                wasModified |= RemoveGroupInternal(in group);
            });

            if (wasModified) {
                _wasModified = true;   
            }

            return wasModified;
        }

        private bool RemoveGroupInternal(in byte group) {
            return _groupsToListen.Remove(group);
        }

#endregion

#region Set Interest to server

        private IEnumerator SetInterestGroupsCoroutine() {
            while (!_isDisabled) {
                yield return _repeatWait;
            
                SetInterestGroups();   
            }
        }

        private void SetInterestGroups() {
            if (!_wasModified) {
                return;
            }

            var newListenGroups = _groupsToListen.ToArray();
            
            Debug.Log($"Changing groups of interest: [{string.Join(";", newListenGroups)}]");
            StartCoroutine(VoiceStateHandler.Instance.PerformAction(() => PhotonVoiceNetwork.Instance.Client.OpChangeGroups(_currentListenGroups, newListenGroups)));
            
            _currentListenGroups = newListenGroups;
            
            _wasModified = false;
        }

        internal void Unmute() {
            _wasModified = true;
        }
        
        internal void MuteAll() {
            _wasModified = false;
            Debug.Log("Locking voice network as there are no clients to listen!");
            
            if(PhotonVoiceNetwork.Instance.Client.IsConnected) {
                PhotonVoiceNetwork.Instance.Disconnect();
            }
        }

#endregion
    }
}