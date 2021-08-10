using Newtonsoft.Json;
using System;

namespace Payments.Business.Models.Input
{
    public class GetMbwayPaymentInformationInput
    {
        [JsonProperty("cardNumber")]
        public string CardNumber { get; set; }

        [JsonProperty("operationId")]
        public string OperationId { get; set; }

        [JsonProperty("sibsOperationId")]
        public string SibsOperationId { get; set; }
    }

}