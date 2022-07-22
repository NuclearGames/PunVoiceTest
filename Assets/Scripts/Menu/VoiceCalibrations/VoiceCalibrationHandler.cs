using System;
using System.Collections;
using System.Threading.Tasks;
using Menu.Networking;
using Networks;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.VoiceCalibrations {
    internal sealed class VoiceCalibrationHandler : BasePhotonConnectionHandler {
        [Header("Settings")]
        [SerializeField]
        private int calibrateMs = 2000;
            
        [Header("UI")]
        [SerializeField]
        private Button calibrationStartBtn;
        [SerializeField]
        private Slider calibrationSlider;

        [Header("Lockers")]
        [SerializeField]
        private GameObject preparePanel;
        [SerializeField]
        private GameObject calibratingPanel;
        

        private protected override void InitializeInternal() {
            CreateRoomOptions.MaxPlayers = 1;
            CreateRoomOptions.PlayerTtl = 1;
            CreateRoomOptions.EmptyRoomTtl = 1;
            
            calibrationStartBtn.onClick.AddListener(StartCalibrate);

            calibrationSlider.minValue = 0;
            calibrationSlider.maxValue = 1;
            calibrationSlider.value = PhotonVoiceNetwork.Instance.PrimaryRecorder.VoiceDetectionThreshold;
            
            calibrationSlider.onValueChanged.AddListener(SliderValueChanged);
        }

        private protected override void ResetConnectionStatesInternal() {
            preparePanel.SetActive(false);
            calibratingPanel.SetActive(false);
        }

#region Actions

        private void StartCalibrate() {
            OnSuccessConnectAction = CreateRoom;
            
            ConnectToPhoton();
            StartCoroutine(ConnectionStateHandler());
            
            preparePanel.SetActive(true);
        }

        private void SliderValueChanged(float value) {
            PhotonVoiceNetwork.Instance.PrimaryRecorder.VoiceDetectionThreshold = value;
        }

#endregion

#region PhotonNetwork

        private bool CreateRoom() {
            return CreateRoomInternal(PhotonNetwork.LocalPlayer.UserId);
        }

#region Callbacks

        private protected override void OnJoinedRoomInternal() {
            StartCoroutine(VoiceStateHandler.Instance.PerformAction(Calibrate));
        }

#endregion

#endregion

#region Photon voice Network

        private void Calibrate() {
            preparePanel.SetActive(false);
            calibratingPanel.SetActive(true);
            {
                var recorder = PhotonVoiceNetwork.Instance.PrimaryRecorder;
                recorder.StartRecording();
                recorder.VoiceDetectorCalibrate(calibrateMs, CalibrateCallback);
            }

            StartCoroutine(CalibrationBackground());
        }

        private void CalibrateCallback(float value) {
            Debug.Log("Calibrate callback handled!");
            _newThresholdValue = value;
        }

        private float _newThresholdValue;
        
        private IEnumerator CalibrationBackground() {
            var recorder = PhotonVoiceNetwork.Instance.PrimaryRecorder;
            
            yield return new WaitForSeconds((calibrateMs / 1000f));
            yield return new WaitUntil(() => !recorder.VoiceDetectorCalibrating);
            
            Debug.Log("Calibrate forced exited! Call disconnect!");
            
            calibratingPanel.SetActive(false);
            calibrationSlider.value = _newThresholdValue; 
            
            recorder.StopRecording();
            PhotonNetwork.Disconnect();
        }

#endregion
    }
}