using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Concrete;
using Ninject.Modules;

namespace BoilerStoreMonolith.Infrastructure
{
    public class NinjectRegistrations : NinjectModule
    {
        public override void Load()
        {
            Bind<IProductRepository>().To<ProductRepository>();
            Bind<IInfoEntityRepository>().To<InfoEntityRepository>();
            Bind<ICategoryRepository>().To<CategoryRepository>();
            Bind<IFirmRepository>().To<FirmRepository>();
            Bind<IProductFeatureRepository>().To<ProductFeatureRepository>();
            Bind<ICategoryFeatureRepository>().To<CategoryFeatureRepository>();
        }
    }
}