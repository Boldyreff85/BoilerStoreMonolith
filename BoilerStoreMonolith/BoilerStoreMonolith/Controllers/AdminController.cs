using BoilerStoreMonolith.Domain.Abstract;
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
        private IInfoEntityRepository siteInfoRepo;
        public AdminController(IProductRepository rep1, IInfoEntityRepository rep2)
        {
            productRepo = rep1;
            siteInfoRepo = rep2;
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
            Console.WriteLine(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AdminEditViewModel model,
                HttpPostedFileBase productImg = null, HttpPostedFileBase categoryImg = null, HttpPostedFileBase firmImg = null)
        {
            Product product = model.Product;
            if (ModelState.IsValid)
            {
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
                return View(product);
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

        // helpers

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

    internal class method
    {
    }
}