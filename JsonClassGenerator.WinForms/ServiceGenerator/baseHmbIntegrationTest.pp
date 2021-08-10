using Common.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using $Product$.Models.Request;
using $Product$.Models.Result;
using Utilities;

namespace $Product$.Tests.HomeBanking.Integration
{
    [TestFixture]
    internal class $ClassName$Test : BaseAutenticatedCAHomebankingIntegrationTest
    {
        private $ClassName$Request _request = new $ClassName$Request.GetInputData();

        [Test]
        public async Task CAHomeBanking_$ClassName$Test_should_return_ChallengeResult()
        {
            var act = await CAHomeBanking.$Area$Service.$ClassName$(_request);

            Assert.IsInstanceOf<ChallengeData>(act);
        }

        [Test]
        public async Task CAHomeBanking_$ClassName$Test_should_return_$ClassName$Result()
        {
            var resultWithChallenge = await CAHomeBanking.$Area$Service.$ClassName$(_request);

            resultWithChallenge.ChallengeList = ChallengeHelper.ValidateChallenge(resultWithChallenge.ChallengeList);

            var act = await CAHomeBanking.$Area$Service.$ClassName$Confirmation(resultWithChallenge);

            Assert.IsInstanceOf<$ClassName$Result>(act);
        }
    }
}