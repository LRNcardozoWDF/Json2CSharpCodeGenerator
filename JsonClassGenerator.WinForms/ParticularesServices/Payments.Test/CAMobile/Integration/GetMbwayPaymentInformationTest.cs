using System;
using System.Threading.Tasks;
using Common.Models;
using NUnit.Framework;
using Payments.Models.Enums;
using Payments.Models.Request;
using Utilities;

namespace Payments.Tests.CAMobile.Integration
{
    [TestFixture]
    public class GetMbwayPaymentInformationTest : BaseAutenticatedCAMobileIntegrationTest
    {
        GetMbwayPaymentInformationRequest _inputData = new GetMbwayPaymentInformationRequest().GetInputData();

        [Test]
        public async Task CAMobile_GetMbwayPaymentInformationTest_should_return_ChallengeData()
        {
            var act = await CAMobile.PaymentService.GetMbwayPaymentInformation(_inputData);

            Assert.IsInstanceOf<ChallengeData>(act);
            Assert.IsInstanceOf<string>(act.TokenId);
        }

        [Test]
        public async Task CAMobile_GetMbwayPaymentInformationTest_ChallengeData_is_filled()
        {

            var act = await CAMobile.PaymentService.GetMbwayPaymentInformation(_inputData);

            Assert.IsNotNull(act.TokenId);
            Assert.IsNotNull(act.ChallengeList);
            Assert.Greater(act.ChallengeList.Count, 0);
        }

        [Test]
        public async Task CAMobile_GetMbwayPaymentInformationTest_should_return_string()
        {
            var challengeData = await CAMobile.PaymentService.GetMbwayPaymentInformation(_inputData);

            var act = await CAMobile.PaymentService.GetMbwayPaymentInformation(challengeData);

            Assert.IsInstanceOf<string>(act);
        }
    }
    
}
