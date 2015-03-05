using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pathed.Services;

namespace Pathed.Test
{
    [TestClass]
    public class SettingsServiceTest : ComposableTest
    {
        private ISettingsService service;

        public SettingsServiceTest()
        {
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue<IApplicationService>(new ApplicationServiceMock());
            Container.Compose(batch);
            this.service = Container.GetExportedValue<ISettingsService>();
        }

        [TestMethod]
        public void SettingsService_Get_ShouldGetDefaultValue()
        {
            var value = this.service.Get<int>("UnknownProperty", 1);
            Assert.AreEqual(value, 1);
        }

        [TestMethod]
        public void SettingsService_Set_ShouldSetProperty()
        {
            this.service.Set("UnknownProperty", "SomeValue");
            var value = this.service.Get<string>("UnknownProperty");
            Assert.AreEqual("SomeValue", value);
        }
    }
}
