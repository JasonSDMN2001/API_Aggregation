using API_Aggregation.Clients;
using API_Aggregation.Services;
using Polly.Extensions.Http;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AggregationService
builder.Services.AddTransient<AggregationService>();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri("https://wttr.in/");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

builder.Services.AddHttpClient<CatFactApiClient>(client =>
{
    client.BaseAddress = new Uri("https://meowfacts.herokuapp.com/");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

builder.Services.AddHttpClient<ArtApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.artic.edu/");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
{
    return Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent("Fallback response")
        });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();