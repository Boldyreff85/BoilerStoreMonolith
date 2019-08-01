﻿using BoilerStoreMonolith.Domain.Abstract;
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

            model.Category = categoryRepo.Categories
                .SingleOrDefault(c => c.Name == model.Product.Category);

            if (model.Category?.CategoryFeatures != null)
            {
                model.FeatureNames = model.Category.CategoryFeatures
                    .Select(cf => cf.Name).ToList();

                model.FeatureValues = model.Category.CategoryFeatures.Join(featureRepo.Features,
                    p => p.Name,
                    t => t.Name,
                    (p, t) => t.Value).ToList();
            }


            ViewBag.categories = new SelectList(
                    categoryRepo.Categories.Select(c => c.Name),
                    model.Product.Category
                );

            ViewBag.firms = new SelectList(
                firmRepo.Firms.Select(c => c.Name),
                model.Product.Firm
            );

            ViewBag.ImageToLoad = "categoryImg";
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditViewModel model,
                HttpPostedFileBase productImg = null,
                HttpPostedFileBase categoryImg = null,
                HttpPostedFileBase firmImg = null
                )
        {


            ViewBag.categories = new SelectList(
                categoryRepo.Categories.Select(c => c.Name),
                model.Product.Category
            );

            ViewBag.firms = new SelectList(
                firmRepo.Firms.Select(c => c.Name),
                model.Product.Firm
            );

            if (model.FeatureNames?.Count > 0 && model.FeatureValues?.Count > 0)
            {
                for (int i = 0; i < model.FeatureNames.Count; i++)
                {
                    var feature = new Feature
                    {
                        Name = model.FeatureNames[i],
                        Value = model.FeatureValues[i]
                    };
                    featureRepo.SaveFeature(feature);
                }

            }

            Product product = model.Product;

            productRepo.SaveProduct(product);
            TempData["category"] = string.Format("{0} has been saved", product.Title);

            var category = categoryRepo.Categories
                .SingleOrDefault(c => c.Name == product.Category);

            if (categoryImg != null)
            {
                category.ImageMimeType = categoryImg.ContentType;
                category.ImageData = new byte[categoryImg.ContentLength];
                categoryImg.InputStream.Read(
                    category.ImageData, 0, categoryImg.ContentLength);
            }
            categoryRepo.SaveCategory(category);

            var firm = firmRepo.Firms
                .SingleOrDefault(f => f.Name == product.Firm);
            if (firmImg != null)
            {
                firm.ImageMimeType = firmImg.ContentType;
                firm.ImageData = new byte[firmImg.ContentLength];
                firmImg.InputStream.Read(
                    firm.ImageData, 0, firmImg.ContentLength);
            }
            firmRepo.SaveFirm(firm);


            return RedirectToAction("Index");

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

            model.Features = model.Category.CategoryFeatures.Select(fn => fn.Name).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCategories(
            EditCategoriesViewModel model,
            HttpPostedFileBase categoryImg = null)
        {
            var catFeatures = new List<CategoryFeature>();

            foreach (var item in model.Features)
            {
                catFeatures.Add(new CategoryFeature
                {
                    Name = item
                });
            }

            var featuresToRemove = categoryFeatureRepo.CategoryFeatures.ToList();
            categoryFeatureRepo.DeleteCategoryFeatures(featuresToRemove);
            model.Category.CategoryFeatures = catFeatures;

            if (categoryImg != null)
            {
                model.Category.ImageMimeType = categoryImg.ContentType;
                model.Category.ImageData = new byte[categoryImg.ContentLength];
                categoryImg.InputStream.Read(
                    model.Category.ImageData, 0, categoryImg.ContentLength);
            }

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
        // ******** firms ********

        [HttpGet]
        public ActionResult IndexFirms()
        {
            ViewBag.ImageToLoad = "firmImg";

            var model = new IndexFirmsViewModel
            {
                Firms = firmRepo.Firms.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult IndexFirms(IndexFirmsViewModel model)
        {
            var names = model.firmName;
            var images = model.firmImg;
            var firmsFromRepo = firmRepo.Firms.ToList();
            // clear the table
            firmRepo.DeleteFirms(firmRepo.Firms.ToList());

            for (int i = 0; i < names.Count; i++)
            {
                // checking if firm exists

                var firm = new Firm
                {
                    Name = names[i]
                };

                if (images[i] != null)
                {
                    firm.ImageMimeType = images[i].ContentType;
                    firm.ImageData = new byte[images[i].ContentLength];
                    images[i].InputStream.Read(
                        firm.ImageData, 0, images[i].ContentLength);
                }
                else
                {
                    var firmFromRepo = firmsFromRepo.Find(f => f.Name == names[i]);
                    if (firmFromRepo != null)
                    {
                        firm.ImageMimeType = firmFromRepo.ImageMimeType;
                        firm.ImageData = firmFromRepo.ImageData;
                    }
                }
                firmRepo.SaveFirm(firm);
            }


            return RedirectToAction("IndexFirms");
        }


        public ActionResult GetFirmListItem(string imgId)
        {
            return PartialView("FirmListItem", imgId);
        }

        // *************************************************************************************


        // helpers
        public FileContentResult GetImageFromCategoryTable(int categoryId)
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