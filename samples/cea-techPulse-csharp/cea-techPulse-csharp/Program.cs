using cea_techPulse_csharp;
using cea_techPulse_csharp.Services;
using Microsoft.SemanticKernel;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Logging.AddConsole();

// Register Semantic Kernel
builder.Services.AddKernel();

// Register the AI service of your choice.
var config = builder.Configuration.Get<ConfigOptions>();

builder.Services.AddOpenAIChatCompletion(
   modelId: config.OpenAI.DefaultModel,
   apiKey: config.OpenAI.ApiKey
);

// Register the TechNewsMcpService
var mcpServerPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "McpServer"));
builder.Services.AddSingleton(new TechNewsMcpService(mcpServerPath, config.NewsApi?.ApiKey ?? ""));

// Add AspNet token validation
builder.Services.AddBotAspNetAuthentication(builder.Configuration);

builder.Services.AddSingleton<IStorage, MemoryStorage>();

// Add AgentApplicationOptions from config.
builder.AddAgentApplicationOptions();

// Add AgentApplicationOptions.  This will use DI'd services and IConfiguration for construction.
builder.Services.AddTransient<AgentApplicationOptions>();

// Add the bot (which is transient)
builder.AddAgent<cea_techPulse_csharp.Bot.TechNewsBot>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/messages", async (HttpRequest request, HttpResponse response, IAgentHttpAdapter adapter, IAgent agent, CancellationToken cancellationToken) =>
{
    await adapter.ProcessAsync(request, response, agent, cancellationToken);
});

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Playground")
{
    app.MapGet("/", () => "TechPulse News Agent");
    app.UseDeveloperExceptionPage();
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

app.Run();

