using Microsoft.Extensions.DependencyInjection;

namespace DFC.App.Services.Common.Providers
{
    public interface IProvideRegistrationServices
    {
        void RegisterWith(IServiceCollection containerCollection);
    }
}