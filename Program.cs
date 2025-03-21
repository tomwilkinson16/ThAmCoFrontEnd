// FrontEnd/Program.cs
using Auth0.AspNetCore.Authentication;
using ThAmCo.CheapestProduct.Services.CheapestProducts;
using ThAmCo.CheapestProducts.Services.CheapestProduct;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth:Domain"];
    options.ClientId = builder.Configuration["Auth:ClientId"];
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ILowestPriceService, LowestPriceServiceFake>();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();