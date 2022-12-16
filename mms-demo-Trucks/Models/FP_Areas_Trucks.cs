using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.Models
{
    public class FP_Areas_Trucks
    {
        [JsonProperty("Area")]
        public string Area { get; set; }

        [JsonProperty("AreaDescription")]
        public string AreaDescription { get; set; }

        [JsonProperty("TruckCode")]
        public string TruckCode { get; set; }

        [JsonProperty("TruckDescription")]
        public string TruckDescription { get; set; }

        [JsonProperty("TruckPlate")]
        public string TruckPlate { get; set; }

        [JsonProperty("Selected")]
        public bool Selected { get; set; }
    }

    public class FP_Areas_Trucks_FullModel
    {
        [JsonProperty("Area")]
        public BaseModel<string> Area { get; set; }

        [JsonProperty("AreaDescription")]
        public BaseModel<string> AreaDescription { get; set; }

        [JsonProperty("TruckCode")]
        public BaseModel<string> TruckCode { get; set; }

        [JsonProperty("TruckDescription")]
        public BaseModel<string> TruckDescription { get; set; }

        [JsonProperty("TruckPlate")]
        public BaseModel<string> TruckPlate { get; set; }

        [JsonProperty("Selected")]
        public BaseModel<bool> Selected { get; set; }
    }
}
