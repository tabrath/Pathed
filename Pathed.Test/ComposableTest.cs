using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pathed.Services;

namespace Pathed.Test
{
    public abstract class ComposableTest : IDisposable
    {
        public AggregateCatalog Catalogs { get; protected set; }
        public CompositionContainer Container { get; protected set; }

        public ComposableTest()
        {
            Catalogs = new AggregateCatalog(new AssemblyCatalog(typeof(IApplicationService).Assembly));
            Container = new CompositionContainer(Catalogs);
        }

        public void Dispose()
        {
            Container.Dispose();
            Catalogs.Dispose();
        }
    }
}
