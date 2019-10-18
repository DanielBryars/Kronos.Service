using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kronos.Service
{
    public class HomeAssistantControl : IHomeAssistantControl
    {
        private static string _token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJkY2E4ZDM2MmM4NWY0NmY5ODNjNDM0ZDc4NGNjNmNiMiIsImlhdCI6MTU3MTMzMjQ1NSwiZXhwIjoxODg2NjkyNDU1fQ.-XSG4YJ4dt8u7tZkT4McTL74WL6DTL73OLrd-QYz4Hc";
        private ILogger<HomeAssistantControl> _logger;
        private readonly string _baseUrl = "http://192.168.1.106:8123/api/";

        public HomeAssistantControl(ILogger<HomeAssistantControl> logger)
        {
            _logger = logger;
        }

        public async Task SwitchHotwaterAsync(bool state)
        {
            await SwitchAsync("switch.hot_water", state);
        }

        public async Task SwitchHeatingAsync(bool state)
        {
            await SwitchAsync("switch.heating_demand", state);
            await SwitchAsync("switch.ws_manifold_0_pin_04", state);
            await SwitchAsync("switch.ws_manifold_0_pin_11", state);
        }

        private async Task SwitchAsync(string entity, bool state)
        {
            _logger.LogInformation($"{entity} -> {state}");
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + $"services/switch/{(state ? "turn_on" : "turn_off")}"),
                Method = HttpMethod.Post,
                Content = new StringContent($"{{\"entity_id\":\"{entity}\"}}", Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Authorization", $"Bearer {_token}");

            var result = await httpClient.SendAsync(request);

            var body = await result.Content.ReadAsStringAsync();
            _logger.LogInformation(body);
        }

        public double GetCurrentHouseTemperature()
        {
            return 16.4;
        }

        public double GetSetTemperature()
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(_baseUrl + "states/input_number.slider1"),
                Method = HttpMethod.Get,
            };

            request.Headers.Add("Authorization", $"Bearer {_token}");

            var result = httpClient.SendAsync(request).Result;

            var body = result.Content.ReadAsStringAsync().Result;

            var json = JsonConvert.DeserializeObject<State>(body);

            return double.Parse(json.state);
        }        
    }

    public class State
    {
        public Attributes attributes { get; set; }
        public Context context { get; set; }
        public string entity_id { get; set; }
        public DateTime last_changed { get; set; }
        public DateTime last_updated { get; set; }
        public string state { get; set; }
    }

    public class Attributes
    {
        public string friendly_name { get; set; }
        public float initial { get; set; }
        public float max { get; set; }
        public float min { get; set; }
        public string mode { get; set; }
        public float step { get; set; }
    }

    public class Context
    {
        public string id { get; set; }
        public object parent_id { get; set; }
        public string user_id { get; set; }
    }
}


