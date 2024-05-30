using LiveSupport.AI.Hubs;
using LiveSupport.AI.Middleware;
using LiveSupport.AI.Models;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//use IServiceProvider for know how to initiate IDictionary 
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(IServiceProvider => new Dictionary<string, UserConnection>());
builder.Services.AddSingleton<IDictionary<string, string>>(IServiceProvider => new Dictionary<string, string>());
builder.Services.AddSingleton<IDictionary<string, List<string>>>(IServiceProvider => new Dictionary<string, List<string>>());
builder.Services.AddSignalR();
builder.Services.AddSignalR();





builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {

        builder.WithOrigins("http://localhost:4200", "http://localhost:56797")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestModifier>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoint =>
{
    endpoint.MapHub<ChatHub>("/chat");
});

/*app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());*/
app.MapControllers();

app.Run();