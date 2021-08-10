using System;
using System.Threading.Tasks;
using Common.Models;
using NUnit.Framework;
using Payments.Models.Request;
using Utilities;

namespace Payments.Tests.CAMobile.Unit
{
    [TestFixture]
    public class GetMbwayPaymentInformationTest : BaseCAMobileUnitTest
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
            var input = new ChallengeData
            {

            };

            var act = await CAMobile.PaymentService.GetMbwayPaymentInformation(input);

            Assert.IsInstanceOf<string>(act);
        }
    }
    
}
