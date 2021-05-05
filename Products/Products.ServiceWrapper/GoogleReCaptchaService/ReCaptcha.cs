using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Products.ServiceWrapper.GoogleReCaptchaService
{
    public class ReCaptcha
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }
    }
}
