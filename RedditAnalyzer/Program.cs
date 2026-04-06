using RedditAnalyzer.Clients;
using RedditAnalyzer.Middleware;
using RedditAnalyzer.Service;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("Logs/out-.log", rollingInterval: RollingInterval.Day)
               .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers();


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<RedditService>();

builder.Services.AddScoped<IRedditClient, RedditJsonClient>();
builder.Services.AddScoped<RedditHtmlClient>();



builder.Services.AddHttpClient<RedditJsonClient>();
builder.Services.AddHttpClient<RedditHtmlClient>();

var app = builder.Build();


//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
//}




app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
