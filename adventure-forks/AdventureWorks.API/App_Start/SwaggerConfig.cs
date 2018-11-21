using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using WebActivatorEx;
using AdventureWorks.API;
using AdventureWorks.API.Controllers;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace AdventureWorks.API
{
    public class SwaggerConfig
    {
        public static void Register() 
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v2", "AdventureWorks.API");
                    c.OperationFilter<FileOperationFilter>();
                })
                .EnableSwaggerUi();
        }
    }
}