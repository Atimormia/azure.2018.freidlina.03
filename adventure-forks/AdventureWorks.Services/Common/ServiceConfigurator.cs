using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Services.Common
{
    public static class ServiceConfigurator
    {
        public static ILogger GetLogger()
        {
            var storage = GetStorage();
            return new LoggerConfiguration()
                .WriteTo.AzureTableStorage(storage)
                .MinimumLevel.Debug()
                .CreateLogger();
        }
        public static CloudStorageAccount GetStorage()
        {
            var connection = CloudConfigurationManager.GetSetting("StorageConnectionString");
            return CloudStorageAccount.Parse(connection);
        }
    }
}
