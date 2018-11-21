using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Swashbuckle.Swagger;

namespace AdventureWorks.API.Controllers
{
    public class ProductionController : ApiController
    {
        private readonly CloudBlobClient _blobClient;
        private readonly CloudQueueClient _queueClient;

        public ProductionController()
        {
            var connection = CloudConfigurationManager.GetSetting("AccountConnectionString");
            var storage = CloudStorageAccount.Parse(connection);
            _blobClient = storage.CreateCloudBlobClient();
            _queueClient = storage.CreateCloudQueueClient();
        }

        // POST: api/Production
        [HttpPost]
        [ResponseType(typeof(string))]
        [SwaggerParameter("file", "A Word file to upload", Required = true, Type = "file")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = await Request.Content.ReadAsMultipartAsync();
            var bytes = await provider.Contents.First().ReadAsByteArrayAsync();

            var container = _blobClient.GetContainerReference("adventure-works-files");
            var filename = $"{DateTime.Now:s}_{Guid.NewGuid()}.docx";
            var blob = container.GetBlockBlobReference(filename);
            blob.UploadFromByteArray(bytes, 0, bytes.Length);

            await _queueClient.GetQueueReference("adventure-works-queue")
                .AddMessageAsync(new CloudQueueMessage($"File {filename} was uploaded"));

            return Ok();
        }
    }

    public class FileOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerParameterAttribute>();
            if (requestAttributes.Any())
            {
                operation.parameters = operation.parameters ?? new List<Parameter>();

                foreach (var attr in requestAttributes)
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = attr.Name,
                        description = attr.Description,
                        @in = attr.Type == "file" ? "formData" : "body",
                        required = attr.Required,
                        type = attr.Type
                    });
                }

                if (requestAttributes.Any(x => x.Type == "file"))
                {
                    operation.consumes.Add("multipart/form-data");
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerParameterAttribute : Attribute
    {
        public SwaggerParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Type { get; set; } = "text";

        public bool Required { get; set; } = false;
    }
}
