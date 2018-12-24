using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Swagger;
using AdventureWorks.Services.Common;

namespace AdventureWorks.API.Controllers
{
    public class ProductionController : ApiController
    {
        private readonly IFileLoader _fileLoader;

        public ProductionController(IFileLoader fileLoader)
        {
            _fileLoader = fileLoader;
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

            await _fileLoader.UploadFile(bytes);

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
