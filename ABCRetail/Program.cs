using ABCRetail.Services;
using Azure.Storage.Blobs;

namespace ABCRetail
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<EntityService>();
            builder.Services.AddSingleton(new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));
            builder.Services.AddScoped(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureStorage:ConnectionString"];
                var shareName = config["AzureStorage:ShareName"];
                return new FileShareService(connectionString, shareName);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
