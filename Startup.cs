using Azure.Storage.Blobs;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using OntrackDb.Assets;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.CustomTokenProviders;
using OntrackDb.Entities;
using OntrackDb.Helper;
using OntrackDb.Hub;
using OntrackDb.Model;
using OntrackDb.Repositories;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OntrackDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //    readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.  
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = 209715200; // 200 MB in bytes
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 209715200; // Set the maximum request body size
            });


            services.AddControllers()
           .AddXmlSerializerFormatters();
            services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConnStr")));

            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddScoped<ChatHub>();
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IOntrackParser, PioneerrxParser>();

            services.AddScoped<IUserData, UserData>();
            services.AddScoped<IPharmacyData, PharmacyData>();
            services.AddScoped<IPatientData, PatientData>();
            services.AddScoped<IWebhookData, WebhookData>();
            services.AddScoped<INoteData, NoteData>();
            services.AddScoped<IMedicationData, MedicationData>();
            services.AddScoped<IDoctorData, DoctorData>();
            services.AddScoped<IMedicationConsumptionData, MedicationConsumptionData>();
            services.AddScoped<IAppointmentData, AppointmentData>();
            services.AddScoped<IMessageData, MessageData>();
            services.AddScoped<INotificationData, NotificationData>();
            services.AddScoped<ICmrMedicationData, CmrMedicationData>();
            services.AddScoped<IAllergyData, AllergyData>();
            services.AddScoped<ISideEffectData, SideEffectData>();
            services.AddScoped<IReactionData, ReactionData>();
            services.AddScoped<IMedicationSubstanceData, MedicationSubstanceData>();
            services.AddScoped<IMedicationToDoListData, MedicationToDoListData>();
            services.AddScoped<IServiceTakeawayInformationData, ServiceTakeawayInformationData>();
            services.AddScoped<ISafetyDisposalData, SafetyDisposalData>();
            services.AddScoped<IOtcMedicationData, OtcMedicationData>();
            services.AddTransient<IPatientPdcData, PatientPdcData>();
            services.AddScoped<IReconciliationAllergyData, ReconciliationAllergyData>();
            services.AddScoped<IReconciliationSideEffectData, ReconciliationSideEffectData>();
            services.AddScoped<IReconciliationData, ReconciliationData>();
            services.AddScoped<IReconciliationToDoListData, ReconciliationToDoListData>();
            services.AddScoped<IServiceTakeAwayMedReconciliationData, ServiceTakeAwayMedReconciliationData>();
            services.AddScoped<IDoctorMedicationData, DoctorMedicationData>();
            services.AddScoped<IPatientMailListData, PatientMailListData>();
            services.AddScoped<IImportLogData, ImportLogData>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPharmacyService, PharmacyService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IWebHookService, WebHookService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMedicationService, MedicationService>();
            services.AddScoped<IMedicationConsumptionService, MedicationConsumptionService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IUtilityFax, UtilityFax>();
            services.AddScoped<INdcApiService, NdcApiService>();
            services.AddScoped<ICmrMedicationService, CmrMedicationService>();
            services.AddScoped<IAllergyService, AllergyService>();
            services.AddScoped<ISideEffectService, SideEffectService>();
            services.AddScoped<IMedicationToDoListService, MedicationToDoListService>();
            services.AddScoped<IServiceTakeawayInformationService, ServiceTakeawayInformationService>();
            services.AddScoped<ISafetyDisposalService, SafetyDisposalService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IPdfStorageService, PdfStorageService>();
            services.AddScoped<IOtcMedicationService, OtcMedicationService>();
            services.AddScoped<IPatientMailListService, PatientMailListService>();
            services.AddTransient<IPatientPdcService, PatientPdcService>();

            services.AddScoped<IReconciliationAllergyService, ReconciliationAllergyService>();
            services.AddScoped<IReconciliationSideEffectService, ReconciliationSideEffectService>();
            services.AddScoped<IReconciliationService, ReconciliationService>();
            services.AddScoped<IReconciliationToDoListService, ReconciliationToDoListService>();
            services.AddScoped<IServiceTakeAwayMedReconciliationService, ServiceTakeAwayMedReconciliationService>();
            services.AddScoped<IImportLogService, ImportLogService>();
            services.AddScoped<IPrimaryThirdPartyService, PrimaryThirdPartyService>();
            services.AddScoped<IPrimaryThirdPartyData, PrimaryThirdPartyData>();

            services.AddTransient<IAuditLogData, AuditLogData>();
            services.AddTransient<IAuditLogService, AuditLogService>();

            services.AddTransient<IImportWizardService, ImportWizardService>();
            services.AddTransient<IImportWizardData, ImportWizardData>();

            services.AddTransient<IPatientCallInfoService, PatientCallInfoService>();
            services.AddTransient<IPatientCallInfoData, PatientCallInfoData>();

            services.AddHttpContextAccessor();

            services.AddHttpClient("NdcServices", c => c.BaseAddress = new Uri(Configuration["Rx-nav-Url"]));
            services.AddHttpClient("SafetyDisposalService", c => c.BaseAddress = new Uri(Configuration["SafetyDisposalNearByAddressUrl"]));
            services.AddHttpClient("RxInfoServices", c => c.BaseAddress = new Uri(Configuration["RxAllInfoApiUri"]));
            services.AddHttpClient("ZipTastic", c => c.BaseAddress = new Uri(Configuration["ZipTasticUrl"]));

            //services.AddSignalR()
            //    .AddJsonProtocol(options => {
            //        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            //    });


            services.AddSignalR();
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(
            //        builder =>
            //        {
            //            builder.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //                 .AllowAnyMethod()
            //               .AllowCredentials()
            //               .SetIsOriginAllowed((host) => true);
            //        });
            //});

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(Configuration["ApiBaseUrl"], Configuration["DevelopmentBaseUrl"], Configuration["EndpointUrl"])
                        .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowed((host) => true);
                    });
            });
            services.AddScoped(_ =>
            {
                return new BlobServiceClient(Configuration.GetConnectionString("AzureBlobStorage"));
            });
            services.AddScoped<IImageService, ImageService>();
            //services.AddScoped(_ => {
            //    return new BlobClient(Configuration.GetConnectionString("AzureBlobStorage"), "ontrackrxstorageforimage","images");
            //});
           
            // For Identity  

            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders().AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation"); ;

            services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["JWT:ValidAudience"],
                    ValidIssuer = Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var path = context.HttpContext.Request.Path;

                        if (path.StartsWithSegments("/chatHub"))
                        {
                            // for SignalR - extract the access token from query string and assign it to the context
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrWhiteSpace(accessToken)) context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        // Skip the default logic.
                        context.HandleResponse();

                        var payload = new JObject
                        {
                            ["error"] = context.Error,
                            ["error_description"] = context.ErrorDescription,
                            ["error_uri"] = context.ErrorUri,
                            ["status"] = 401
                        };

                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = 401;

                        return context.Response.WriteAsync(payload.ToString());
                    }
                };
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;  //104857600;
            });

            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 100 MB
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.MultipartHeadersCountLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;

               // options.MultipartBodyLengthLimit = 104857600;
            });

            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 6 Web API",
                    Description = "Authentication and Authorization in ASP.NET 6 with JWT and Swagger"
                });
                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.  
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext applicationDbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            try
            {
                applicationDbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
            }

            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null; // unlimited I guess
                await next.Invoke();
            });
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET 6 Web API v1");
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
            }             
            app.UseDefaultFiles();
            //  app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();


            // global cors policy
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatHub");

                endpoints.MapControllerRoute(
                    name: "twilio-sms",
                    pattern: "twilio-sms/{Action}",
                    defaults: new { Controller = "TwilioSms" });
            });


            //app.UseAzureSignalR(routes =>
            //{
            //    routes.MapHub<ChatHub>("/chatHub");
            //});

            EmailTemplates.Initialize(env);
        }
    }
}
