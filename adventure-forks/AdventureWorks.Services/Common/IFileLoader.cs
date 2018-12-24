using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Services.Common
{
    public interface IFileLoader
    {
        Task UploadFile(byte[] bytes);
    }
}
