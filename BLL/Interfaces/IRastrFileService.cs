using BLL.Rastrwin3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IRastrFileService
    {
        public Task<List<RastrFile>> GetRastrFiles();
        public Task PostRastrFiles();
    }
}
