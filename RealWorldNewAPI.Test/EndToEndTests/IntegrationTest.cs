using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;
using RealWorldNewAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace RealWorldNewAPI.Test.EndToEndTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        public IntegrationTest()
        {
            var appFactor = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        if (dbContext != null)
                        {
                            services.Remove(dbContext);
                        }
                        var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryEmployeeTest");
                            options.UseInternalServiceProvider(serviceProvider);
                        });
                        var sp = services.BuildServiceProvider();

                        using (var scope = sp.CreateScope())
                        {
                            using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                try
                                {
                                    appContext.Database.EnsureCreated();
                                }
                                catch (Exception ex)
                                {
                                    throw;
                                }
                            }
                        }
                    });
                });
            TestClient = appFactor.CreateClient();
        }

        protected async Task Register()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await RegisterGetToken());
        }

        protected async Task Login()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await LoginGetToken());
        }

        private async Task<string> LoginGetToken()
        {
            Regex rx = new Regex(@"[a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var response = await TestClient.PostAsJsonAsync("api/users/login", new LoginUserPack
            {
                user = new LoginUser()
                {
                    Email = "test@gmail.com",
                    Password = "ZAQ!2wsx"
                }
            });

            string registerResponse = await response.Content.ReadAsStringAsync();

            MatchCollection matches = rx.Matches(registerResponse);

            return matches[0].ToString();
        }

        private async Task<string> RegisterGetToken()
        {
            Regex rx = new Regex(@"[a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+[.][a-zA-Z0-9_-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var response = await TestClient.PostAsJsonAsync("api/users", new RegisterUserPack
            {
                user = new RegisterUserDto
                {
                    email = "test@gmail.com",
                    password = "ZAQ!2wsx",
                    username = "test"
                }
            });

            var registerResponse = await response.Content.ReadAsStringAsync();

            MatchCollection matches = rx.Matches(registerResponse);

            return matches[0].ToString();
        }
        
    }
}
