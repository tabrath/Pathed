using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.Services
{
    public interface ISecurityService
    {
        bool IsAdministrator();
        void ElevateToAdministrator(params string[] args);
    }
}
