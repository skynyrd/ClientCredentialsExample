using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace TheClient
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        
        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000"); //IdentityServer Address
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "TheClientId", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("TheApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine("Token Response: \n");
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n**************************");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/api/values");
            Console.WriteLine("Status code from the API:\n");
            Console.WriteLine(response.StatusCode);
            Console.WriteLine("\n**************************");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Response body: ");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
