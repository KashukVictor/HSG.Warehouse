using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSG.Warehouse.Web.Controllers
{
    [Authorize]
    public class SaleController : Controller
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IOptionsRepository _optionsRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public SaleController(ISalesRepository salesRepository, IOptionsRepository optionsRepository, IWarehouseRepository warehouseRepository)
        {
            _salesRepository = salesRepository;
            _optionsRepository = optionsRepository;
            _warehouseRepository = warehouseRepository;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new SalesListViewModel();
            vm.Sales = await _salesRepository.GetAllAsync();

            return View(vm);
        }
        public async Task<IActionResult> Create()
        {
            var vm = new SaleViewModel();
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Sale sale)
        {
            var error = await _salesRepository.CreateAsync(sale);

            if (sale.Id > 0)
            {
                return LocalRedirect("~/Sale/AddProduct?CategoryId=0&SaleId=" + sale.Id);
            }

            var vm = new SaleViewModel();
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            TempData["Error"] = error;
            return View(vm);
        }
        public async Task<IActionResult> AddProduct(int categoryId, int saleId)
        {

            //Роблю перевірку чи є така накладна і чи вона не "зафіксована"
            var saleStatus = await _salesRepository.GetSatusAsync(saleId);

            if (saleStatus != 0)
            {
                TempData["Error"] = "Нема даних!";
                return RedirectToAction("Index");
            }

            var vm = new WarehouseViewModel();
            vm.Warehouse = await _warehouseRepository.GetProductsAsync(categoryId, 0);
            var categories = await _optionsRepository.GetAllCategoriesAsync();
            vm.ProductToSales = await _salesRepository.GetProductsForSaleAsync(categoryId, saleId);
            vm.Sale = await _salesRepository.GetByIdAsync(saleId);
            vm.Sum = await _salesRepository.GetSumByIdAsync(saleId);
            vm.Categories = categories;
            vm.CategoryMenu = new CategoryMenu
            {
                SelectedId = categoryId,
                Categories = categories
            };

            var path = HttpContext.Request.QueryString.ToString();
            vm.CategoryMenu.QueryString = path.Remove(0, path.IndexOf("&"));

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(SaleDetail saleDetail)
        {
            var error = await _salesRepository.AddProductAsync(saleDetail);
            var sum = await _salesRepository.GetSumByIdAsync(saleDetail.SaleId);

            return error == null ? Ok(sum) : NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int saleDetailId, int saleId)
        {
            var error = await _salesRepository.DeleteProductAsync(saleDetailId);
            var sum = await _salesRepository.GetSumByIdAsync(saleId);

            return error == null ? Ok(sum) : NotFound();
        }
        public async Task<IActionResult> Delete(int saleId)
        {
            TempData["Error"] = await _salesRepository.DeleteAsync(saleId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Save(Sale sale)
        {
            var result = await _salesRepository.SaveAsync(sale.Id);

            if (result == null)
            {
                return RedirectToAction("Index");
            }

            TempData["Error"] = result;

            return LocalRedirect($"~/Sale/AddProduct?CategoryId=0&SaleId={sale.Id}");
        }
        public async Task<IActionResult> View(int saleId)
        {
            var vm = new SaleViewViewModel();
            vm.Sale = await _salesRepository.GetByIdAsync(saleId);
            vm.SaleDetail = await _salesRepository.GetAllProductsFromSaleAsync(saleId);
            vm.Sum = await _salesRepository.GetSumByIdAsync(saleId);
            return View(vm);
        }

        public async Task<IActionResult> Edit(int saleId)
        {
            //Роблю перевірку чи є така накладна і чи вона не зафіксована
            var saleStatus = await _salesRepository.GetSatusAsync(saleId);

            switch (saleStatus)
            {
                case 1:
                    TempData["Error"] = "Накладна зафіксована. Редагування заборонено!";
                    return RedirectToAction("Index");

                case 2:
                    TempData["Error"] = "Нема даних!";
                    return RedirectToAction("Index");
            }

            var vm = new SaleViewModel();
            vm.Sale = await _salesRepository.GetByIdAsync(saleId);
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Sale sale)
        {
            //Роблю перевірку чи є така накладна і чи вона не зафіксована

            if (sale.Id > 0)
            {
                var saleStatus = await _salesRepository.GetSatusAsync(sale.Id);

                switch (saleStatus)
                {
                    case 0:
                        var result = await _salesRepository.EditAsync(sale);
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
            var vm = new SaleViewModel();
            vm.Sale = sale;
            vm.Currencys = await _optionsRepository.GetAllCurrencyAsync();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);

        }
    }
}
