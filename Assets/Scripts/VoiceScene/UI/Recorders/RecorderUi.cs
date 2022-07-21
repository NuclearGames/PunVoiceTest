using System;
using System.Collections.Generic;
using Networks;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using VoiceScene.Logic.Controller;

namespace VoiceScene.UI.Recorders {
    internal sealed class RecorderUi: MonoBehaviour {
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

        private Recorder _recorder;
        private PlayerVoiceInfoController _voiceInfoController;

        private void Start() {
            lockMicroContainer.button.onClick.AddListener(LockMicroCall);
            teamMicroContainer.button.onClick.AddListener(SetUpMicroForTeam);
            allMicroContainer.button.onClick.AddListener(SetUpMicroForAll);
        }

        internal void Initialize(PlayerVoiceInfoController localPlayerVoiceController) {
            _voiceInfoController = localPlayerVoiceController;
            _recorder = _voiceInfoController.NetworkVoiceInfo.Components.Recorder;
            
            LockMicroCall();
        }

        private void LockMicroCall() {
            DeactivateMic();

            DeactivateButton(0);
        }

        private void SetUpMicroForTeam() {
            _recorder.InterestGroup = _voiceInfoController.NetworkVoiceInfo.DefaultRecordGroup;
            ActivateMic();
            
            DeactivateButton(1);
        }
        
        private void SetUpMicroForAll() {
            _recorder.InterestGroup = 0;
            ActivateMic();
            
            DeactivateButton(2);
        }

        private void ActivateMic() {
            StartCoroutine(VoiceStateHandler.Instance.PerformAction(_recorder.StartRecording));
        }
        
        private void DeactivateMic() {
            _recorder.StopRecording();
        }

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