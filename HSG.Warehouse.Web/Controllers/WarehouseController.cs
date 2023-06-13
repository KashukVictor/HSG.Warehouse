using HSG.Warehouse.Common.Models;
using HSG.Warehouse.Common.Models.Entity;
using HSG.Warehouse.Interfaces.Repository;
using HSG.Warehouse.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HSG.Warehouse.Web.Controllers
{
    [Authorize]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IOptionsRepository _optionsRepository;

        public WarehouseController(IWarehouseRepository warehouseRepository, IOptionsRepository optionsRepository)
        {
            _warehouseRepository = warehouseRepository;
            _optionsRepository = optionsRepository;
        }
        public async Task<IActionResult> Index(int categoryId)
        {
            int currencId = 0;
            var vm = new WarehouseViewModel();
            vm.Warehouse = await _warehouseRepository.GetProductsAsync(categoryId, currencId);
            var categories = await _optionsRepository.GetAllCategoriesAsync();
            vm.Categories = categories;
            vm.CategoryMenu = new CategoryMenu
            {
                SelectedId = categoryId,
                Categories = categories
            };
            //vm.seededCategories.QueryString = HttpContext.Request.QueryString.ToString();

            return View(vm);
        }
    }
}
