using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.CosmosDb
{
    public class CosmosDbOptions
    {
        public const string SectionName = "CosmosDb";

        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public string Account { get; set; }
        public string Key { get; set; }

    }
}
