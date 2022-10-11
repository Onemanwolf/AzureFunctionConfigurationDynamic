using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Mvp.Function
{
    public class HttpTriggerWithOpenAPICSharp1
    {
        private readonly ILogger<HttpTriggerWithOpenAPICSharp1> _logger;
        private readonly IConfiguration _config;

        public HttpTriggerWithOpenAPICSharp1(ILogger<HttpTriggerWithOpenAPICSharp1> log, IConfiguration configuration)
        {
            _logger = log;
            _logger.LogInformation("HttpTriggerWithOpenAPICSharp1 created");
            _config = configuration;
        }


        [FunctionName("HttpTriggerWithOpenAPICSharp1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
           _logger.LogInformation("C# HTTP trigger function processed a request.");

    // Read configuration data
    string keyName = "TestApp:Settings:Message";
    string message = _config[keyName];

    return message != null
        ? (ActionResult)new OkObjectResult(message)
        : new BadRequestObjectResult($"Please create a key-value with the key '{keyName}' in App Configuration.");
        }
    }
}

