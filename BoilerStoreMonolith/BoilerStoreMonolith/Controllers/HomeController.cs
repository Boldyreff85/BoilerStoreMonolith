using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Concrete;
using BoilerStoreMonolith.Domain.Entities;
using BoilerStoreMonolith.Extensions;
using BoilerStoreMonolith.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BoilerStoreMonolith.Controllers
{
    public class HomeController : Controller
    {
        public int PageSize = 6;
        private IProductRepository productRepo;
        private IProductFeatureRepository featureRepo;
        private ICategoryFeatureRepository categoryFeatureRepo;
        private ICategoryRepository categoryRepo;
        private IFirmRepository firmRepo;
        private IInfoEntityRepository siteInfoRepo;
        private ApplicationContext context = new ApplicationContext();

        public HomeController(
            IProductRepository _productRepo,
            IProductFeatureRepository _featureRepo,
            ICategoryFeatureRepository _categoryFeatureRepo,
            IFirmRepository _firmRepo,
            IInfoEntityRepository _siteInfoRepo,
            ICategoryRepository _categoryRepo
            )
        {
            productRepo = _productRepo;
            featureRepo = _featureRepo;
            categoryFeatureRepo = _categoryFeatureRepo;
            firmRepo = _firmRepo;
            categoryRepo = _categoryRepo;
            siteInfoRepo = _siteInfoRepo;
        }

        public ActionResult Index()
        {
            var catList = categoryRepo.Categories.Distinct().ToList();
            return View(catList);
        }

        public ActionResult FirmList(string category, bool isAjax = false)
        {
            ViewBag.Category = category;
            ViewBag.IsAjax = isAjax;
            var firmNames = productRepo.Products
                .Where(n => n.Category == category)
                .Select(n => n.Firm).Distinct().ToList();

            var firms = firmNames.Join(firmRepo.Firms,
                p => p,
                t => t.Name,
                (p, t) => t).ToList();

            if (firms != null)
            {
                return PartialView("FirmList", firms);
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

            var products = productRepo.Products.ToList();

            model.Categories = products.Select(n => n.Category).ToList().Distinct();
            if (category != null)
            {
                model.Firms = products
                    .Where(f => f.Category == category)
                    .Select(n => n.Firm).ToList().Distinct();
            }
            else
            {
                model.Firms = products.Select(n => n.Firm).ToList().Distinct();
            }

            // тут получаем характеристики текущей категории
            var categories = categoryRepo.Categories.ToList();
            model.CategoryFeatures = new List<string>();
            if (categories.Any() && !string.IsNullOrEmpty(category))
            {
                var categoryId = categories
                    .Where(c => c.Name == category)
                    .Select(c => c.Id)
                    .Single();

                var catFeatureIds = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.FeatureId)
                    .ToList();
                model.CategoryFeatures = catFeatureIds.Join(featureRepo.Features,
                    p => p,
                    t => t.Id,
                    (p, t) => t.Name).ToList();
            }

            // feature ranges
            // получить минимальные и максимальные цены
            model.FeatureRanges = new List<FeatureRange>();
            model.FeatureRanges.Add(
                new FeatureRange
                {
                    FeatureName = "Цена",
                    From = products.Min(p => p.Price),
                    To = products.Max(p => p.Price)
                }
            );

            // filter by category and firm
            products = FilterProductList(products, category, firm);
            var features = featureRepo.Features.ToList();
            var productWithFeaturesList = new List<ProductWithFeatures>();

            if (products.Any())
            {
                foreach (var item in products)
                {
                    var prodWithFeatures = new ProductWithFeatures
                    {
                        Product = item,
                        ProductFeatures = features.Where(f => f.ProductId == item.ProductID).ToList()
                    };
                    productWithFeaturesList.Add(prodWithFeatures);
                }

                productWithFeaturesList = OrderProductWithFeaturesList(productWithFeaturesList, linkName, filter);

                model.ProductList = new ProductListViewModel
                {
                    ProductWithFeaturesList = productWithFeaturesList
                        .Skip((page - 1) * PageSize)
                        .Take(PageSize),
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = productWithFeaturesList.Count()
                    }
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
            var model = new ProductWithFeatures
            {
                Product = productRepo.Products.FirstOrDefault(p => p.ProductID == productId),
                ProductFeatures = featureRepo.Features.Where(f => f.ProductId == productId).ToList()
            };

            return View(model);
        }

        // выводим страницу каталога с 3 секциями (категории, производители и полный список с пагинацией)
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
            var products = productRepo.Products.ToList();

            model.Categories = products.Select(n => n.Category).ToList().Distinct();
            if (category != null)
            {
                model.Firms = products
                    .Where(f => f.Category == category)
                    .Select(n => n.Firm).ToList().Distinct();
            }
            else
            {
                model.Firms = products.Select(n => n.Firm).ToList().Distinct();
            }

            // тут получаем характеристики текущей категории
            var categories = categoryRepo.Categories.ToList();
            model.CategoryFeatures = new List<string>();
            if (categories.Any() && !string.IsNullOrEmpty(category))
            {
                var categoryId = categories
                    .Where(c => c.Name == category)
                    .Select(c => c.Id)
                    .Single();

                var catFeatureIds = categoryFeatureRepo.CategoryFeatures
                    .Where(cf => cf.CategoryId == categoryId)
                    .Select(cf => cf.FeatureId)
                    .ToList();
                model.CategoryFeatures = catFeatureIds.Join(featureRepo.Features,
                    p => p,
                    t => t.Id,
                    (p, t) => t.Name).ToList();
            }

            // feature ranges
            // получить минимальные и максимальные цены
            model.FeatureRanges = new List<FeatureRange>();
            model.FeatureRanges.Add(
                new FeatureRange
                {
                    FeatureName = "Цена",
                    From = products.Min(p => p.Price),
                    To = products.Max(p => p.Price)
                }
            );

            foreach (var item in model.CategoryFeatures)
            {

                var from = featureRepo.Features
                    .Where(f => f.Name == item)
                    .Min(f => f.Value);

                var to = featureRepo.Features
                    .Where(f => f.Name == item)
                    .Max(f => f.Value);

                var unit = featureRepo.Features
                    .Where(f => f.Name == item)
                    .Select(f => f.Unit)
                    .First();

                model.FeatureRanges.Add(
                new FeatureRange
                {
                    FeatureName = item,
                    From = from,
                    To = to,
                    Unit = unit
                });
            }

            // filter by category and firm
            products = FilterProductList(products, category, firm);
            var features = featureRepo.Features.ToList();
            var productWithFeaturesList = new List<ProductWithFeatures>();
            if (products.Any())
            {
                foreach (var item in products)
                {
                    var prodWithFeatures = new ProductWithFeatures
                    {
                        Product = item,
                        ProductFeatures = features.Where(f => f.ProductId == item.ProductID).ToList()
                    };
                    productWithFeaturesList.Add(prodWithFeatures);
                }
            }

            productWithFeaturesList = OrderProductWithFeaturesList(productWithFeaturesList, linkName, filter);

            model.ProductList = new ProductListViewModel
            {
                ProductWithFeaturesList = productWithFeaturesList
                .Skip((page - 1) * PageSize)
                .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = productWithFeaturesList.Count()
                }
            };
            return View(model);
        }

        public ActionResult Services()
        {
            return View(siteInfoRepo.InfoEntities.FirstOrDefault());
        }

        public ActionResult AboutCompany()
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
            var model = siteInfoRepo.InfoEntities.FirstOrDefault() ?? new InfoEntity();
            return PartialView("Header", model);
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
            var categories = productRepo.Products.Select(n => n.Category).ToList();

            model.CurrCategory = category;

            foreach (var item in categories)
            {
                if (model.Categories.Count(n => n.Name == item) > 0)
                {
                    continue;
                }

                var firmsInCategory = productRepo.Products
                    .Where(n => n.Category == item)
                    .Select(n => n.Firm)
                    .Distinct()
                    .ToList();

                CategoryModel categoryModel = new CategoryModel
                {
                    Name = item,
                    Firms = firmsInCategory
                };
                model.Categories.Add(categoryModel);
            }

            return PartialView("CatalogueTree", model);
        }

        [HttpPost]
        public ActionResult WriteUs(WriteUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var body = model.Message + "\n\n" +
                    "Обратная связь: \n" +
                     "email:    " + model.Email + "\n" +
                     "tel:  " + model.Phone;

                var client = "Заявка от клиента - " +
                    model.Name + " " + model.LastName;

                SendMail(model.Email, "админ", client, body);
                return RedirectToAction("WriteUsSuccess");
            }
            else
            {
                return RedirectToAction("WriteUsFail");
            }
        }

        public ActionResult WriteUsSuccess()
        {
            return View();
        }

        public ActionResult WriteUsFail()
        {
            return View();
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
            var category = context.Categories.FirstOrDefault(cat => cat.Name == product.Category);
            if (category != null && category.ImageData != null && category.ImageMimeType != null)
            {
                return File(category.ImageData, category.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public List<ProductWithFeatures> OrderProductWithFeaturesList(
            List<ProductWithFeatures> products,
            string propertyName,
            string value = "default")
        {
            if (value == "default" || string.IsNullOrEmpty(propertyName))
            {
                return products.OrderBy(p => p.Product.ProductID).ToList();
            }
            else if (value == "up")
            {
                if (propertyName == "Price")
                {
                    products = products
                        .OrderBy(p => p.Product.Price.ToFloat())
                        .ToList();
                }
                else
                {
                    products = products
                        .OrderBy(p => p.ProductFeatures
                            .Where(f => f.Name == propertyName)
                            .Select(f => f.Value.ToFloat())
                            .Single()
                        ).ToList();
                }
                return products;
            }
            else
            {
                if (propertyName == "Price")
                {
                    products = products
                        .OrderByDescending(p => p.Product.Price.ToFloat())
                        .ToList();
                }
                else
                {
                    products = products
                        .OrderByDescending(p => p.ProductFeatures
                            .Where(f => f.Name == propertyName)
                            .Select(f => f.Value.ToFloat())
                            .Single()
                        ).ToList();
                }
                return products;
            }
        }

        public List<Product> OrderProductList(
            List<Product> products,
            string propertyName,
            string value = "default")
        {
            if (value == "default" || propertyName == null)
            {
                return products.OrderBy(p => p.ProductID).ToList();
            }
            else if (value == "up")
            {
                foreach (var product in products)
                {
                    var featureName = product.GetType().GetProperty(propertyName);
                    if (featureName != null)
                        products = products
                            .OrderBy(s =>
                            s.GetType().GetProperty(propertyName).GetValue(s, null).ToString().ToFloat())
                            .ToList();
                }
                return products;
            }
            else
            {
                foreach (var product in products)
                {
                    var featureName = product.GetType().GetProperty(propertyName);
                    if (featureName != null)
                        products = products
                            .OrderByDescending(s =>
                            s.GetType().GetProperty(propertyName).GetValue(s, null).ToString().ToFloat())
                            .ToList();
                }
                return products;
            }
        }

        public List<Product> FilterProductList(
            List<Product> products,
            string category = null,
            string firm = null)
        {
            if (category != null)
            {
                products = products.Where(p => p.Category == category).ToList();
            }
            if (firm != null)
            {
                products = products.Where(p => p.Firm == firm).ToList();
            }
            return products;
        }

        private void SendMail(string toMail, string userName, string subject, string body)
        {
            // тестовые рабочие настройки для почты vitalisun2gmail.com
            //var email = "vitalisun2gmail.com";
            //var password = "sliujqaomaazntyf";
            //var host = "smtp.gmail.com";
            //var port = 465;
            //var doUseSsl = true;

            var infoEntity = siteInfoRepo.InfoEntities.FirstOrDefault();
            // настройки почты установленные в админке
            var email = infoEntity.Email;
            var password = infoEntity.Password;
            var host = infoEntity.Host;
            var port = infoEntity.Port;
            var doUseSsl = infoEntity.doUseSsl;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(subject, infoEntity.Email));
            message.To.Add(new MailboxAddress(userName, infoEntity.Email));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(host, port, doUseSsl);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(email, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }


    }
}