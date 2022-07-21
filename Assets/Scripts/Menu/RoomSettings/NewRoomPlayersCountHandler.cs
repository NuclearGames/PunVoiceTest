using System;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Core;

namespace Menu.RoomSettings {
    internal sealed class NewRoomPlayersCountHandler : MonoBehaviour {
        internal event Action<int> onPlayersCountChanged;
        
        [SerializeField]
        private Text playersCountText;
        [SerializeField]
        private string defaultPlayersCountText;
        [Space]
        [SerializeField]
        private Slider playerCountSlider;

        internal byte PlayersCount { get; private set; }
        
        private void Start() {
            onPlayersCountChanged += SetText;
            onPlayersCountChanged += (value) => PlayersCount = Convert.ToByte(value);
            
            playerCountSlider.onValueChanged.AddListener(HandlePlayersCountChange);
            
            playerCountSlider.value = playerCountSlider.minValue;
            HandlePlayersCountChange(playerCountSlider.minValue);
        }

        private void HandlePlayersCountChange(float value) {
            var intValue = Mathf.RoundToInt(value);
            
            onPlayersCountChanged?.Invoke(intValue);
        }

        private void SetText(int value) {
            playersCountText.text = $"{defaultPlayersCountText}: {value}";
        }
    }
}