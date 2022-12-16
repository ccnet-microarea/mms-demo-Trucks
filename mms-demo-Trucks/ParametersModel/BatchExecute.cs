using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MMSDemoTrucks.Models;
namespace MMSDemoTrucks.ParametersModel
{
    public class BatchExecuteRequest : BaseRequest
    {
        /// <summary>
        /// ParamIn
        /// </summary>
        [JsonProperty("ParamIn")]
        public List<FP_Areas_Trucks> ParamIn { get; set; }
    }

    public class BatchExecuteResponse : BaseResponse
    {
        //add your custom properties here to be returned back to tbServer
    }
}
