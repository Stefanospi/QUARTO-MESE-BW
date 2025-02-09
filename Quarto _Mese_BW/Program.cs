﻿using Microsoft.Extensions.DependencyInjection;
using Quarto__Mese_BW.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register services
builder.Services.AddTransient<IProdottoService, ProdottoService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<CarrelloService>(); // Registra CarrelloService come Scoped

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Intro}/{id?}");

app.Run();
