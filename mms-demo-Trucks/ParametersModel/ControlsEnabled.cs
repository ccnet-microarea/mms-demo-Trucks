using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MMSDemoTrucks.Models;

namespace MMSDemoTrucks.ParametersModel
{
    public class ControlsEnabledRequest : BaseRequest
    {
        /// <summary>
        /// ParamIn
        /// </summary>
        [JsonProperty("ParamIn")]
        public List<FP_Areas_Trucks_FullModel> ParamIn { get; set; }
    }
}
