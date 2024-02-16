using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OntrackDb.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace OntrackDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            JobScheduler.Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((context, config) =>
            {
                var root = config.Build();
                var isDev = context.HostingEnvironment.IsDevelopment();
                var isProd = context.HostingEnvironment.IsProduction();
                //var tenantId = "";
                //var clientId = "";
                //var clientSecret = "";
                //var url = "";
                //if (isProd)
                //{
                //    tenantId = root["ProductionSettings:tenantId"];
                //    clientId = root["ProductionSettings:ClientId"];
                //    clientSecret = root["ProductionSettings:ClientSecret"];
                //    url = root["ProductionSettings:Url"];
                //    ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                //    var secretClient = new SecretClient(new Uri(url), credential);
                //    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                //}
                //else
                //{
                //    if (isDev)
                //    {
                //        tenantId = root["DevelopmentSettings:tenantId"];
                //        clientId = root["DevelopmentSettings:ClientId"];
                //        clientSecret = root["DevelopmentSettings:ClientSecret"];
                //        url = root["DevelopmentSettings:Url"];
                //    }
               
                //    ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                //    var secretClient = new SecretClient(new Uri(url), credential);
                //    config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                //}
            }).ConfigureWebHostDefaults(webBuilder =>
                {
                    
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(opt =>
                    {
                        opt.Limits.MaxRequestBodySize = long.MaxValue;
                    });
                }).ConfigureServices((hostContext, services) =>
                {
                    // Add the required Quartz.NET services
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();

                        //q.UseMicrosoftDependencyInjectionScopedJobFactory();

                        // Create a "key" for the job
                        var jobKey = new JobKey("UserNotificationJob");

                        // Register the job with the DI container
                        q.AddJob<UserNotificationJob>(opts => opts.WithIdentity(jobKey));

                        // Create a trigger for the job
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey) // link to the HelloWorldJob
                            .WithIdentity("UserNotificationJob-trigger") // give the trigger a unique name
                            .WithCronSchedule("* 0/2 * * * ?")); // run every 5 seconds

                    }); // run every 5 seconds);

                    // Add the Quartz.NET hosted service

                    services.AddQuartzHostedService(
                        q => q.WaitForJobsToComplete = true);

                    // other config
                }).ConfigureServices((hostContext, services) =>
                {
                    // Add the required Quartz.NET services
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        //q.UseMicrosoftDependencyInjectionScopedJobFactory();

                        // Create a "key" for the job
                        var jobKey = new JobKey("AppointmentNotificationJob");

                        // Register the job with the DI container
                        q.AddJob<AppointmentNotificationJob>(opts => opts.WithIdentity(jobKey));

                        // Create a trigger for the job
                        q.AddTrigger(opts => opts
                            .ForJob(jobKey) // link to the HelloWorldJob
                            .WithIdentity("AppointmentNotificationJob-trigger") // give the trigger a unique name
                            .WithCronSchedule("0 */5 * * * ?")); // run every 5 seconds

                    }); // run every 5 seconds);

                    // Add the Quartz.NET hosted service

                    services.AddQuartzHostedService(
                        q => q.WaitForJobsToComplete = true);

                    // other config
                });
    }
}
