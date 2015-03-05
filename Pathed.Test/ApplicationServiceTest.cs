using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pathed.Services;

namespace Pathed.Test
{
    [TestClass]
    public class ApplicationServiceTest
    {
        private IApplicationService service = new ApplicationServiceMock();
        
        [TestMethod]
        public void ApplicationService_Title_IsCorrect()
        {
            Assert.AreEqual(ApplicationServiceMock.DefaultTitle, this.service.Title);
        }

        [TestMethod]
        public void ApplicationService_Version_IsCorrect()
        {
            Assert.AreEqual(ApplicationServiceMock.DefaultVersion, this.service.Version);
        }

        [TestMethod, ExpectedException(typeof(ApplicationException))]
        public void ApplicationService_Shutdown_ThrowsException()
        {
            this.service.Shutdown();
        }
    }
}
