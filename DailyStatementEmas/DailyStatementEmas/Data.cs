using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using static DailyStatementEmas.Model;

namespace DailyStatementEmas
{
    public class Data : DbContext
    {
        private readonly IConfiguration _iConfiguration;
        private readonly string _connectionString;
        public Data()
        {
            var uri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            IConfigurationBuilder builder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(path))
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iConfiguration = builder.Build();
            _connectionString = _iConfiguration.GetConnectionString("myconn");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlServer(_connectionString, providerOptions => providerOptions.CommandTimeout(60000))
                          .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
        public DbSet<app_VendorOutlet> app_VendorOutlet { get; set; }
    }
}
