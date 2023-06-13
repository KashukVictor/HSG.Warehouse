using HSG.Warehouse.ClientServices.NbuCurrency;
using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSG.Warehouse.Web.Controllers
{
    [Authorize]
    public class OptionsController : Controller
    {
        //private readonly AppDbContext _context;
        private readonly IOptionsRepository _optionsRepository;
        private readonly INbuCurrencyServiceClient _currencyApi;

        public OptionsController(IOptionsRepository optionsRepository, INbuCurrencyServiceClient nbuCurrency)
        {
            //_context = context;
            _optionsRepository = optionsRepository;
            _currencyApi = nbuCurrency;

        }

        public IActionResult Index()
        {
            return View();
        }

        #region ОДИНИЦІ ВИМІРУ
        public async Task<IActionResult> Units(int unitId)
        {
            var vm = new UnitViewModel();
            vm.Units = await _optionsRepository.GetAllUnitsAsync();
            vm.Unit = await _optionsRepository.GetUnitByIdAsync(unitId);

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddUnit(Unit unit)
        {
            TempData["Error"] = await _optionsRepository.AddUnitAsync(unit);
            return RedirectToAction("Units");
        }
        public async Task<IActionResult> EditUnit(Unit unit)
        {
            TempData["Error"] = await _optionsRepository.EditUnitAsync(unit); ;
            return RedirectToAction("Units");
        }
        public async Task<IActionResult> DeleteUnit(int unitId)
        {
            TempData["Error"] = await _optionsRepository.DeleteUnitAsync(unitId);

            return RedirectToAction("Units");
        }
        #endregion
        #region ВИРОБНИКИ / БРЕНДИ
        public async Task<IActionResult> Manufacturer(int manufacturerId)
        {
            var vm = new ManufacturerViewModel();
            vm.Manufacturer = await _optionsRepository.GetManufacturerByIdAsync(manufacturerId);
            vm.Manufacturers = await _optionsRepository.GetAllManufacturerAsync();

            return View(vm);
        }
        public async Task<IActionResult> AddManufacturer(Manufacturer manufacturer)
        {
            TempData["Error"] = await _optionsRepository.AddManufacturerAsync(manufacturer);

            return RedirectToAction("Manufacturer");
        }
        public async Task<IActionResult> EditManufacturer(Manufacturer manufacturer)
        {
            TempData["Error"] = await _optionsRepository.EditManufacturerAsync(manufacturer);

            return RedirectToAction("Manufacturer");
        }
        public async Task<IActionResult> DeleteManufacturer(int manufacturerId)
        {
            TempData["Error"] = await _optionsRepository.DeleteManufacturerAsync(manufacturerId);
            return RedirectToAction("Manufacturer");
        }
        #endregion
        #region КАТЕГОРІЇ
        public async Task<IActionResult> Category(int categoryId)
        {
            var vm = new CategoryViewModel();
            vm.Categories = await _optionsRepository.GetAllCategoriesAsync();
            vm.Category = await _optionsRepository.GetCategoryByIdAsync(categoryId);

            return View(vm);
        }
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            TempData["Error"] = await _optionsRepository.DeleteCategoryAsync(categoryId); ;
            return RedirectToAction("Category");
        }
        public async Task<IActionResult> EditCategory(Category category)
        {
            TempData["Error"] = await _optionsRepository.EditCategoryAsync(category); ;

            return RedirectToAction("Category");
        }
        public async Task<IActionResult> AddCategory(Category category)
        {
            TempData["Error"] = await _optionsRepository.AddCategoryAsync(category);

            return RedirectToAction("Category");
        }

        #endregion
        #region ПРОДУКЦІЯ 
        public async Task<IActionResult> ProductsList(int categoryId)
        {
            var categories = await _optionsRepository.GetAllCategoriesAsync();
            var vm = new ProductsListViewModel();
            vm.Categories = categories;
            vm.Products = await _optionsRepository.GetProductsFromCategoryAsync(categoryId);

            vm.CategoryMenu = new CategoryMenu
            {
                SelectedId = categoryId,
                Categories = categories,
            };
           
            return View(vm);

        }
        public async Task<IActionResult> Product(int productId)
        {
            //_optionsRepository.GenerateBarcode();
            var vm = new ProductViewModel();
            vm.Categories = await _optionsRepository.GetAllCategoriesAsync();
            vm.Manufacturers = await _optionsRepository.GetAllManufacturerAsync();
            vm.Units = await _optionsRepository.GetAllUnitsAsync();
            vm.Product = await _optionsRepository.GetProductByIdAsync(productId);

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Product(ProductViewModel productViewModel)
        {
            var product = productViewModel.Product;

            string? error = null;
            // 0 - Створення нового товару
            if (productViewModel.Product?.Id == 0)
            {
                error = await _optionsRepository.AddProductAsync(product, productViewModel.IsAutoBarcode);
            }
            else
            {
                error = await _optionsRepository.EditProductAsync(product);
            }

            if (error == null)
            {
                return RedirectToAction("ProductsList");
            }

            productViewModel.Categories = await _optionsRepository.GetAllCategoriesAsync();
            productViewModel.Manufacturers = await _optionsRepository.GetAllManufacturerAsync();
            productViewModel.Units = await _optionsRepository.GetAllUnitsAsync();

            TempData["Error"] = error;
            return View(productViewModel);
        }
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            TempData["Error"] = await _optionsRepository.DeleteProductAsync(productId);
            return RedirectToAction("ProductsList");
        }
        #endregion
        #region ВАЛЮТА
        public async Task<IActionResult> Currency(DateTime dateTime)
        {
            var value = _currencyApi.GetCurrencyOnDateAsync(DateTime.Now);
            var vm = new CurrencyViewModel();

            vm.Currency = await _optionsRepository.GetAllCurrencyAsync();

            dateTime = DateTime.Now;
            vm.CurrencyNBU = await _currencyApi.GetCurrencyOnDateAsync(dateTime);
            return View(vm);
        }
        public async Task<IActionResult> DeleteCurrency(int currencyId)
        {
            TempData["Error"] = await _optionsRepository.DeleteCurrencyAsync(currencyId);
            return RedirectToAction("Currency");
        }
        public async Task<IActionResult> AddCurrency(int currencyId)
        {
            //Отримую дазі з сайту НБУ
            var currencyList = await _currencyApi.GetCurrencyOnDateAsync(DateTime.Now);
            var currency = currencyList?.Where(c => c?.Code == currencyId).FirstOrDefault();

            TempData["Error"] = await _optionsRepository.AddCurrencyAsync(currency);

            return RedirectToAction("Currency");
        }
        #endregion
        #region КЛІЄНТИ
        public async Task<IActionResult> ClientsList()
        {
            var vm = new ClientsListViewModel();
            vm.Clients = await _optionsRepository.GetAllClientsAsync();

            return View(vm);
        }
        public async Task<IActionResult> Client(int clientId)
        {
            var client = new Client();
            client = await _optionsRepository.GetClientByIdAsync(clientId);

            return View(client);
        }
        [HttpPost]
        public async Task<IActionResult> AddClient(Client client)
        {
            TempData["Error"] = await _optionsRepository.AddClientAsync(client);
            return RedirectToAction("ClientsList");
        }
        [HttpPost]
        public async Task<IActionResult> EditClient(Client client)
        {
            TempData["Error"] = await _optionsRepository.EditClientAsync(client);
            return RedirectToAction("ClientsList");
        }
        public async Task<IActionResult> DeleteClient(int clientId)
        {
            TempData["Error"] = await _optionsRepository.DeleteClientAsync(clientId);
            return RedirectToAction("ClientsList");
        }
        #endregion
    }
}
