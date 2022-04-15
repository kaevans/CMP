using CMP.Core.Interfaces;
using CMP.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP.Infrastructure.Data
{
    public interface ICosmosDbRepository<T> : IRepository<T> where T : Entity
    {
        
    }
}
