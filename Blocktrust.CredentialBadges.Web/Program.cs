using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CredentialBadgesDatabase")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VerifyOpenBadgeHandler).Assembly));

// Register all MediatR handlers from the current domain's assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();