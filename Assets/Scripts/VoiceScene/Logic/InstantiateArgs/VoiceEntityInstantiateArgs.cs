using System;

namespace VoiceScene.Logic.InstantiateArgs {
    internal readonly struct VoiceEntityInstantiateArgs {
        private const int SIZE = 4;
        
        internal readonly int PlayerId;

        internal VoiceEntityInstantiateArgs(int playerId) {
            PlayerId = playerId;
        }
        
        public static explicit operator byte[] (VoiceEntityInstantiateArgs properties) {
            // var byteArray = new byte[SIZE];
            // int offset = 0;
            // properties.PlayerId.NetworkSerialize(byteArray, ref offset);


            var byteArray = BitConverter.GetBytes(properties.PlayerId);
            return byteArray;
        }

        public static implicit operator VoiceEntityInstantiateArgs(byte[] source) {
            var offset = 0;
            // source.NetworkDeserialize(out int playerId, ref offset);
            var playerId = BitConverter.ToInt32(source, offset);

            return new VoiceEntityInstantiateArgs(playerId);
        }
    }
}