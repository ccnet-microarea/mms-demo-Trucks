using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.Models
{
    public class BaseModel<T>
    {
        [JsonProperty("value")]
        public T value { get; set; }
        [JsonProperty("isReadOnly")]
        public bool IsReadOnly { get; set; } = false;
        [JsonProperty("isHide")]
        public bool IsHide { get; set; } = false;
        [JsonProperty("mandatory")]
        public bool Mandatory { get; set; } = false;
    }
}
