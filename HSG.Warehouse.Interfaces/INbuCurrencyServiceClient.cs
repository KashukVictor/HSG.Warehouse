using HSG.Warehouse.Common.Models.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces
{
    public interface INbuCurrencyServiceClient
    {
        Task<IEnumerable<Currency?>?> GetCurrencyOnDateAsync(DateTime date);
    }
}
