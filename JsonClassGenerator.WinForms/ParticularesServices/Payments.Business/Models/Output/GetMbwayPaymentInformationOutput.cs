namespace Payments.Models.Result
{
    public class GetMbwayPaymentInformationOutput
    {
        [JsonProperty("sibsOperationId")]
        public string SibsOperationId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

}