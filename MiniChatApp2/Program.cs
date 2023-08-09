using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MiniChatApp2.Data;
using MiniChatApp2.Interfaces;
using MiniChatApp2.Middlewares;
using MiniChatApp2.Model;
using MiniChatApp2.Repositories;
using MiniChatApp2.Services;
using System.Text;

namespace MiniChatApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<RealAppContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("RealAppContext") ));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddGoogle(options =>
            {
                 options.ClientId = "741527376978-3ednvlp0982shao300v82o9umag8re9n.apps.googleusercontent.com";
                 options.ClientSecret = "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";
             });



            builder.Services.AddScoped<UserManager<IdentityUser>>();
            builder.Services.AddScoped<SignInManager<IdentityUser>>();
            // builder.Services.AddScoped<Middleware>();

            builder.Services.AddScoped<IUserService, UserService>();
             builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            // Add services to the container.

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
             .AddEntityFrameworkStores<RealAppContext>()
             .AddDefaultTokenProviders()
             .AddUserManager<UserManager<IdentityUser>>(); ;

            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
             
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            app.UseCors(builder => builder.AllowAnyOrigin().
            AllowAnyMethod().
            AllowAnyHeader());

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<RealAppContext>();
                context.Database.EnsureCreated();
                // DbInitializer.Initialize(context);
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

           // app.UseMiddleware<Middleware>();
            
            app.MapControllers();

            app.Run();
        }
    }
}