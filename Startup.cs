using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BDLocal;
using HelperFuncoes;
using JogoFuncoes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using wsJogo;
using wsModel;

namespace back
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

            services.AddDbContext<LocalContext>(options => options.UseInMemoryDatabase(databaseName: "bdlocal"));

            //Injeção de dependencias
            services.AddScoped<IJogo, Jogos>();
            services.AddScoped<IHelper, Helper>();

            //Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "testeDTI.com.br",
                    ValidAudience = "testeDTI.com.br",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("testeDTItesteDTItesteDTI"))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSignalR();

             services.AddCors(c =>  
            {  
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                                                              .AllowAnyHeader()
                                                              .AllowAnyMethod());  
            });


            //Swagger
            services.AddSwaggerGen(c=>{
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {Title="Master", Version ="V1"});  
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                 c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "As APIs utilizam uma autorização baseada em JWT, para isso é necessário adicionar no Header um Authorization: Bearer {token} - Para Utilizar as APIS desta documentação coloque no campo abaixo Bearer (token proveniente da API de Login)",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                 c.AddSecurityRequirement(security);
                
            });

            //GZIP
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddSession();
           
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(options => options.AllowAnyOrigin()
                                          .AllowAnyMethod()
                                          .AllowAnyHeader()); 

            app.UseSignalR(routes =>
            {
                routes.MapHub<JogoHub>("/jogo");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
