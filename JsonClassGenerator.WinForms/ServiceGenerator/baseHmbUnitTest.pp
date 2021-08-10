using Common.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using $Product$.Models.Request;
using $Product$.Models.Result;
using Utilities;

namespace $Product$.Tests.HomeBanking.Unit
{
    [TestFixture]
    internal class $ClassName$Test : BaseCAMobileUnitTest
    {
        private $ClassName$Request _request = new $ClassName$Request.GetInputData();

        [Test]
        public async Task CAMobile_$ClassName$Test_should_return_ChallengeResult()
        {
            var act = await CAMobile.$Area$Service.$ClassName$(_request);

            Assert.IsInstanceOf<ChallengeData>(act);
        }

        [Test]
        public async Task CAMobile_$ClassName$Test_should_return_$ClassName$Result()
        {
            var resultWithChallenge = await CAMobile.$Area$Service.$ClassName$(_request);

            var act = await CAMobile.$Area$Service.$ClassName$Confirmation(resultWithChallenge);

            Assert.IsInstanceOf<$ClassName$Result>(act);
        }
    }
}