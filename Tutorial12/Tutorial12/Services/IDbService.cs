using Tutorial12.Data;
using Tutorial12.DTO;

namespace Tutorial12.Services;

public interface IDbService
{
    Task<PaginatedTripsDto> GetTripsAsync(int page, int pageSize);
    Task DeleteClientAsync(int clientId);
    Task AssignClientToTripAsync(int idTrip, AssignClientToTripDto dto);
    
}