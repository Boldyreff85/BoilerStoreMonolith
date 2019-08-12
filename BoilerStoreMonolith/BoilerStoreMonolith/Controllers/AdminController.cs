using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Concrete;
using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BoilerStoreMonolith.Controllers
{
    public class AdminController : Controller
    {
        private IProductRepository productRepo;
        private ICategoryRepository categoryRepo;
        private ICategoryFeatureRepository categoryFeatureRepo;
        private IFeatureRepository featureRepo;
        private IInfoEntityRepository siteInfoRepo;
        private IFirmRepository firmRepo;
        private ApplicationContext context = new ApplicationContext();

        public AdminController(
            IProductRepository _productRepo,
            ICategoryRepository _categoryRepo,
            ICategoryFeatureRepository _categoryFeatureRepo,
            IFeatureRepository _featureRepo,
            IInfoEntityRepository _siteInfoRepo,
            IFirmRepository _firmRepo
        )
        {
            productRepo = _productRepo;
            categoryRepo = _categoryRepo;
            categoryFeatureRepo = _categoryFeatureRepo;
            featureRepo = _featureRepo;
            siteInfoRepo = _siteInfoRepo;
            firmRepo = _firmRepo;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var products = productRepo.Products.ToList();
            return View(products);
        }


        [HttpPost]
        public ActionResult DeleteSelected(string[] productIds)
        {
            if (productIds == null || productIds.Length == 0)
            {
                TempData["ProductDeletionStatus"] = "Нет выбранных товаров для удаления.";
            }

            List<int> ids = productIds
                .Where(ch => ch != "false").Select(x => Int32.Parse(x)).ToList();
            var count = 0;
            foreach (var id in ids)
            {
                // удаляем товар по id 
                productRepo.DeleteProduct(id);
                // и его features если есть
                var prodFeatures = featureRepo.Features
                    .Where(pf => pf.ProductId == id).ToList();
                if (prodFeatures.Any())
                    featureRepo.DeleteFeatures(prodFeatures);
                count++;
            }

            TempData["ProductDeletionStatus"] = $"Удалено товаров -  {count}";
            return RedirectToAction("Index");
        }

        // тут по id собираем данные и заполняем поля товара
        [HttpGet]
        public ViewResult Edit(AdminEditViewModel model,
             int productId, string categoryName = null, string firmName = null)
        {
            model.Product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            model.Categories = categoryRepo.Categories.Select(c => c.Name).ToList();
            model.Firms = firmRepo.Firms.Select(c => c.Name).ToList();

            if(firmName != null)
                model.Product.Firm = firmName;

            if (categoryName != null)
            {
                model.Product.Category = categoryName;
                model.Features = GetFeatureList(categoryName, productId);
            }
            else
            {
                model.Features = GetFeatureList(model.Product.Category, productId);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditViewModel model,HttpPostedFileBase productImg = null)
        {
            if (ModelState.IsValid)
            {
                ///////////////////////////////  processing products table ///////////////////////////////
                // сохраняем товар
                Product product = model.Product;
                if (productImg != null)
                {

                    product.ImageMimeType = productImg.ContentType;
                    product.ImageData = new byte[productImg.ContentLength];
                    productImg.InputStream.Read(
                        product.ImageData, 0, productImg.ContentLength);
                }
                productRepo.SaveProduct(product);

                /////////////////////////////// processing features table ///////////////////////////////
                // clearing product features from table
                var prodFeatures = featureRepo.Features
                    .Where(pf => pf.ProductId == product.ProductID).ToList();
                if (prodFeatures.Any())
                {
                    featureRepo.DeleteFeatures(prodFeatures);
                }
                // saving features of product
                if (model.Features?.Count > 0)
                {
                    foreach (var feature in model.Features)
                    {
                        var productFeature = new Feature
                        {
                            Name = feature.Name,
                            Value = feature.Value,
                            ProductId = product.ProductID
                        };
                        featureRepo.SaveFeature(productFeature);
                    }
                }

                TempData["category"] = string.Format("{0} has been saved", product.Title);

                return RedirectToAction("Index");
            }
            else
            {
                return View("Edit", model);
            }


        }


        [HttpGet]
        public ViewResult Create(string categoryName = null, string firmName = null)
        {
            var model = new AdminEditViewModel();
            model.Product = new Product();
            model.Categories = categoryRepo.Categories.Select(c => c.Name).ToList();
            model.Firms = firmRepo.Firms.Select(c => c.Name).ToList();

            model.Product.Category = categoryName ?? model.Categories[0];
            model.Product.Firm = firmName ?? model.Firms[0];

            model.Features = GetCategoryFeatureList(model.Product.Category);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(
            AdminEditViewModel model, HttpPostedFileBase productImg = null)
        {
            if (ModelState.IsValid)
            {
                // сохраняем товар
                Product product = model.Product;
                if (productImg != null)
                {

                    product.ImageMimeType = productImg.ContentType;
                    product.ImageData = new byte[productImg.ContentLength];
                    productImg.InputStream.Read(
                        product.ImageData, 0, productImg.ContentLength);
                }
                productRepo.SaveProduct(product);

                // сохраняем features 
                if (model.Features?.Count > 0)
                {
                    foreach (var feature in model.Features)
                    {
                        var productFeature = new Feature
                        {
                            Name = feature.Name,
                            Value = feature.Value,
                            ProductId = product.ProductID
                        };
                        featureRepo.SaveFeature(productFeature);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
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

            model.Category = categoryRepo.Categories.Single(c => c.Id == categoryId);

            model.CategoryFeaturesNames = categoryFeatureRepo.CategoryFeatures
                .Where(cf => cf.CategoryId == categoryId).Select(cfn => cfn.Name).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCategories(
            EditCategoriesViewModel model,
            HttpPostedFileBase categoryImg = null)
        {

            if (categoryImg != null)
            {
                model.Category.ImageMimeType = categoryImg.ContentType;
                model.Category.ImageData = new byte[categoryImg.ContentLength];
                categoryImg.InputStream.Read(
                    model.Category.ImageData, 0, categoryImg.ContentLength);
            }

            categoryRepo.SaveCategory(model.Category);

            // saving category features
            if (model.CategoryFeaturesNames?.Count > 0)
            {
                //// clear table
                //if (categoryFeatureRepo.CategoryFeatures?.Any() == true)
                //    categoryFeatureRepo
                //        .DeleteCategoryFeatures(categoryFeatureRepo.CategoryFeatures.ToList());

                // clear features if this category
                var categoryFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == model.Category.Id)
                    .ToList();
                if (categoryFeatures.Any())
                    categoryFeatureRepo.DeleteCategoryFeatures(categoryFeatures);


                foreach (var categoryFeaturesName in model.CategoryFeaturesNames)
                {
                    var catFeature = new CategoryFeature
                    {
                        Name = categoryFeaturesName,
                        CategoryId = model.Category.Id
                    };
                    categoryFeatureRepo.SaveCategoryFeature(catFeature);
                }
            }


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
        // ******** firms ********

        [HttpGet]
        public ActionResult EditFirms()
        {

            var model = new IndexFirmsViewModel
            {
                Firms = firmRepo.Firms.ToList()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult EditFirms(IndexFirmsViewModel model,
            HttpPostedFileBase firmImg = null)
        {
            var firm = new Firm();
            firm.Name = model.NewFirmName;
            if (firmImg != null)
            {
                firm.ImageMimeType = firmImg.ContentType;
                firm.ImageData = new byte[firmImg.ContentLength];
                firmImg.InputStream.Read(
                    firm.ImageData, 0, firmImg.ContentLength);
            }
            firmRepo.SaveFirm(firm);
            return RedirectToRoute(new{controller = "Admin", action="EditFirms"});
        }

 
        public ActionResult DeleteFirm(int firmId)
        {

            firmRepo.DeleteFirm(firmId);
            return RedirectToRoute(new{controller = "Admin", action="EditFirms"});
        }


        public List<Feature> GetCategoryFeatureList(string categoryName)
        {
            var categories = categoryRepo.Categories.ToList();
            var catFeatures = new List<string>();
            // getting category features
            if (categories.Any() && categoryName != null)
            {
                var categoryId = categories
                    .Where(c => c.Name == categoryName)
                    .Select(c => c.Id)
                    .Single();
                catFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.Name)
                    .ToList();
            }

            var featuresList = new List<Feature>();
            // feeding feature list with newly created items containing names
            foreach (var catFeature in catFeatures)
            {
                featuresList.Add(new Feature
                {
                    Name = catFeature,
                    Value = ""
                });
            }
            return featuresList;
        }

        public List<Feature> GetFeatureList(string categoryName, int productId)
        {

            var categories = categoryRepo.Categories.ToList();
            var catFeatures = new List<string>();
            // getting category features
            if (categories.Any() && categoryName != null)
            {
                var categoryId = categories
                    .Where(c => c.Name == categoryName)
                    .Select(c => c.Id)
                    .Single();
                catFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.Name)
                    .ToList();
            }

            var featuresList = new List<Feature>();
            // feeding feature list with newly created items containing names
            foreach (var catFeature in catFeatures)
            {
                var featureValue = "";
                var prodFeatures = featureRepo.Features
                    .Where(f => f.Name == catFeature && f.ProductId == productId)
                    .ToList();
                if (prodFeatures.Any())
                    featureValue = prodFeatures
                        .Where(f => f.Name == catFeature && f.ProductId == productId)
                        .Select(f => f.Value)
                        .Single();

                featuresList.Add(new Feature
                {
                    Name = catFeature,
                    Value = featureValue ?? ""
                });
            }

            return featuresList;
        }


        // *************************************************************************************


        // helpers
        public FileContentResult GetImageFromCategoryTable(int categoryId)
        {
            Category category = categoryRepo.Categories
                .FirstOrDefault(p => p.Id == categoryId);
            if (category != null)
            {
                if (category.ImageData != null && category.ImageMimeType != null)
                {
                    return File(category.ImageData, category.ImageMimeType);
                }
            }
            return null;
        }

        public FileContentResult GetImageFromFirmTable(string firmName)
        {
            Firm firm = context.Firms.FirstOrDefault(f => f.Name == firmName);
            if (firm != null)
            {
                if (firm.ImageData != null && firm.ImageMimeType != null)
                {
                    return File(firm.ImageData, firm.ImageMimeType);
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