namespace Tutorial12.DTO;

public class PaginatedTripsDto
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripGetDto> Trips { get; set; } = new List<TripGetDto>();
}