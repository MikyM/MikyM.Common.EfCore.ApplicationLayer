using Microsoft.Extensions.Options;
using MikyM.Common.ApplicationLayer;

namespace MikyM.Common.EfCore.ApplicationLayer;

/// <summary>
/// Registration extension configuration.
/// </summary>
public sealed class EfCoreDataServicesConfiguration : DataServiceConfigurationBase, IOptions<EfCoreDataServicesConfiguration>
{
    internal EfCoreDataServicesConfiguration(ApplicationConfiguration config)
    {
        Config = config;
    }

    internal ApplicationConfiguration Config { get; set; }

    /// <inheritdoc />
    public EfCoreDataServicesConfiguration Value => this;
}
