using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.Services
{
    public interface IApplicationService
    {
        string GetTitle();
        Version GetVersion();
        void Shutdown();
    }
}
