using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSG.Warehouse.Web.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoicesRepository _invoicesRepository;
        private readonly IOptionsRepository _optionsRepository;

        public InvoiceController(IInvoicesRepository invoicesRepository, IOptionsRepository optionsRepository)
        {
            _invoicesRepository = invoicesRepository;
            _optionsRepository = optionsRepository;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new InvoicesListViewModel();
            vm.Invoices = await _invoicesRepository.GetAllAsync();

            return View(vm);
        }
        public async Task<IActionResult> Create()
        {
            var vm = new InvoiceViewModel();
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Invoice invoice)
        {            
            var error = await _invoicesRepository.CreateAsync(invoice);

            if (invoice.Id > 0)
            {
                return LocalRedirect("~/Invoice/AddProduct?CategoryId=0&InvoiceId=" + @invoice.Id);
            }

            var vm = new InvoiceViewModel();
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            TempData["Error"] = error;
            return View(vm);
        }
        public async Task<IActionResult> Edit(int invoiceId)
        {
            //Роблю перевірку чи є такий інвойс і чи він не "зафіксований"
            var invoiceStatus = await _invoicesRepository.GetSatusAsync(invoiceId);

            switch (invoiceStatus)
            {
                case 1:
                    TempData["Error"] = "Накладна зафіксована. Редагування заборонено!";
                    return RedirectToAction("Index");

                case 2:
                    TempData["Error"] = "Нема даних!";
                    return RedirectToAction("Index");
            }

            var vm = new InvoiceViewModel();
            vm.Invoice = await _invoicesRepository.GetByIdAsync(invoiceId);
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Invoice invoice)
        {
            //Роблю перевірку чи є такий інвойс і чи він не "зафіксований"

            if (invoice.Id > 0)
            {
                var invoiceStatus = await _invoicesRepository.GetSatusAsync(invoice.Id);

                switch (invoiceStatus)
                {
                    case 0:
                        var result = await _invoicesRepository.EditAsync(invoice);
                        if (result != null)
                        {
                            TempData["Error"] = result;
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                        break;
                    case 1:
                        TempData["Error"] = "Накладна зафіксована. Редагування заборонено!";
                        break;
                    case 2:
                        TempData["Error"] = "Нема даних!";
                        break;
                }
            }
            var vm = new InvoiceViewModel();
            vm.Invoice = invoice;
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        public async Task<IActionResult> AddProduct(int categoryId, int invoiceId)
        {
            //Роблю перевірку чи є такий інвойс і чи він не "зафіксований"
            var invoiceStatus = await _invoicesRepository.GetSatusAsync(invoiceId);

            if (invoiceStatus != 0)
            {
                TempData["Error"] = "Нема даних!";
                return RedirectToAction("Index");
            }

            var categories = await _optionsRepository.GetAllCategoriesAsync();

            var vm = new ProductToInvoiceViewModel();
            vm.Categories = categories;
            vm.ProductToInvoices = await _invoicesRepository.GetProductToInvoices(categoryId, invoiceId);
            vm.Invoice = await _invoicesRepository.GetByIdAsync(invoiceId);
            vm.Sum = await _invoicesRepository.GetSumByIdAsync(invoiceId);

            vm.CategoryMenu = new CategoryMenu
            {
                SelectedId = categoryId,
                Categories = categories,
            };

            var path = HttpContext.Request.QueryString.ToString();
            vm.CategoryMenu.QueryString = path.Remove(0, path.IndexOf("&"));

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(InvoiceDetail invoiceDetail)
        {
            var error = await _invoicesRepository.AddProductAsync(invoiceDetail);
            var sum = await _invoicesRepository.GetSumByIdAsync(invoiceDetail.InvoiceId);

            return error == null ? Ok(sum) : NotFound();
        }
        public async Task<IActionResult> View(int invoiceId)
        {
            var vm = new InvoiceViewViewModel();
            vm.Invoice = await _invoicesRepository.GetByIdAsync(invoiceId);
            vm.InvoiceDetail = await _invoicesRepository.GetAllProductsFromInvoiceAsync(invoiceId);
            vm.Sum = await _invoicesRepository.GetSumByIdAsync(invoiceId);
            return View(vm);
        }
        public async Task<IActionResult> Delete(int invoiceId)
        {
            TempData["Error"] = await _invoicesRepository.DeleteAsync(invoiceId);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteProduct(InvoiceDetail invoiceDetail)
        {
            var result = await _invoicesRepository.DeleteProductAsync(invoiceDetail);
            var sum = await _invoicesRepository.GetSumByIdAsync(invoiceDetail.InvoiceId);

            return result == null ? Ok(sum) : NotFound();
        }
        public async Task<IActionResult> Save(Invoice invoice)
        {
            var result = await _invoicesRepository.SaveAsync(invoice.Id);

            if (result == null)
            {
                return RedirectToAction("Index");
            }

            TempData["Error"] = result;

            return RedirectToAction("View", new { id = invoice.Id });
        }
    }
}
