using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Hack.Service.RedisDb.Controllers {
    [ApiController]
    [Route("[controller]/[action]")]

    public class HomeController {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public HomeController(IHttpContextAccessor httpContextAccessor, IConfiguration config) {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }
        
        [HttpGet("index")]
        public object Index()
        {
            return new
            {
                HostName = Dns.GetHostName(),
                Id = _httpContextAccessor.HttpContext.Connection.Id,
                RedisConnection = _config["RedisConnection"]
            };
        }
        
        [HttpGet("secrets")]
        public object Secrets() {
            string mountPath = "/run/secrets/kubernetes.io/serviceaccount/";
            return new
            {
                Namespace = File.ReadAllText(@$"{mountPath}namespace"),
            };
        }
        
        [HttpGet("pods")]
        public async Task<string> Pods() {
            string mountPath = "/run/secrets/kubernetes.io/serviceaccount/";
            var nm = File.ReadAllText(@$"{mountPath}namespace");
            var cert = new X509Certificate2(File.ReadAllBytes($"{mountPath}ca.crt"));
            var token = File.ReadAllText(@$"{mountPath}token");

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (x, y, z, t) => true;
            handler.ClientCertificates.Add(cert);
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var res = await client.GetAsync("https://192.168.49.2:8443/api/v1/pods");
            return await res.Content.ReadAsStringAsync();
        }
        
        [HttpGet("PodsForCurrentNamespace")]
        public async Task<string> PodsForCurrentNamespace() {
            string mountPath = "/run/secrets/kubernetes.io/serviceaccount/";
            var nm = File.ReadAllText(@$"{mountPath}namespace");
            var cert = new X509Certificate2(File.ReadAllBytes($"{mountPath}ca.crt"));
            var token = File.ReadAllText(@$"{mountPath}token");

            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (x, y, z, t) => true;
            handler.ClientCertificates.Add(cert);
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var res = await client.GetAsync($"https://192.168.49.2:8443/api/v1/namespaces/{nm.Trim()}/pods");
            return await res.Content.ReadAsStringAsync();
        }
    }
}