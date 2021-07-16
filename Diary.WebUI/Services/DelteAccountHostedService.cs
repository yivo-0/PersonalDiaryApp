using DataAccessLibrary.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diary.WebUI.Services
{
    public class DelteAccountHostedService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        public DelteAccountHostedService(IServiceProvider provider)
        {
            _provider = provider;
            Console.WriteLine("HostedService created");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _provider.CreateScope())
                {
                    var scopedProvider = scope.ServiceProvider;

                    var context = scope.ServiceProvider
                        .GetRequiredService<ApplicationContext>();

                    var user = context.Users.Where(x => !string.IsNullOrEmpty(x.DeleteDate)).FirstOrDefault();
                    if (user != null)
                    {
                        context.Users.Remove(user);
                        await context.SaveChangesAsync();
                    }
                }
                await Task.Delay(TimeSpan.FromDays(2), stoppingToken);
            }
        }
    }
}


