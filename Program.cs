using API_Aggregation.Clients;
using Polly.Extensions.Http;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri("https://openweathermap.org/api");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

builder.Services.AddHttpClient<NewsApiClient>(client =>
{
    client.BaseAddress = new Uri("https://newsapi.org/");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

builder.Services.AddHttpClient<SocialMediaApiClient>(client =>
{
    client.BaseAddress = new Uri("https://developer.twitter.com/en/docs/twitter-api");
}).AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetFallbackPolicy());

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
