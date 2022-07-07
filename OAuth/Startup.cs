using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace OAuth
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
            services.AddAuthentication(config =>
            {
                //we check the cookie to confirm that we are authenticated.
                config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                // when we sign in we will deal out a cookie.
                config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //use this to check that we are allowed to do something
                config.DefaultChallengeScheme = "OAuthServer";
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme).AddOAuth(
                "OAuthServer",
                config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "123";
                    config.AuthorizationEndpoint = "https://localhost:44348/oauth/authorize";
                    config.TokenEndpoint = "https://localhost:44348/oauth/token";
                    config.CallbackPath = "/OAuth/CallBack";
                    config.SaveTokens = true;
                });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
             
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
