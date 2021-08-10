using System;
using System.Threading.Tasks;
using Common.Business.Service.Managers;
using Common.Models.Enums;
using Common.Models.Output;
using ParticularesServices.Services.$Area$;
using $Product$.Business.Models.Input;
using $Product$.Business.Models.Output;

namespace $Product$.Business.Services.Implementations
{
    public class $ClassName$Service : BaseService
    {

        protected override async Task<object> Invoke()
        {
            var input = ($ClassName$Input)BaseInputRequest ?? throw new ArgumentNullException(nameof(BaseInputRequest));
            return await ServiceClientHttp.$Type$Async<ChallengesOutput>(UrlHelper.GetBaseUriByChannel($Area$UrlService.$ClassName$),
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

        protected override object InvokeMockWithChallenge() => $ClassName$Output.GetOutputData();

        protected override async Task<object> InvokeWithChallenge()
        {
            return await ServiceClientHttp.$Type$Async<$ClassName$Output>(UrlHelper.GetBaseUriByChannel($Area$UrlService.$ClassName$),
                                                                                new object(),
                                                                                UrlHelper.FillHeader(challenge: InputWithChallenge));
        }
    }
}

