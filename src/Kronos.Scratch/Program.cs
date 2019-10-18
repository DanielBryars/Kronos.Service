using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Kronos.Scratch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var baseUrl = "http://192.168.1.106:8123/api/";


            var httpClient = new HttpClient();



            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(baseUrl + "states/input_number.slider1"),
                Method = HttpMethod.Get,
            };

            request.Headers.Add("Authorization", $"Bearer {_token}");

            var result = await httpClient.SendAsync(request);

            var body = await result.Content.ReadAsStringAsync();

            //var json = JsonConvert.DeserializeObject<State>(body);

            //Console.WriteLine(json.state);
        }

        private static string _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJkY2E4ZDM2MmM4NWY0NmY5ODNjNDM0ZDc4NGNjNmNiMiIsImlhdCI6MTU3MTMzMjQ1NSwiZXhwIjoxODg2NjkyNDU1fQ.-XSG4YJ4dt8u7tZkT4McTL74WL6DTL73OLrd-QYz4Hc";
    }


    

}
