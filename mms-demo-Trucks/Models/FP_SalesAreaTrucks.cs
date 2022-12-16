using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.Models
{
    public class FP_SalesAreaTrucks
    {
        [JsonProperty("Area")]
        public string Area { get; set; }

        [JsonProperty("TruckCode")]
        public string TruckCode { get; set; }
    }
}
