using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupplyChainSystem.Server.Models;

namespace SupplyChainSystem.Server.Service
{
    internal class TimedHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private IServiceProvider Services;

        public TimedHostedService(IServiceProvider services)
        {
            Services = services;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(3));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = Services.CreateScope())
            {
                var _dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<ProcedurementContext>();
                var today = DateTime.Now.Date;
                var ppa = _dbContext.Agreement
                    .Where(p => p.AgreementType == AgreementType.Planned && p.StartDate <= today &&
                                p.ExpiryDate >= today)
                    .Include(p => p.PlannedPurchaseAgreementDetails);
                foreach (var pa in ppa)
                {
                    var peroid = pa.PlannedPurchaseAgreementDetails.Period;
                    var unit = pa.PlannedPurchaseAgreementDetails.TimeUnit.ToLower();
                    var date = pa.StartDate.Date;

                    while (date < today)
                    {
                        switch (unit)
                        {
                            case "day":
                                date = date.AddDays(peroid);
                                break;
                            case "month":
                                date = date.AddMonths(peroid);
                                break;
                        }
                    }

                    if (DateTime.Now.Date == date.Date)
                    {
                        if (_dbContext.ScheduleRelease.SingleOrDefault(p =>
                                p.AgreementId == pa.AgreementId && p.CreateTime.Date == today) == null)
                        {
                            _dbContext.ScheduleRelease.Add(new ScheduleRelease
                            {
                                AgreementId = pa.AgreementId,
                                CreateTime = DateTime.Now,
                                ExpectedDeliveryDate = today.AddDays(3)
                            });
                            _dbContext.SaveChanges();
                        }
                    }
                }
            }

            //new ProcedurementContext()
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}