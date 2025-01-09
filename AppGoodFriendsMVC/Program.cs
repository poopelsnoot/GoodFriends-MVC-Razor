using Services;
using Configuration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//use multiple Secret sources, Database connections and their respective DbContexts
builder.Configuration.AddApplicationSecrets("../Configuration/Configuration.csproj");
builder.Services.AddDatabaseConnections(builder.Configuration);

#region Setup the Dependency service
builder.Services.AddSingleton<LatinService>();
builder.Services.AddSingleton<IQuoteService, QuoteService>();
builder.Services.AddScoped<IMusicService, MusicServiceWapi>();
builder.Services.AddHttpClient(name: "MusicWebApi", configureClient: options =>
{
    options.BaseAddress = new Uri(builder.Configuration["DataService:WebApiBaseUri"]);
    options.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
            mediaType: "application/json",
            quality: 1.0));
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//Map aliases
app.MapGet("/hello", () => "Hello world");
app.MapGet("/lovequotes", () => Results.Redirect("/Model/Search?search=Love"));
app.MapGet("/workquotes", () => Results.Redirect("/Model/Search?search=work"));
app.MapGet("/quotes/microsoft", () => Results.Redirect("/Model/Search?search=bill gates"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

