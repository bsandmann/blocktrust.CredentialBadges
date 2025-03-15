using Blocktrust.CredentialBadges.Builder.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blocktrust.CredentialBadges.Builder.Components;
using Blocktrust.CredentialBadges.Builder.Components.Account;
using Blocktrust.CredentialBadges.Builder.Data;
using Blocktrust.CredentialBadges.Builder.Data.Entities;
using Blocktrust.CredentialBadges.Builder.Services;
using Blocktrust.CredentialBadges.Core.Services.Clipboard;
using Blocktrust.CredentialBadges.Core.Services.Images;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

//  Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.WebHost.ConfigureKestrel(options =>
{
#if !DEBUG
    options.ListenAnyIP(8080); // Listen on port 8080 for HTTP - HTTPS is handled by Traefik enterily! No https ports or redirection here!
#endif
});

builder.Services.AddScoped<ClipboardService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<ImageProcessingService>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("BuilderDbConnection") ?? throw new InvalidOperationException("Connection string 'BuilderDbConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
           .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(
        options =>
        {
            options.SignIn.RequireConfirmedAccount = true;

            // loosened password requirements
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 5;
            options.Password.RequireDigit = false;

            // default password requirements :
            // options.Password.RequireNonAlphanumeric = true;
            // options.Password.RequireLowercase = true;
            // options.Password.RequireUppercase = true;
            // options.Password.RequiredLength = 6;
            // options.Password.RequireDigit = true;
        })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, SendGridEmailSender>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, ApplicationUserEmailSender>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
builder.Services.AddHttpClient("IdentusAgents");
builder.Services.AddHttpClient("AdminAgent");
builder.Services.AddHttpClient("UserAgent");

builder.Services.AddScoped<ImageBytesToBase64>();

// Add Antiforgery services
builder.Services.AddAntiforgery();

// Configure forwarded headers when running behind a reverse proxy like Traefik
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    // Clear known networks and proxies to accept all forwarded headers
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Apply forwarded headers early in the pipeline
app.UseForwardedHeaders();

app.UseStaticFiles();

// Use Routing
app.UseRouting();

// HTTPS is handled by Traefik in production
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Add content security policy to enforce HTTPS
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", 
                               "upgrade-insecure-requests; default-src https:; img-src https: data:; connect-src https: wss:;");
    await next();
});

// Add UseAntiforgery middleware here, after authentication and authorization
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.UseStatusCodePagesWithRedirects("/");

// Ensure database is created and migrated on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Apply migrations and create database if it doesn't exist
        dbContext.Database.Migrate();
        logger.LogInformation("Database migration completed successfully.");

        // Check if we need to seed an admin user
        if (!await dbContext.AnyUsersAsync())
        {
            logger.LogInformation("No users found. Creating default admin user.");

            // Load admin data from configuration if available
            var adminEmail = builder.Configuration["AdminUser:Email"] ?? "admin@blocktrust.dev";
            var adminPassword = builder.Configuration["AdminUser:Password"] ?? "Password123!";

            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "adminRole");
                logger.LogInformation("Default admin user created successfully.");
            }
            else
            {
                logger.LogWarning("Failed to create default admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "An error occurred while migrating or seeding the database.");
    }
}

app.Run();