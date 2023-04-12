using AssemblyLineManager.DataContracts;
using System.Collections.Immutable;

namespace AssemblyLineManager.Services.Endpoints
{
    [Headers("Content-Type: application/json")]
    public interface IApiClient
    {
        [Get("/api/weatherforecast")]
        Task<ApiResponse<IImmutableList<WeatherForecast>>> GetWeather(CancellationToken cancellationToken = default);
    }
}