﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSDemoTrucks.ParametersModel
{
    public class BaseResponse
    {
        /// <summary>
        /// ReturnValue
        /// </summary>
        [JsonProperty("retVal")]
        public object ReturnValue { get; set; }
        /// <summary>
        /// Success
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; } = true;
        /// <summary>
        /// ErrorMessage
        /// </summary>
        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorMessage ErrorMessage { get; set; }
    }
}
