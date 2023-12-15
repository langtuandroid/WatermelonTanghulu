using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace CookApps
{
    [Serializable]
    public class SampleData
    {
        //───────────────────────────── public Serialize 가능
        public int publicIntFieldValue = 0;

        public string publicStringFieldValue = "publicStringFieldValue";
        public string publicStringPropertyValue { get; set; } = "publicStringPropertyValue";

        public Dictionary<int, string> _dictionary = new();
        public List<string> _list = new();

        //───────────────────────────── [JsonProperty]를 붙이면 internal, private도 Serialize 가능
        [JsonProperty]
        internal int internalIntFieldValue = 1;

        [JsonProperty("psf")] //serialize를 psf라는 이름으로 함
        private string privateStringFieldValue = "privateStringFieldValue";

        //───────────────────────────── 기본적으로 private / internal은 Serialize 불가능
        private int privateIntFieldValue = 2;
        internal string internalStringFieldValue = "internalStringFieldValue";
        internal string internalStringPropertyValue { get; set; } = "internalStringPropertyValue";
        private string privateStringPropertyValue { get; set; } = "privateStringPropertyValue";

        //───────────────────────────── [NonSerialized]는 의도적인 Serialize 불가능
        [NonSerialized]
        public string nonSerializedValue = "nonSerializedValue";

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            Debug.Log($"OnSerializingMethod");
        }

        [OnSerialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            Debug.Log($"OnSerializedMethod");
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {
            Debug.Log($"OnDeserializingMethod");
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            Debug.Log($"OnDeserializedMethod");
        }
    }
}
