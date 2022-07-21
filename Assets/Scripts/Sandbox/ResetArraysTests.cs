using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Sandbox {
    public class ResetArraysTests : MonoBehaviour {
        private void Start() {
            var firstArray = new byte[4] { 0, 1, 2, 3 };
            var secondArray = new byte[] { 4, 5, 6 };

            var map = new Dictionary<byte, object>();
            map.Add(0, firstArray);
            map.Add(1, secondArray);

            firstArray = secondArray;
            
            Assert.AreNotEqual(map[0], map[1]);
            Assert.AreNotEqual(((byte[])map[0]).Length, ((byte[])map[1]).Length);
            
            Assert.AreEqual(firstArray.Length, secondArray.Length);
            Assert.AreEqual(3, firstArray.Length);
        }
    }
}