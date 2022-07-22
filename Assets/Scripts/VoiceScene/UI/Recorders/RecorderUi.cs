using System;
using System.Collections.Generic;
using Networks;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using VoiceScene.Logic;
using VoiceScene.Logic.Controller;

namespace VoiceScene.UI.Recorders {
    internal sealed class RecorderUi: MonoBehaviour {
        [SerializeField]
        private GroupSpeakerController groupSpeakerController;
        
        [Space]
        [SerializeField]
        private ButtonContainer lockMicroContainer;
        [SerializeField]
        private ButtonContainer teamMicroContainer;
        [SerializeField]
        private ButtonContainer allMicroContainer;

        [Space]
        [SerializeField]
        private Color selectedColor;
        [SerializeField]
        private Color normalColor;

        private IEnumerable<ButtonContainer> GetButtonContainers() {
            yield return lockMicroContainer;
            yield return teamMicroContainer;
            yield return allMicroContainer;
        }

        private int _lastState = -1;
        private Recorder _recorder;
        private PlayerVoiceInfoController _voiceInfoController;
        private readonly IDictionary<int, Action> _stateActionIndexMap = new Dictionary<int, Action>();

        internal void Initialize(PlayerVoiceInfoController localPlayerVoiceController) {
            _voiceInfoController = localPlayerVoiceController;
            _recorder = _voiceInfoController.NetworkVoiceInfo.Components.Recorder;
            
            LockMicroCall();
        }

        private void LockMicroCall() {
            DeactivateMic();

            _lastState = 0;
            DeactivateButton(_lastState);
        }

        private void SetUpMicroForTeam() {
            _recorder.InterestGroup = _voiceInfoController.NetworkVoiceInfo.DefaultRecordGroup;
            ActivateMic();
            
            _lastState = 1;
            DeactivateButton(_lastState);
        }
        
        private void SetUpMicroForAll() {
            _recorder.InterestGroup = 0;
            ActivateMic();
            
            _lastState = 2;
            DeactivateButton(_lastState);
        }

        private void ActivateMic() {
            if (!groupSpeakerController.AnyListening) {
                return;
            }
            
            _recorder.TransmitEnabled = true;
            StartCoroutine(VoiceStateHandler.Instance.PerformAction(_recorder.StartRecording));
        }
        
        private void DeactivateMic() {
            _recorder.TransmitEnabled = false;
            _recorder.StopRecording();
        }
        
#region Monobehaviours

        private void Start() {
            lockMicroContainer.button.onClick.AddListener(LockMicroCall);
            teamMicroContainer.button.onClick.AddListener(SetUpMicroForTeam);
            allMicroContainer.button.onClick.AddListener(SetUpMicroForAll);

            _stateActionIndexMap.Add(0, LockMicroCall);
            _stateActionIndexMap.Add(1, SetUpMicroForTeam);
            _stateActionIndexMap.Add(2, SetUpMicroForAll);

            VoiceStateHandler.Instance.onConnected += OnPhotonVoiceConnected;
        }

        private void OnDestroy() {
            if(!ReferenceEquals(VoiceStateHandler.Instance, null)) {
                VoiceStateHandler.Instance.onConnected -= OnPhotonVoiceConnected;
            }
        }

#endregion

#region Subscriptions

        private void OnPhotonVoiceConnected() {
            if (_lastState == -1) {
                return;
            }

            if (_stateActionIndexMap.TryGetValue(_lastState, out var action)) {
                action();
            }
        }

#endregion

#region Utils

        private void DeactivateButton(int buttonIndex) {
            int index = 0;
            foreach (var container in GetButtonContainers()) {
                bool selected = (buttonIndex == index);
                container.button.interactable = !selected;
                container.image.color = selected ? selectedColor : normalColor;
                index++;
            }
        }

#endregion
        
        [Serializable]
        private class ButtonContainer {
            [SerializeField]
            internal Button button;
            
            [SerializeField]
            internal Image image;
        }
        
        
#if UNITY_EDITOR
        private void OnValidate() {
            if (ReferenceEquals(groupSpeakerController, null)) {
                groupSpeakerController = FindObjectOfType<GroupSpeakerController>();
            }
            
            ValidateContainer(lockMicroContainer);
            ValidateContainer(teamMicroContainer);
            ValidateContainer(allMicroContainer);
        }

        void ValidateContainer(ButtonContainer container) {
            if (!ReferenceEquals(container.button, null)) {
                if (ReferenceEquals(container.image, null)) {
                    container.image = container.button.GetComponent<Image>();
                }
            }
        }

#endif  
    }
}