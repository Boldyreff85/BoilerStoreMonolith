using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Concrete;
using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Models;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private IProductFeatureRepository productFeatureRepo;
        private IFeatureRepository featureRepo;
        private IDescriptionFeatureRepository descriptionFeatureRepo;
        private IInfoEntityRepository siteInfoRepo;
        private IFirmRepository firmRepo;
        private ApplicationContext context = new ApplicationContext();

        public AdminController(
            IProductRepository _productRepo,
            ICategoryRepository _categoryRepo,
            ICategoryFeatureRepository _categoryFeatureRepo,
            IFeatureRepository _featureRepo,
            IDescriptionFeatureRepository _descriptionFeatureRepo,
            IProductFeatureRepository _productFeatureRepo,
            IInfoEntityRepository _siteInfoRepo,
            IFirmRepository _firmRepo
        )
        {
            productRepo = _productRepo;
            categoryRepo = _categoryRepo;
            categoryFeatureRepo = _categoryFeatureRepo;
            featureRepo = _featureRepo;
            descriptionFeatureRepo = _descriptionFeatureRepo;
            productFeatureRepo = _productFeatureRepo;
            siteInfoRepo = _siteInfoRepo;
            firmRepo = _firmRepo;
        }

        public static byte[] resizeImage(HttpPostedFileBase image)
        {
            using (MagickImage resultImage = new MagickImage())
            {
                MagickGeometry size = new MagickGeometry(300, 300);
                System.Drawing.Bitmap sourceimage = (Bitmap)Image.FromStream(image.InputStream);
                resultImage.Read(sourceimage);
                resultImage.Resize(size);
                resultImage.Format = MagickFormat.Jpeg;

                return resultImage.ToByteArray();
            }
        }

        public static bool validateImage(HttpPostedFileBase image)
        {
            return image != null && image.ContentLength > 0 && (image.ContentType == "image/jpeg" || image.ContentType == "image/png");
        }

        [HttpGet]
        public ActionResult Index(
            AdminIndexListViewModel model,
            string category = null,
            string firm = null)
        {
            ViewBag.Category = category;
            ViewBag.Firm = firm;

            model.Products = productRepo.Products.ToList();
            model.Products = FilterProductList(model.Products, category, firm);

            model.Categories = categoryRepo.Categories.Select(n => n.Name).Distinct().ToList();
            model.Firms = firmRepo.Firms.Select(n => n.Name).Distinct().ToList();

            return View(model);
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
                var prodFeatures = productFeatureRepo.ProductFeatures
                    .Where(pf => pf.ProductId == id).ToList();
                if (prodFeatures.Any())
                    productFeatureRepo.DeleteFeatures(prodFeatures);
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

            if (firmName != null)
                model.Product.Firm = firmName;
            if (categoryName != null)
                model.Product.Category = categoryName;

            model.ProductFeatures = GetProductFeatures(productId, model.Product.Category);
            model.DescriptionFeatures = descriptionFeatureRepo.DescriptionFeatures
                .Where(df => df.ProductId == productId).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditViewModel model, HttpPostedFileBase productImg = null)
        {
            if (ModelState.IsValid)
            {
                ///////////////////////////////  processing products table ///////////////////////////////
                // сохраняем товар
                Product product = model.Product;
                bool isImageValid = validateImage(productImg);
                if (isImageValid)
                {
                    product.ImageMimeType = productImg.ContentType;
                    product.ImageData = resizeImage(productImg);
                }
                else
                {
                    Category productCategory = categoryRepo.Categories.Single(c => c.Name == product.Category);
                    product.ImageMimeType = productCategory.ImageMimeType;
                    product.ImageData = productCategory.ImageData;
                }

                productRepo.SaveProduct(product);

                /////////////////////////////// processing description features table ///////////////////////////////
                if (model.DescriptionFeatures?.Count > 0)
                {
                    // очищаем предварительно список
                    var listToClear = descriptionFeatureRepo.DescriptionFeatures
                        .Where(df => df.ProductId == product.ProductID).ToList();
                    if (listToClear?.Count > 0)
                        descriptionFeatureRepo.DeleteDescriptionFeatures(listToClear);

                    foreach (var descFeature in model.DescriptionFeatures)
                    {
                        // добавляем принадлежность к товару
                        descFeature.ProductId = product.ProductID;
                        descriptionFeatureRepo.SaveFeature(descFeature);
                    }
                }


                /////////////////////////////// processing features table ///////////////////////////////
                // clearing product features from table
                var prodFeatures = productFeatureRepo.ProductFeatures
                    .Where(pf => pf.ProductId == product.ProductID).ToList();
                if (prodFeatures.Any())
                {
                    productFeatureRepo.DeleteFeatures(prodFeatures);
                }
                // saving features of product
                if (model.ProductFeatures?.Count > 0)
                {
                    foreach (var feature in model.ProductFeatures)
                    {
                        var productFeature = new ProductFeature
                        {
                            Name = feature.Name,
                            Value = feature.Value,
                            Unit = feature.Unit,
                            ProductId = product.ProductID
                        };
                        productFeatureRepo.SaveFeature(productFeature);
                    }
                }

                TempData["category"] = string.Format("{0} has been saved", product.Title);

                return RedirectToAction("Index");
            }
            else
            {
                model.Categories = categoryRepo.Categories.Select(c => c.Name).ToList();
                model.Firms = firmRepo.Firms.Select(c => c.Name).ToList();
                return View(model);
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

            model.ProductFeatures = GetCategoryFeatureList(model.Product.Category);

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
                bool isImageValid = validateImage(productImg);
                if (isImageValid)
                {
                    product.ImageMimeType = productImg.ContentType;
                    product.ImageData = resizeImage(productImg);
                }
                else
                {
                    Category productCategory = categoryRepo.Categories.Single(c => c.Name == product.Category);
                    product.ImageMimeType = productCategory.ImageMimeType;
                    product.ImageData = productCategory.ImageData;
                }
                productRepo.SaveProduct(product);

                // сохраняем features 
                if (model.ProductFeatures?.Count > 0)
                {
                    foreach (var feature in model.ProductFeatures)
                    {
                        var productFeature = new ProductFeature
                        {
                            Name = feature.Name,
                            Value = feature.Value,
                            Unit = feature.Unit,
                            ProductId = product.ProductID
                        };
                        productFeatureRepo.SaveFeature(productFeature);
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
        public ActionResult EditCategories(
            EditCategoriesViewModel model,
            int categoryId)
        {

            model.Category = categoryRepo.Categories.Single(c => c.Id == categoryId);

            model.CategoryFeaturesIds = categoryFeatureRepo.CategoryFeatures
                .Where(cf => cf.CategoryId == categoryId)
                .Select(cf => cf.FeatureId)
                .ToList();

            model.CategoryFeaturesNames = model.CategoryFeaturesIds.Join(featureRepo.Features,
                p => p,
                t => t.Id,
                (p, t) => t.Name).ToList();

            model.Features = featureRepo.Features.ToList();

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
            if (model.CategoryFeaturesIds?.Count > 0)
            {
                // clear features if this category
                var categoryFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == model.Category.Id)
                    .ToList();
                if (categoryFeatures.Any())
                    categoryFeatureRepo.DeleteCategoryFeatures(categoryFeatures);

                foreach (var featureId in model.CategoryFeaturesIds)
                {
                    var catFeature = new CategoryFeature
                    {
                        CategoryId = model.Category.Id,
                        FeatureId = featureId
                    };
                    categoryFeatureRepo.SaveCategoryFeature(catFeature);
                }
            }

            return RedirectToAction("IndexCategories");
        }

        [HttpGet]
        public ActionResult CreateCategory()
        {
            var model = new EditCategoriesViewModel
            {
                Category = new Category(),
                Features = featureRepo.Features.ToList()
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

            List<int> ids = categoriesIds.Where(ch => ch != "false")
                .Select(x => Int32.Parse(x)).ToList();
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

                // удаляем features категории
                var catFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == id)
                    .ToList();
                categoryFeatureRepo.DeleteCategoryFeatures(catFeatures);

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
                firm.ImageData = resizeImage(firmImg);
            }
            firmRepo.SaveFirm(firm);
            return RedirectToRoute(new { controller = "Admin", action = "EditFirms" });
        }


        public ActionResult DeleteFirm(int firmId)
        {

            firmRepo.DeleteFirm(firmId);
            return RedirectToRoute(new { controller = "Admin", action = "EditFirms" });
        }


        public List<ProductFeature> GetCategoryFeatureList(string categoryName)
        {
            var categories = categoryRepo.Categories.ToList();
            var catFeatureIds = new List<int>();
            var catFeatures = new List<string>();
            // getting category features
            if (categoryRepo.Categories.Any(c => c.Name == categoryName))
            {
                var categoryId = categories
                    .Where(c => c.Name == categoryName)
                    .Select(c => c.Id)
                    .Single();
                catFeatureIds = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.FeatureId)
                    .ToList();
                catFeatures = catFeatureIds.Join(featureRepo.Features,
                    p => p,
                    t => t.Id,
                    (p, t) => t.Name).ToList();
            }

            var featuresList = new List<ProductFeature>();
            // feeding feature list with newly created items containing names
            foreach (var catFeature in catFeatures)
            {
                featuresList.Add(new ProductFeature
                {
                    Name = catFeature,
                    Value = "",
                    Unit = ""
                });
            }
            return featuresList;
        }

        public List<ProductFeature> GetProductFeatures(int productId, string categoryName)
        {
            // сначала получаем список фич категории

            var catFeatures = new List<string>();
            var prodFeatures = new List<ProductFeature>();
            var result = new List<ProductFeature>();

            if (categoryRepo.Categories.Any(c => c.Name == categoryName) &&
                categoryFeatureRepo.CategoryFeatures.Any() &&
                featureRepo.Features.Any())
            {
                var categoryId = categoryRepo.Categories
                .Where(c => c.Name == categoryName)
                .Select(c => c.Id)
                .Single();

                catFeatures = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf =>
                            featureRepo.Features
                                .Where(f => f.Id == cf.FeatureId)
                                .Select(f => f.Name).Single())
                    .ToList();

                // далее получаем список фич продукта
                prodFeatures = productFeatureRepo.ProductFeatures
                    .Where(f => f.ProductId == productId)
                    .ToList();

                // далее делаем join двух списков

                result = catFeatures.Join(prodFeatures,
                       p => p,
                       t => t.Name,
                       (p, t) => t).ToList();
            }

            if (result.Count == 0)
                result = GetCategoryFeatureList(categoryName);

            return result;
        }

        public List<ProductFeature> GetFeatureList_(string categoryName, int productId)
        {

            var categories = categoryRepo.Categories.ToList();
            var catFeatureIds = new List<int>();
            var catFeatures = new List<string>();
            // getting category features
            if (categories.Any() && categoryName != null)
            {
                var categoryId = categories
                    .Where(c => c.Name == categoryName)
                    .Select(c => c.Id)
                    .Single();

                catFeatureIds = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.FeatureId)
                    .ToList();

                catFeatures = catFeatureIds.Join(productFeatureRepo.ProductFeatures,
                    p => p,
                    t => t.Id,
                    (p, t) => t.Name).ToList();
            }

            var featuresList = new List<ProductFeature>();
            // feeding feature list with newly created items containing names
            foreach (var catFeature in catFeatures)
            {
                var featureValue = "";
                var featureUnit = "";
                var prodFeatures = productFeatureRepo.ProductFeatures
                    .Where(f => f.Name == catFeature && f.ProductId == productId)
                    .ToList();
                if (prodFeatures.Any())
                {
                    featureValue = prodFeatures
                        .Where(f => f.Name == catFeature && f.ProductId == productId)
                        .Select(f => f.Value)
                        .Single();
                    featureUnit = prodFeatures
                        .Where(f => f.Name == catFeature && f.ProductId == productId)
                        .Select(f => f.Unit)
                        .Single();
                }


                featuresList.Add(new ProductFeature
                {
                    Name = catFeature,
                    Value = featureValue ?? "",
                    Unit = featureUnit ?? ""
                });
            }

            return featuresList;
        }

        // *************************************************************************************
        // ******** features ********

        [HttpGet]
        public ActionResult EditFeatures()
        {

            var model = new IndexFeaturesViewModel
            {
                Features = featureRepo.Features.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditFeatures(IndexFeaturesViewModel model)
        {
            var feature = new Feature();
            feature.Name = model.NewFeatureName;

            featureRepo.SaveFeature(feature);
            return RedirectToRoute(new { controller = "Admin", action = "EditFeatures" });
        }

        public ActionResult DeleteFeature(int featureId)
        {

            featureRepo.DeleteFeature(featureId);
            return RedirectToRoute(new { controller = "Admin", action = "EditFeatures" });
        }



        // *************************************************************************************


        // helpers

        public List<Product> FilterProductList(
            List<Product> products,
            string category = null,
            string firm = null)
        {
            if (category != null && category != "Категория")
            {
                products = products.Where(p => p.Category == category).ToList();
            }
            if (firm != null && firm != "Производитель")
            {
                products = products.Where(p => p.Firm == firm).ToList();
            }
            return products;
        }

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