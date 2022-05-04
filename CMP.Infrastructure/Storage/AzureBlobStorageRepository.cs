using CMP.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Storage
{
    public class AzureBlobStorageRepository
    {
        private readonly ILogger _logger;
        public AzureBlobStorageRepository(
            ILogger<AzureBlobStorageRepository> logger)
        {
            _logger = logger;
        }

       
    }
}
