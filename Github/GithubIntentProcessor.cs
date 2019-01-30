using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;

namespace GithubLex.Github
{
    public class GithubIntentProcessor : AbstractIntentProcessor
    {
        public const string USERNAME_SLOT = "userName";
        public GithubIntentProcessor()
        {
        }

        public async override Task<LexResponse> Process(LexEvent lexEvent, ILambdaContext context)
        {
            var slots = lexEvent.CurrentIntent.Slots;
            foreach (var slot in slots)
            {
                System.Console.WriteLine(slot.Key);
                System.Console.WriteLine(slot.Value);
            }
            var userName = slots.ContainsKey(USERNAME_SLOT) ? slots[USERNAME_SLOT] : "";
            var sessionAttributes = lexEvent.SessionAttributes ?? new Dictionary<string, string>();
            var validateResult = Validate(userName);

            if (!validateResult.IsValid)
            {
                slots[validateResult.ViolationSlot] = null;
                return ElicitSlot(sessionAttributes, lexEvent.CurrentIntent.Name, slots, validateResult.ViolationSlot, validateResult.Message);
            }
            try
            {
                var user = await GithubService.GetUser(userName);
                return Close(sessionAttributes, "Fulfilled",
                    new LexResponse.LexMessage()
                    {
                        ContentType = "PlainText",
                        Content = $"Located a Github user named {user.Name}. This user has {user.PublicRepos} repos, {user.Followers} followers, and {user.PublicGists} Gists."
                    });
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return ElicitSlot(sessionAttributes, lexEvent.CurrentIntent.Name, slots, validateResult.ViolationSlot,
                    new LexResponse.LexMessage()
                    {
                        ContentType = "PlainText",
                        Content = "I had trouble getting information on that user. Please try another Github user."
                    });
            }
        }

        private ValidationResult Validate(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new ValidationResult(false, USERNAME_SLOT,
                        "I don't think you entered a Github username. Can you try again?");
            }
            return ValidationResult.VALID_RESULT;
        }
    }
}