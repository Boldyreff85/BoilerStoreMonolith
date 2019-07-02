using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public ActionResult FirmList(string category, bool isAjax = false)
        {
            ViewBag.Category = category;
            ViewBag.IsAjax = isAjax;
            var res = productRepo.Products.Where(n => n.Category == category).Select(n => n.Firm).ToList().Distinct();
            if (res.FirstOrDefault() != "")
            {
                return PartialView("FirmList", res);
            }
            else
            {
                return PartialView("ErrorPage", "Производители не найдены");
            }
        }

        public ActionResult BoilerList(
            CatalogueViewModel model,
            string firm,
            string category,
            string linkName,
            int page = 1,
            bool isAjax = false,
            string filter = "default")
        {
            ViewBag.Category = category;
            ViewBag.Firm = firm;
            ViewBag.IsAjax = isAjax;
            ViewBag.Filter = filter;
            ViewBag.LinkName = linkName;

            var products = productRepo.Products;

            model.Categories = products.Select(n => n.Category).ToList().Distinct();
            model.Firms = products.Select(n => n.Firm).ToList().Distinct();

            // filter by power
            products = OrderProductList(products, linkName, filter);
            // filter by category and firm
            products = FilterProductList(products, category, firm);

            if (products.FirstOrDefault() != null)
            {
                model.ProductList = new ProductListViewModel
                {
                    Products = products
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = products.Count()
                    },
                };
                return PartialView("BoilerList", model);
            }
            else
            {
                return PartialView("ErrorPage", "Список товаров не найден");
            }


        }

        public ActionResult ProductPage(int productId)
        {
            return View(productRepo.Products.FirstOrDefault(p => p.ProductID == productId));
        }

        // выводим страницу каталога с 3 секциями (категории, производители и полный списко с пагинацией)
        public ActionResult Catalogue(
            CatalogueViewModel model, 
            string linkName, 
            int page = 1,
            string filter = "default",
            string category = null,
            string firm = null)
        {
            ViewBag.Filter = filter;
            ViewBag.LinkName = linkName;
            ViewBag.Category = category;
            ViewBag.Firm = firm;
            var products = productRepo.Products;

            model.Categories = products.Select(n => n.Category).ToList().Distinct();
            model.Firms = products.Select(n => n.Firm).ToList().Distinct();

            // order by power
            products = OrderProductList(products, linkName, filter);
            // filter by category and firm
            products = FilterProductList(products, category, firm);

            model.ProductList = new ProductListViewModel
            {
                Products = products
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

        public ActionResult Breadcrumb(string category = null, string firm = null, string title = null)
        {
            var model = new List<string> { category, firm, title };


            return PartialView("Breadcrumb", model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView("Footer", siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        [ChildActionOnly]
        public ActionResult CatalogueTree(CatalogueTreeViewModel model, string category, int page = 1)
        {
            var categories = productRepo.Products.Select(n => n.Category);
            var firms = productRepo.Products.Select(n => n.Firm);

            model.CurrCategory = category;

            foreach (var item in categories)
            {
                if (model.Categories.Where(n => n.Name == item).Count() > 0)
                {
                    continue;
                }

                CategoryModel categoryModel = new CategoryModel
                {
                    Name = item,
                    Firms = productRepo.Products.Where(n => n.Category == item).Select(n => n.Firm).Distinct()
                };

                model.Categories.Add(categoryModel);
            }

            return PartialView("CatalogueTree", model);
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

        public FileContentResult GetCategoryImage(int productId)
        {

            Product product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null && product.CategoryImageData != null && product.CategoryImageMimeType != null)
            {
                return File(product.CategoryImageData, product.CategoryImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public FileContentResult GetFirmImage(int productId)
        {

            Product product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null && product.FirmImageData != null && product.FirmImageMimeType != null)
            {
                return File(product.FirmImageData, product.FirmImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Product> OrderProductList(
            IEnumerable<Product> products, 
            string propertyName, 
            string value = "default")
        {
            if(value == "default" || propertyName == null)
            {
                return products.OrderBy(p => p.ProductID);
            }else if(value == "up")
            {
                return products.OrderBy(s => s.GetType().GetProperty(propertyName).GetValue(s, null));
            }
            else
            {
                return products.OrderByDescending(s => s.GetType().GetProperty(propertyName).GetValue(s, null));
            }

        }

        public IEnumerable<Product> FilterProductList(
            IEnumerable<Product> products, 
            string category = null,
            string firm = null)
        {
            if(category != null)
            {
                products = products.Where(p => p.Category == category);
            } 
            if(firm != null)
            {
                products = products.Where(p => p.Firm == firm);
            }
            return products;
        }

        private void SendMail(string toMail, string userName, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Joey Tribbiani", "vitalisun2@gmail.com"));
            message.To.Add(new MailboxAddress(userName, toMail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp.gmail.com", 465, true);
                string appSpecificPassword = "sliujqaomaazntyf";
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("vitalisun2@gmail.com", appSpecificPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }


    }
}