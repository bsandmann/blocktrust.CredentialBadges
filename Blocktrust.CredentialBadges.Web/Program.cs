using Blocktrust.CredentialBadges.Core.Commands.VerifyOpenBadge;
using Blocktrust.CredentialBadges.Core.Crypto;
using Blocktrust.CredentialBadges.Core.Services.Images;
using Blocktrust.CredentialBadges.Web;
using Blocktrust.CredentialBadges.Web.Components;
using Blocktrust.CredentialBadges.Web.Services.TemplatesService;
using Microsoft.EntityFrameworkCore;
using Blocktrust.CredentialBadges.Core.Services.DIDPrism;
using Blocktrust.CredentialBadges.Core.Commands.CheckDIDKeySignature;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CredentialBadgesDatabase")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register GenerateSnippetService
builder.Services.AddTransient<TemplatesService>();
builder.Services.AddTransient<SelectTemplateService>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(VerifyOpenBadgeHandler).Assembly));
// Register all MediatR handlers from the current domain's assemblies
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Register HttpClient
builder.Services.AddHttpClient();

// Add controllers to the services
builder.Services.AddControllers();

// Register CryptoService
builder.Services.AddScoped<IEcService, EcServiceBouncyCastle>();

// Register EcServiceBouncyCastle explicitly
builder.Services.AddScoped<EcServiceBouncyCastle>();

// Register SHA256 service
builder.Services.AddScoped<ISha256Service, Sha256ServiceBouncyCastle>();

builder.Services.AddScoped<ImageBytesToBase64>();

// Register the ExtractPrismPubKeyFromLongFormDid service
builder.Services.AddScoped<ExtractPrismPubKeyFromLongFormDid>();

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 12 * 1024 * 1024; // 12MB
    });

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