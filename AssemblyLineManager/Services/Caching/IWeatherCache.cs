using AssemblyLineManager.DataContracts;
using System.Collections.Immutable;

namespace AssemblyLineManager.Services.Caching
{
    public interface IWeatherCache
    {
        ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
    }
}