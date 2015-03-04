using System;

namespace Pathed.Services
{
    public interface IApplicationService
    {
        string Title { get; }
        Version Version { get; }

        void Shutdown();
    }
}
