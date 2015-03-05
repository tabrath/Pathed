using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pathed.Services;

namespace Pathed.Test
{
    [TestClass]
    public class SecurityServiceTest
    {
        private ISecurityService service = new SecurityService();

        [TestMethod]
        public void SecurityService_IsAdministrator_IsFalseWhenNotElevated()
        {
            Assert.IsFalse(this.service.IsAdministrator());
        }

        [TestMethod]
        public void SecurityService_IsAdministrator_IsTrueWhenElevated()
        {
            this.service.ElevateToAdministrator();

            Assert.IsTrue(this.service.IsAdministrator());
        }

        private class SecurityService : ISecurityService
        {
            private bool isAdministrator = false;

            public bool IsAdministrator()
            {
                return this.isAdministrator;
            }

            public void ElevateToAdministrator(params string[] args)
            {
                this.isAdministrator = true;
            }
        }
    }
}
