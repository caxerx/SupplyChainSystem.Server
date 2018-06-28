using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SupplyChainSystem.Server.Hub;
using SupplyChainSystem.Server.Models;
using SupplyChainSystem.Server.Service;

namespace SupplyChainSystem.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            HashUtilities.Salt =
                (string) Configuration.GetSection("ApplicationConfig").GetValue(typeof(string), "Salt");

            services.AddSignalR();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowCredentials();
                });
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddDbContext<ProcedurementContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });

            services.AddHostedService<TimedHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ProcedurementContext dbContext)
        {
            //Make sure the authentication middleware is registered before all the other middleware, including app.UseMvc()
            app.UseAuthentication();

            app.UseCors("AllowAllOrigins");


            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();


            app.UseWebSockets();
            app.UseSignalR(routes => { routes.MapHub<NotificationHub>("/api/notification"); });
            //allow corss origin for test
            app.UseMvc();

            //dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();


            //create warehouse stock if not exist
            if (dbContext.Stock.SingleOrDefault(ws => ws.StockType == StockType.WarehouseStock) == null)
            {
                dbContext.Stock.Add(new Stock
                {
                    StockType = StockType.WarehouseStock
                });
                dbContext.SaveChanges();
            }


            if (env.IsDevelopment())
            {
                //TEST USER
                var testUser = dbContext.User.SingleOrDefault(p => p.UserName == "Test");
                if (testUser == null) dbContext.User.Add(new User("Test", "TestUser", "Test", UserType.ROOT));

                dbContext.SaveChanges();
            }
        }
    }
}