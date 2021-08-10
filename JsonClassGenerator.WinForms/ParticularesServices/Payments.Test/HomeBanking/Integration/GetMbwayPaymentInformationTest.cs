using Common.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using Payments.Models.Request;
using Payments.Models.Result;
using Utilities;

namespace Payments.Tests.HomeBanking.Integration
{
    [TestFixture]
    internal class GetMbwayPaymentInformationTest : BaseAutenticatedCAHomebankingIntegrationTest
    {
        private GetMbwayPaymentInformationRequest _request = new GetMbwayPaymentInformationRequest.GetInputData();

        [Test]
        public async Task CAHomeBanking_GetMbwayPaymentInformationTest_should_return_ChallengeResult()
        {
            var act = await CAHomeBanking.PaymentService.GetMbwayPaymentInformation(_request);

            Assert.IsInstanceOf<ChallengeData>(act);
        }

        [Test]
        public async Task CAHomeBanking_GetMbwayPaymentInformationTest_should_return_GetMbwayPaymentInformationResult()
        {
            var resultWithChallenge = await CAHomeBanking.PaymentService.GetMbwayPaymentInformation(_request);

            resultWithChallenge.ChallengeList = ChallengeHelper.ValidateChallenge(resultWithChallenge.ChallengeList);

            var act = await CAHomeBanking.PaymentService.GetMbwayPaymentInformationConfirmation(resultWithChallenge);

            Assert.IsInstanceOf<GetMbwayPaymentInformationResult>(act);
        }
    }
}