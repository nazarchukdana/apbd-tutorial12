using Microsoft.EntityFrameworkCore;
using Tutorial12.Data;
using Tutorial12.DTO;
using Tutorial12.Exceptions;
using Tutorial12.Models;

namespace Tutorial12.Services;

public class DbService : IDbService
{
    private readonly MasterContext _context;

    public DbService(MasterContext context)
    {
        _context = context;
    }

    public async Task<PaginatedTripsDto> GetTripsAsync(int page, int pageSize)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        var totalTrips = await _context.Trips.CountAsync();
        var totalrages = (int)Math.Ceiling((double)totalTrips / pageSize);

        var trips = await _context.Trips
            .Include(t => t.IdCountries)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TripGetDto
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.IdCountries.Select(c => new CountryDto
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(c => new ClientDto
                {
                    FirstName = c.IdClientNavigation.FirstName,
                    LastName = c.IdClientNavigation.LastName
                }).ToList()
            }).ToListAsync();
        return new PaginatedTripsDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = totalrages,
            Trips = trips
        };
        
    }

    public async Task DeleteClientAsync(int clientId)
    {
        var client = await _context.Clients
            .Include(c=> c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == clientId);
        if (client == null)
        {
            return;
        }

        if (client.ClientTrips.Any())
        {
            throw new BadRequestException("Client has assigned trips and cannot be deleted.");
        }
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
    }

    public async Task AssignClientToTripAsync(int idTrip, AssignClientToTripDto dto)
    {
        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        if (existingClient != null)
            throw new BadRequestException("Client with this pesel already exists.");
        
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
            throw new NotFoundException("Trip does not exist.");
        
        if(trip.DateFrom <= DateTime.Now)
            throw new BadRequestException("Trip already started.");
        
        var assignedCount = await _context.ClientTrips.CountAsync(ct => ct.IdTrip == idTrip);
        if (assignedCount >= trip.MaxPeople)
            throw new BadRequestException("Trip is fully booked.");
        
        var isClientRegistered = await _context.ClientTrips
            .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClientNavigation.Pesel == dto.Pesel);
        if (isClientRegistered)
            throw new BadRequestException("Client is already registered for this trip.");
        
        var newClient = new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };
        await _context.Clients.AddAsync(newClient);
        await _context.SaveChangesAsync();
        var clientTrip = new ClientTrip
        {
            IdClient = newClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate,
        };
        await _context.ClientTrips.AddAsync(clientTrip);
        await _context.SaveChangesAsync();
    }
}