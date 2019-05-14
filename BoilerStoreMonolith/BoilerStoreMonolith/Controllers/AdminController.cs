﻿using BoilerStoreMonolith.Domain.Abstract;
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

        public ViewResult Index()
        {
            return View(productRepo.Products.ToList());
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
                TempData["VBCategory"] = string.Format("{0} has been saved", product.Title);
                return RedirectToAction("Index");
            }
            else
            {// there is something wrong with the data values
                return View(product);
            }
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }


        [HttpPost]
        public ActionResult Delete(int ProductID)
        {

            var deletedProduct = productRepo.DeleteProduct(ProductID);
            if (deletedProduct != null)
            {
                TempData["VBCategory"] = string.Format("{0} was deleted",
                deletedProduct.Title);
            }
            return RedirectToAction("Index");
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

                TempData["VBCategory"] = string.Format("Общая информация о сайте обновлена");
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
}