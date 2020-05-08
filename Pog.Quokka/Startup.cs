using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Pog.Quokka
{
    public class PublicKey
    {
        public string Public { get; set; }
    }
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
            services.AddControllers();
            var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(
                source: Convert.FromBase64String("MIIBCgKCAQEA0e0phULAGmcsZthKP3+pkl5jp8/eDjyWgaRWaiY5JIcQsYXfSn/eLt1qi5gbMVpiXv0Xczb2aLv7Qem/he4RPXz3ghEf8soNzTt8VDqE62IGLblrdUib2d8kbWhOLXJnkHAoPT+6Dv+wGH+CwN7aNQV1vtw7fxOMSlLQktGcNCNSZ4MMX56zRb91fsq1uDh46jJbWiG88ZJup8xZ/R14gLWM35J3RA/czyWNpN86\nhgEi6FWDgYEA2mve7fM7dhULnZGR5VWEbJFrcqJw1PmiehhxmmDvxy5AXDBVXzxJe9VA0pg3UWa+HQjXnsZC69LA2acRY99IRK97Kq+kujX0FQIDAQAB"),
                bytesRead: out int _
            );
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.Audience = "quokka";
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = "krwnx"
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}