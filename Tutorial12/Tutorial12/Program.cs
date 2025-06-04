using Microsoft.EntityFrameworkCore;
using Tutorial12.Data;
using Tutorial12.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthorization();
app.MapControllers();

app.Run();
