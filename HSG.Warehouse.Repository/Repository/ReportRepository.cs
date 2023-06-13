using HSG.Warehouse.Common.Models.Report;
using HSG.Warehouse.Interfaces.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;
        private readonly string? _connectionString;

        public ReportRepository(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetSection("ConnectionStrings")["DefaultConnection"];
        }
        public async Task<IEnumerable<SaleStats?>?> GetSaleStat()
        {
            var stats = new List<SaleStats?>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(@"SELECT	
                                                    YEAR(S.Date) as year,
                                                    MONTH(S.Date) as month,
                                                    SUM(Amount) SaleCount,
                                                    SUM(Amount * Price) SaleSum,
                                                    C.Id as CurrencyId,
                                                    C.ShortName as CurrencyName
                                                FROM
                                                    Sales S
                                                    INNER JOIN SaleDetails SD ON SD.SaleId = S.Id
                                                    INNER JOIN Currency C ON C.Id = S.CurrencyId
                                                GROUP BY
                                                    YEAR(S.Date),
                                                    MONTH(S.Date),
                                                    C.Id,
                                                    C.ShortName", connection);

                

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        stats.Add(new SaleStats
                        {
                            CurrencyId = Convert.ToInt32(reader["CurrencyId"]),
                            CurrencyShortName = reader["CurrencyName"].ToString(),
                            Month = Convert.ToByte(reader["month"]),
                            SaleCount = Convert.ToInt32(reader["SaleCount"]),
                            SaleSum = Convert.ToDouble(reader["SaleSum"]),
                            Year = Convert.ToInt32(reader["Year"])
                        });

                    }
                }

            }

            return stats;
        }
    }
}
