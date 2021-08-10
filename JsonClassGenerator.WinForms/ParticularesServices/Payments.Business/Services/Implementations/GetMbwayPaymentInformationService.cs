using System;
using System.Threading.Tasks;
using Common.Business.Service.Managers;
using Common.Models.Enums;
using Common.Models.Output;
using ParticularesServices.Services.Payment;
using Payments.Business.Models.Input;
using Payments.Business.Models.Output;

namespace Payments.Business.Services.Implementations
{
    public class GetMbwayPaymentInformationService : BaseService
    {

        protected override async Task<object> Invoke()
        {
            var input = (GetMbwayPaymentInformationInput)BaseInputRequest ?? throw new ArgumentNullException(nameof(BaseInputRequest));
            return await ServiceClientHttp.PostAsync<ChallengesOutput>(UrlHelper.GetBaseUriByChannel(PaymentUrlService.GetMbwayPaymentInformation),
                                                                                input,
                                                                                UrlHelper.FillHeader());
        }

        protected override object InvokeMock()
        {
            var challengeResult = new ChallengesOutput
            {
                TokenId = "2",
                ChallengeList = new[]
                {
                    new ChallengeList
                    {
                        Type = (int) ClientChallengeType.SMS, Value = string.Empty
                    },
                    new ChallengeList
                    {
                        Type = (int) ClientChallengeType.Pin, Value = string.Empty
                    }
                }
            };
            return challengeResult;
        }

        protected override object InvokeMockWithChallenge() => GetMbwayPaymentInformationOutput.GetOutputData();

        protected override async Task<object> InvokeWithChallenge()
        {
            return await ServiceClientHttp.PostAsync<GetMbwayPaymentInformationOutput>(UrlHelper.GetBaseUriByChannel(PaymentUrlService.GetMbwayPaymentInformation),
                                                                                new object(),
                                                                                UrlHelper.FillHeader(challenge: InputWithChallenge));
        }
    }
}

