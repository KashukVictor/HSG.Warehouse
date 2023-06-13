using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSG.Warehouse.Interfaces.Repository
{
    public interface IInvoicesRepository
    {
        Task<IEnumerable<Invoice?>?> GetAllAsync();
        Task<string?> CreateAsync(Invoice invoice);
        Task<string?> EditAsync(Invoice invoice);
        Task<string?> DeleteAsync(int invoiceId);
        Task<string?> SaveAsync(int invoiceId);
        Task<IEnumerable<InvoiceDetail?>?> GetAllProductsFromInvoiceAsync(int invoiceId);
        Task<Invoice?> GetByIdAsync(int invoiceId);
        Task<IEnumerable<ProductToInvoice?>?> GetProductToInvoices(int invoiceId, int categoryId);
        Task<int> GetSatusAsync(int invoiceId);
        Task<double> GetSumByIdAsync(int invoiceId);
        Task<string?> AddProductAsync(InvoiceDetail invoiceDetail);
        Task<string?> DeleteProductAsync(InvoiceDetail invoiceDetail);


    }
}
