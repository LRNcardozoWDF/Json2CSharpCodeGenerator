using System;
using System.Threading.Tasks;
using Common.Models;
using NUnit.Framework;
using $Product$.Models.Request;
using Utilities;

namespace $Product$.Tests.CAMobile.Unit
{
    [TestFixture]
    public class $ClassName$Test : BaseCAMobileUnitTest
    {
        $ClassName$Request _inputData = new $ClassName$Request().GetInputData();

        [Test]
        public async Task CAMobile_$ClassName$Test_should_return_ChallengeData()
        {
            var act = await CAMobile.$Area$Service.$ClassName$(_inputData);

            Assert.IsInstanceOf<ChallengeData>(act);
            Assert.IsInstanceOf<string>(act.TokenId);
        }


        [Test]
        public async Task CAMobile_$ClassName$Test_ChallengeData_is_filled()
        {

            var act = await CAMobile.$Area$Service.$ClassName$(_inputData);

            Assert.IsNotNull(act.TokenId);
            Assert.IsNotNull(act.ChallengeList);
            Assert.Greater(act.ChallengeList.Count, 0);
        }

        [Test]
        public async Task CAMobile_$ClassName$Test_should_return_string()
        {
            var input = new ChallengeData
            {

            };

            var act = await CAMobile.$Area$Service.$ClassName$(input);

            Assert.IsInstanceOf<string>(act);
        }
    }
    
}
