using System;
using System.Collections.Generic;
using System.Text;

namespace Wombat.Core.DependencyInjection
{
    public enum ServiceLifetime
    {
        //
        // 摘要:
        //     Specifies that a single instance of the service will be created.
        Singleton = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton,
        //
        // 摘要:
        //     Specifies that a new instance of the service will be created for each scope.
        //
        // 言论：
        //     In ASP.NET Core applications a scope is created around each server request.
        Scoped = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped,
        //
        // 摘要:
        //     Specifies that a new instance of the service will be created every time it is
        //     requested.
        Transient = Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient

    }
}
