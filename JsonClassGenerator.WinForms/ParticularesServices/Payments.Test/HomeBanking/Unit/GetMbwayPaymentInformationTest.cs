using Common.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using Payments.Models.Request;
using Payments.Models.Result;
using Utilities;

namespace Payments.Tests.HomeBanking.Unit
{
    [TestFixture]
    internal class GetMbwayPaymentInformationTest : BaseCAMobileUnitTest
    {
        private GetMbwayPaymentInformationRequest _request = new GetMbwayPaymentInformationRequest.GetInputData();

        [Test]
        public async Task CAMobile_GetMbwayPaymentInformationTest_should_return_ChallengeResult()
        {
            var act = await CAMobile.PaymentService.GetMbwayPaymentInformation(_request);

            Assert.IsInstanceOf<ChallengeData>(act);
        }

        [Test]
        public async Task CAMobile_GetMbwayPaymentInformationTest_should_return_GetMbwayPaymentInformationResult()
        {
            var resultWithChallenge = await CAMobile.PaymentService.GetMbwayPaymentInformation(_request);

            var act = await CAMobile.PaymentService.GetMbwayPaymentInformationConfirmation(resultWithChallenge);

            Assert.IsInstanceOf<GetMbwayPaymentInformationResult>(act);
        }
    }
}