using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.Models
{
    public class SalesAreas
    {
        [JsonProperty("Area")]
        public string Area { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}
