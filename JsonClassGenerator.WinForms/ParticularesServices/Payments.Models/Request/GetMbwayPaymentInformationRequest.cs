namespace Payments.Models.Request
{
    public class GetMbwayPaymentInformationRequest
    {
        public string CardNumber { get; set; }
        public string OperationId { get; set; }
        public string SibsOperationId { get; set; }
    }

}