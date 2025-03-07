using FishyFlip;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<FishyFlip.ATProtocol>(x => new ATProtocolBuilder().Build());

builder.Services.AddAntiforgery();

builder.Services.AddAuthentication(defaultScheme: "cookies")
    .AddCookie("cookies", o =>
    {
        o.ExpireTimeSpan = TimeSpan.FromDays(1);
        o.SlidingExpiration = true;

        o.LoginPath = "/login";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAntiforgery();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
