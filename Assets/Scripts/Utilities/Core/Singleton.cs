using UnityEngine;
using Utilities.Extensions;

namespace Utilities.Core {
    /// <summary>
    /// Паттерн "одиночка"
    /// </summary>
    internal class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        [SerializeField]
        private bool destroyOnlyComponent;
        
        /// <summary>
        /// Единственный экземпляр класса
        /// </summary>
        public static T Instance {
            get => _instance == null ? FindInstance() : _instance ;
            private protected set => _instance = value;
        }
        private static T _instance;
        private bool _isInitialized;

        private protected virtual void Awake() {
            if (Instance != this) {
                Destroy(destroyOnlyComponent ? (Object)this : gameObject);
            } else if (!_isInitialized) {
                SetUp();
            } 
        }

        /// <summary>
        /// Returns a single object
        /// </summary>
        private void SetUp() {
            _isInitialized = true;
        }

        private static T FindInstance() {
            return _instance = ObjectExtensions.FindSingleInScene<T>(true) ?? FindObjectOfType<T>();
        }
    }
}