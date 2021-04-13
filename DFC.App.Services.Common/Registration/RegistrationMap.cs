using DFC.App.Services.Common.Registration.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace DFC.App.Services.Common.Registration
{
    /// <summary>
    /// an internally used registration map.
    /// </summary>
    internal sealed class RegistrationMap :
        Dictionary<TypeOfRegistrationScope, Action<IServiceCollection, ContainerRegistrationAttribute>>
    {
    }
}
