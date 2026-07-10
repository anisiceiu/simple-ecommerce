using ecommerce.Models;
using ECommerce.Application.Interfaces;
using ECommerce.Domain;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
   
        public class InventoryOperationsController : Controller
        {
            private readonly IProductService _productService;
            private readonly IInventoryTransactionService _inventoryService;

            public InventoryOperationsController(
                IProductService productService,
                IInventoryTransactionService inventoryService)
            {
                _productService = productService;
                _inventoryService = inventoryService;
            }

            #region Stock In

            [HttpGet]
            public async Task<IActionResult> StockIn(int id)
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var vm = new StockInViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name
                };

                return View(vm);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> StockIn(StockInViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var product = await _productService.GetByIdAsync(model.ProductId);

                if (product == null)
                    return NotFound();

                int previousStock = product.Stock;

                product.Stock += model.Quantity;

                bool updated = await _productService.UpdateAsync(product.Id, product);

                if (!updated)
                {
                    ModelState.AddModelError("", "Unable to update stock.");
                    return View(model);
                }

                var transaction = new InventoryTransaction
                {
                    ProductId = product.Id,
                    TransactionType = InventoryTransactionType.StockIn,
                    Quantity = model.Quantity,
                    PreviousStock = previousStock,
                    NewStock = product.Stock,
                    ReferenceNo = model.ReferenceNo,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _inventoryService.AddAsync(transaction);
                await _inventoryService.SaveChangesAsync();

                TempData["Success"] = "Stock added successfully.";

                return RedirectToAction("Index", "Inventory");
            }

            #endregion

            #region Stock Out

            [HttpGet]
            public async Task<IActionResult> StockOut(int id)
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var vm = new StockOutViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name
                };

                return View(vm);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> StockOut(StockOutViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var product = await _productService.GetByIdAsync(model.ProductId);

                if (product == null)
                    return NotFound();

                if (model.Quantity > product.Stock)
                {
                    ModelState.AddModelError("", "Insufficient stock.");
                    return View(model);
                }

                int previousStock = product.Stock;

                product.Stock -= model.Quantity;

                bool updated = await _productService.UpdateAsync(product.Id, product);

                if (!updated)
                {
                    ModelState.AddModelError("", "Unable to update stock.");
                    return View(model);
                }

                var transaction = new InventoryTransaction
                {
                    ProductId = product.Id,
                    TransactionType = InventoryTransactionType.StockOut,
                    Quantity = model.Quantity,
                    PreviousStock = previousStock,
                    NewStock = product.Stock,
                    ReferenceNo = model.ReferenceNo,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _inventoryService.AddAsync(transaction);
                await _inventoryService.SaveChangesAsync();

                TempData["Success"] = "Stock removed successfully.";

                return RedirectToAction("Index", "Inventory");
            }

            #endregion

            #region Stock Adjustment

            [HttpGet]
            public async Task<IActionResult> Adjustment(int id)
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                    return NotFound();

                var vm = new StockAdjustmentViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    NewStock = product.Stock
                };

                return View(vm);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Adjustment(StockAdjustmentViewModel model)
            {
                if (!ModelState.IsValid)
                    return View(model);

                var product = await _productService.GetByIdAsync(model.ProductId);

                if (product == null)
                    return NotFound();

                int previousStock = product.Stock;

                product.Stock = model.NewStock;

                bool updated = await _productService.UpdateAsync(product.Id, product);

                if (!updated)
                {
                    ModelState.AddModelError("", "Unable to update stock.");
                    return View(model);
                }

                var transaction = new InventoryTransaction
                {
                    ProductId = product.Id,
                    TransactionType = InventoryTransactionType.Adjustment,
                    Quantity = Math.Abs(model.NewStock - previousStock),
                    PreviousStock = previousStock,
                    NewStock = model.NewStock,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _inventoryService.AddAsync(transaction);
                await _inventoryService.SaveChangesAsync();

                TempData["Success"] = "Stock adjusted successfully.";

                return RedirectToAction("Index", "Inventory");
            }

            #endregion
        }
   
}
