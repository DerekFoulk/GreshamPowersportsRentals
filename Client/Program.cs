using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp.Client;
using BlazorApp.Shared.Contexts;
using Microsoft.EntityFrameworkCore;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazorBootstrap();

builder.Services.AddDbContext<RentalsContext>(options =>
    options.UseCosmos("https://cosmos-gprentals-dev-001.documents.azure.com:443/",
        "FESOV0K6q0DTaHKp8ihWZQhRianqZGlkzzPpMQsplcVbUZLeGwqf5V0VbXNSxvVwhdNgZ9Wd9K3IACDbagBlAg==",
        "Rentals")
);

await builder.Build().RunAsync();
