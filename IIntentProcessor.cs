using Amazon.Lambda.Core;
using Amazon.Lambda.LexEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GithubLex
{
    /// <summary>
    /// Represents an intent processor that the Lambda function will invoke to process the event.
    /// </summary>
    public interface IIntentProcessor
    {
        /// <summary>
        /// Main method for processing the Lex event for the intent.
        /// </summary>
        /// <param name="lexEvent"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<LexResponse> Process(LexEvent lexEvent, ILambdaContext context);
    }
}
