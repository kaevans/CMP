using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Core.Models
{

    public class GitRepo : Entity
    {
        public string Name { get; set; }
        
        public string Url { get; set; }
        public string RemoteUrl { get; set; }
        public string WebUrl { get; set; }
        
    }
}
