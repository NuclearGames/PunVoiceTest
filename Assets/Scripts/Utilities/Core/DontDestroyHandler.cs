using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities.Core {
    public class DontDestroyHandler : MonoBehaviour {
        [SerializeField]
        private bool dontDestroyOnlyComponent;
        
        private void Awake() {
            var obj = dontDestroyOnlyComponent ? (Object)this : gameObject;
            DontDestroyOnLoad(obj);
        }
    }
}