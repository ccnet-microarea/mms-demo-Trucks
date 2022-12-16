using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.ParametersModel
{
    public class ExtractDataResponse : BaseResponse
    {

    }

    public class ExtractDataRequest : BaseRequest
    {
        /// <summary>
        /// AllAreas
        /// </summary>
        [JsonProperty("AllAreas")]
        public bool AllAreas { get; set; }

        /// <summary>
        /// SelectArea
        /// </summary>
        [JsonProperty("SelectArea")]
        public bool SelectArea { get; set; }
        /// <summary>
        /// FromArea
        /// </summary>
        [JsonProperty("FromArea")]
        public string FromArea { get; set; }

        /// <summary>
        /// ToArea
        /// </summary>
        [JsonProperty("ToArea")]
        public string ToArea { get; set; }

    }
}
