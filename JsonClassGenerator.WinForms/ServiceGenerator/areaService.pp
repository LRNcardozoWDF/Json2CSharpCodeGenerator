public async Task<ChallengeData> $ClassName$($ClassName$Request request)
        {
            var input = $Area$ServiceRequestMapping.$ClassName$(request);
            var output = (ChallengesOutput)await serviceManager.InvokeChallengeService<$ClassName$Service>(input);

            return CommonResultMapping.GetChallengeData(output);
        }

        public async Task<string> $ClassName$(ChallengeData challengeData)
        {
            var output = ($ClassName$Output)await serviceManager.InvokeWithChallenge<$ClassName$Service>(challengeData);

            return output.$Area$Reference;
        }

        //$Replace$