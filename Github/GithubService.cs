using System;
using System.Net.Http;
using System.Threading.Tasks;
using GithubLex;
using Newtonsoft.Json;

namespace GithubLex.Github
{
    public static class GithubService
    {
        public const string GITHUB_API = "https://api.github.com/users/";
        public static HttpClient client = new HttpClient();

        static GithubService()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("GithubLexbot v1.0.0");
        }

        public static async Task<GithubUser> GetUser(string userName)
        {
            var response = await client.GetAsync(GITHUB_API + userName);
            System.Console.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                var userString = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<GithubUser>(userString);
                return user;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine("ERROR:");
                System.Console.WriteLine(error);
                throw new Exception(error);
            }
        }
    }
}