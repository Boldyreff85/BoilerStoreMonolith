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
    public class HomeController : Controller
    {
        public int PageSize = 6;
        private IProductRepository productRepo;
        private IInfoEntityRepository siteInfoRepo;

        public HomeController(IProductRepository rep1, IInfoEntityRepository rep2)
        {
            productRepo = rep1;
            siteInfoRepo = rep2;
        }

        public ActionResult Index()
        {
            var catList = productRepo.Products.Select(n => n.Category).ToList().Distinct();
            var siteInfo = siteInfoRepo.InfoEntities.FirstOrDefault();
            IndexViewModel result = new IndexViewModel
            {
                categories = catList,
                infoEntity = siteInfo
            };
            return View(result);
        }

        public ActionResult FirmList(string category)
        {
            var res = productRepo.Products.Where(n => n.Category == category).Select(n => n.Firm).ToList().Distinct();
            return PartialView("FirmList", res);
        }

        public ActionResult BoilerList(string firm, int page = 1)
        {

            var products = productRepo.Products.Where(n => n.Firm == firm);

            ProductListViewModel model = new ProductListViewModel
            {
                Products = products
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = products.Count()
                }
            };
            return PartialView("BoilerList", model);
        }

        public ActionResult ProductPage(int productId)
        {
            return View(productRepo.Products.FirstOrDefault(p => p.ProductID == productId));
        }

        // выводим страницу каталога с 3 секциями (категории, производители и полный списко с пагинацией)
        public ActionResult Catalogue(CatalogueModelView model, int page = 1)
        {
            var products = productRepo.Products;
            model.Categories = products.Select(n => n.Category).ToList().Distinct();
            model.Firms = products.Select(n => n.Firm).ToList().Distinct();

            model.ProductList = new ProductListViewModel
            {
                Products = products
                    .OrderBy(p => p.ProductID)
                    .Skip((page - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = products.Count()
                }
            };

            return View(model);
        }

        public ActionResult Services()
        {
            return View(siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        public ActionResult Contacts()
        {
            return View(siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        // child actions

        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView("Header", siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView("Footer", siteInfoRepo.InfoEntities.FirstOrDefault());
        }


        // helpers

        public FileContentResult GetImage(int productId)
        {

            Product product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null && product.ImageData != null && product.ImageMimeType != null)
            {
                return File(product.ImageData, product.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}