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
    public class EnvironmentServiceTest
    {
        private IEnvironmentService service = new EnvironmentService();

        [TestMethod]
        public async Task EnvironmentService_GetVariableAsync_ReturnsValidValue()
        {
            var value = await this.service.GetVariableAsync("PATH", EnvironmentVariableTarget.User);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public async Task EnvironmentService_SetVariableAsync_SetsValidValue()
        {
            var original = "the test value of a variable";
            await this.service.SetVariableAsync("PATHED_TEST_VARIABLE", original, EnvironmentVariableTarget.Process);
            var value = await this.service.GetVariableAsync("PATHED_TEST_VARIABLE", EnvironmentVariableTarget.Process);

            Assert.AreEqual(original, value);
        }

        [TestMethod]
        public async Task EnvironmentService_GetPathsAsync_ReturnsValidPaths()
        {
            var paths = await this.service.GetPathsAsync(EnvironmentVariableTarget.User);

            Assert.IsNotNull(paths);
            Assert.IsTrue(paths.Count() > 0);
        }

        [TestMethod]
        public async Task EnvironmentService_SetPathsAsync_SetsValidPaths()
        {
            var original = (new string[] { @"C:\Program Files", @"C:\Windows" });
            await this.service.SetPathsAsync(original, EnvironmentVariableTarget.Process);
            var values = (await this.service.GetPathsAsync(EnvironmentVariableTarget.Process)).ToArray();

            Assert.IsNotNull(values);
            Assert.IsTrue(values.SequenceEqual(original));
        }
    }
}
