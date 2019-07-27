using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Concrete;
using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace BoilerStoreMonolith.Controllers
{
    public class AdminController : Controller
    {
        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;
        private IInfoEntityRepository siteInfoRepo;
        private ApplicationContext context = new ApplicationContext();
        public AdminController(
            IProductRepository _productRepo,
            IInfoEntityRepository _siteInfoRepo,
            ICategoryRepository _categoryRepositoryRepo)
        {
            productRepo = _productRepo;
            siteInfoRepo = _siteInfoRepo;
            categoryRepo = _categoryRepositoryRepo;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(productRepo.Products.ToList());
        }


        [HttpPost]
        public ActionResult DeleteSelected(string[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
            {
                TempData["ProductDeletionStatus"] = "Нет выбранных товаров для удаления.";
            }

            List<int> ids = productIds.Where(ch => ch != "false").Select(x => Int32.Parse(x)).ToList();
            var count = 0;
            foreach (var id in ids)
            {
                productRepo.DeleteProduct(id);
                count++;
            }

            TempData["ProductDeletionStatus"] = $"Удалено товаров -  {count}";
            return RedirectToAction("Index");
        }


        public ViewResult Edit(AdminEditViewModel model, int productId)
        {
            model.Product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);

            ViewBag.categories = new SelectList(
                    context.Categories.Select(c => c.Name),
                    model.Product.Category
                );
            ViewBag.ImageToLoad = "categoryImg";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditViewModel model,
                HttpPostedFileBase productImg = null, HttpPostedFileBase categoryImg = null, HttpPostedFileBase firmImg = null)
        {
            Product product = model.Product;
            if (ModelState.IsValid)
            {
                // добавляем категорию в таблицу категорий
                context.Categories.Add(
                    new Category
                    {
                        Name = model.Product.Category
                    });
                context.SaveChanges();

                if (productImg != null)
                {
                    product.ImageMimeType = productImg.ContentType;
                    product.ImageData = new byte[productImg.ContentLength];
                    productImg.InputStream.Read(product.ImageData, 0, productImg.ContentLength);
                }
                productRepo.SaveProduct(product);

                if (categoryImg != null)
                {
                    product.CategoryImageMimeType = categoryImg.ContentType;
                    product.CategoryImageData = new byte[categoryImg.ContentLength];
                    categoryImg.InputStream.Read(product.CategoryImageData, 0, categoryImg.ContentLength);
                }
                productRepo.SaveProduct(product);

                if (firmImg != null)
                {
                    product.FirmImageMimeType = firmImg.ContentType;
                    product.FirmImageData = new byte[firmImg.ContentLength];
                    firmImg.InputStream.Read(product.FirmImageData, 0, firmImg.ContentLength);
                }

                productRepo.SaveProduct(product);
                TempData["category"] = string.Format("{0} has been saved", product.Title);

                return RedirectToAction("Index");
            }
            else
            {// there is something wrong with the data values
                return View(model);
            }
        }

        public ViewResult Create()
        {
            var model = new AdminEditViewModel
            {
                Product = new Product(),
                ImageToLoad = ""
            };
            return View("Edit", model);
        }

        public ViewResult EditSiteInfo()
        {
            return View(siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult EditSiteInfo(InfoEntity infoEntity, HttpPostedFileBase logoImg = null, HttpPostedFileBase homeImg = null)
        {
            if (ModelState.IsValid)
            {

                if (logoImg != null)
                {
                    infoEntity.ImageMimeType = logoImg.ContentType;
                    infoEntity.ImageData = new byte[logoImg.ContentLength];
                    logoImg.InputStream.Read(infoEntity.ImageData, 0, logoImg.ContentLength);
                }
                siteInfoRepo.SaveInfoEntity(infoEntity);

                if (homeImg != null)
                {
                    infoEntity.ImageMimeType2 = homeImg.ContentType;
                    infoEntity.ImageData2 = new byte[homeImg.ContentLength];
                    homeImg.InputStream.Read(infoEntity.ImageData2, 0, homeImg.ContentLength);
                }

                siteInfoRepo.SaveInfoEntity(infoEntity);

                TempData["category"] = string.Format("Общая информация о сайте обновлена");
                return RedirectToAction("Index");
            }
            else
            {// there is something wrong with the data values
                return View(infoEntity);
            }
        }


        // *************************************************************************************
        // ******** categories ********


        [HttpGet]
        public ActionResult IndexCategories()
        {
            ViewBag.ImageToLoad = "categoryImg";
            return View(categoryRepo.Categories.ToList());
        }

        [HttpGet]
        public ActionResult EditCategories(EditCategoriesViewModel model, int categoryId)
        {
            model.Category = categoryRepo.Categories
                .SingleOrDefault(c => c.Id == categoryId);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCategories(
            EditCategoriesViewModel model,
            HttpPostedFileBase categoryImg = null)
        {
            var specs = new List<CategorySpec>();

            foreach (var item in model.Specs)
            {
                specs.Add(new CategorySpec
                {
                    Name = item
                });
            }

            // clear specs table
            var tableSpecs = context.CategorySpecs;
            context.CategorySpecs.RemoveRange(tableSpecs);
            // add new specs
            model.Category.CategorySpecs = specs;
            context.SaveChangesAsync();

            if (categoryImg != null)
            {
                model.Category.ImageMimeType = categoryImg.ContentType;
                model.Category.ImageData = new byte[categoryImg.ContentLength];
                categoryImg.InputStream.Read(
                    model.Category.ImageData, 0, categoryImg.ContentLength);
            }
;
            categoryRepo.SaveCategory(model.Category);

            return RedirectToAction("IndexCategories");
        }

        public ViewResult CreateCategory()
        {
            var model = new EditCategoriesViewModel
            {
                Category = new Category()
            };
            return View("EditCategories", model);
        }

        [HttpPost]
        public ActionResult DeleteCategoriesSelected(string[] categoriesIds)
        {
            if (categoriesIds == null || categoriesIds.Length == 0)
            {
                TempData["CategoryDeletionStatus"] = "Нет выбранных категорий для удаления.";
                return RedirectToAction("IndexCategories");
            }

            List<int> ids = categoriesIds.Where(ch => ch != "false").Select(x => Int32.Parse(x)).ToList();
            var count = 0;

            // находим и удаляем категории
            foreach (var id in ids)
            {
                Category dbEntry = context.Categories.Find(id);
                if (dbEntry != null)
                {
                    context.Categories.Remove(dbEntry);
                    context.SaveChanges();
                }
                count++;
            }

            TempData["CategoryDeletionStatus"] = $"Удалено категорий -  {count}";
            return RedirectToAction("IndexCategories");
        }

        // *************************************************************************************


        // helpers
        public FileContentResult GetCategoryImageFromCategoryTable(int categoryId)
        {
            Category category = categoryRepo.Categories.FirstOrDefault(p => p.Id == categoryId);
            if (category != null)
            {
                if (category.ImageData != null && category.ImageMimeType != null)
                {
                    return File(category.ImageData, category.ImageMimeType);
                }
            }
            return null;
        }

        public FileContentResult GetImage(int infoId, string imgElementName)
        {
            InfoEntity infoEntity = siteInfoRepo.InfoEntities.FirstOrDefault(p => p.InfoID == infoId);
            if (infoEntity != null)
            {
                if (infoEntity.ImageData != null && infoEntity.ImageMimeType != null)
                {
                    return File(infoEntity.ImageData, infoEntity.ImageMimeType);
                }
            }
            return null;
        }

        public FileContentResult GetImage2(int infoId, string imgElementName)
        {
            InfoEntity infoEntity = siteInfoRepo.InfoEntities.FirstOrDefault(p => p.InfoID == infoId);
            if (infoEntity != null)
            {
                if (infoEntity.ImageData2 != null && infoEntity.ImageMimeType2 != null)
                {
                    return File(infoEntity.ImageData2, infoEntity.ImageMimeType2);
                }
            }
            return null;
        }

    }

}