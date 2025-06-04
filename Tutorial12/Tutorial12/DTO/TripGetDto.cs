namespace Tutorial12.DTO;

public class TripGetDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
    public List<ClientDto> Clients { get; set; } = new List<ClientDto>();
}