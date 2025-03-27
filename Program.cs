// FrontEnd/Program.cs
using Auth0.AspNetCore.Authentication;
using ThAmCo.CheapestProduct.Services.CheapestProducts;
using ThAmCo.CheapestProducts.Services.CheapestProduct;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the cookie HTTP-only
    options.Cookie.IsEssential = true; // Ensure the cookie is essential
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth:Domain"];
    options.ClientId = builder.Configuration["Auth:ClientId"];
});

if (builder.Environment.IsDevelopment())
{
    // builder.Services.AddScoped<ILowestPriceService, LowestPriceServiceFake>();
    builder.Services.AddHttpClient<ILowestPriceService, LowestProducts>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["LowestProducts:Uri"]);
    });
}
else
{
    builder.Services.AddHttpClient<ILowestPriceService, LowestProducts>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["LowestProducts:Uri"]);
    });
}

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


app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();