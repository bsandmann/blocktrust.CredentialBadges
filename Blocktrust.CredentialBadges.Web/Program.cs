using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Web.Components;
using Blocktrust.CredentialBadges.Web.Services.GenerateSnippetService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CredentialBadgesDatabase")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register GenerateSnippetService
builder.Services.AddTransient<GenerateSnippetService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VerifyOpenBadgeHandler).Assembly));
// Register all MediatR handlers from the current domain's assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Add controllers to the services
builder.Services.AddControllers();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Enable CORS
app.UseCors("AllowAllOrigins");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map controller routes
app.MapControllers();

app.Run();