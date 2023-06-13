using HSG.Warehouse.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSG.Warehouse.Repository.Repository
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;            
        }
        public async Task<IEnumerable<Common.Models.Entity.Warehouse?>?> GetProductsAsync(int categoryId, int currencId)
        {
            var list = await _context.Warehouse
                .Include(p => p.Product)
                .Include(c => c.Product.Category)
                .Include(m => m.Product.Manufacturer)
                .Include(u => u.Product.Unit)
                .Include(i => i.InvoiceDetail)
                .Include(inv => inv.InvoiceDetail.Invoice)
                .Include(inv => inv.InvoiceDetail.Invoice.Client)
                .Include(cur => cur.InvoiceDetail.Invoice.Currency)
                .Where(cur => cur.InvoiceDetail.Invoice.CurrencyId == currencId || currencId == 0)
                .Where(cat => cat.Product.CategoryId == categoryId || categoryId == 0)
                .OrderBy(o => o.Product.Category.Name)
                .ThenBy(o => o.Product.Manufacturer.Name)
                .ThenBy(o => o.Product.Name)
                .ToListAsync();

            return list;
        }
    }
}
