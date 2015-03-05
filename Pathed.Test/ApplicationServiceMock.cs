using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pathed.Services;

namespace Pathed.Test
{
    public class ApplicationServiceMock : IApplicationService
    {
        public static readonly string DefaultTitle = "Pathed";
        public static readonly Version DefaultVersion = new Version(1, 0);

        public string Title
        {
            get { return DefaultTitle; }
        }

        public Version Version
        {
            get { return DefaultVersion; }
        }

        public void Shutdown()
        {
            throw new ApplicationException("shutdown");
        }
    }
}
