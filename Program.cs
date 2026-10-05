using ProductManagementApp.Components;
using ProductManagementApp.Business.Interfaces;
using ProductManagementApp.Business.Services;
using ProductManagementApp.DataAccessLayer.Interfaces;
using ProductManagementApp.DataAccessLayer.Repositories;
using ProductManagementApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IProductRepository, ProductRepositoryJson>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();