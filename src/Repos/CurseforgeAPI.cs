using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mercurius.Configuration;
using NLog;

namespace Mercurius.API {
    public class CurseforgeAPI {
        private HttpClient client;
        private const string BaseUrl = @"https://api.curseforge.com";
        private ILogger logger;
        public CurseforgeAPI() {
            logger = LogManager.GetCurrentClassLogger();

            client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("x-api-key", "");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("user-agent", "Mercurius");
        }
    }
}