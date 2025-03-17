var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Listen on port 8080 for HTTP - HTTPS is handled by Traefik enterily! No https ports or redirection here!
});


var app = builder.Build();

// Enable serving default files like index.html
app.UseDefaultFiles();

// Enable serving static files from wwwroot
app.UseStaticFiles();

app.Run();