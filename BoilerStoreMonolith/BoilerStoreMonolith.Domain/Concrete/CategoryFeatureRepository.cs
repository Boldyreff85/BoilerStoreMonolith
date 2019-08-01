using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class CategoryFeatureRepository : ICategoryFeatureRepository
    {
        private ApplicationContext context = new ApplicationContext();


        public IEnumerable<CategoryFeature> CategoryFeatures => context.CategoryFeatures;


        public async void SaveCategoryFeature(CategoryFeature categoryFeature)
        {
            using (var context = new ApplicationContext())
            {
                if (categoryFeature.Id == 0)
                {
                    context.CategoryFeatures.Add(categoryFeature);
                }
                else
                {
                    CategoryFeature dbEntry = context.CategoryFeatures.Find(categoryFeature.Id);
                    if (dbEntry != null)
                    {
                        dbEntry.Id = categoryFeature.Id;
                        dbEntry.Name = categoryFeature.Name;
                    }
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task<CategoryFeature> DeleteCategoryFeature(int categoryFeatureId)
        {
            using (var context = new ApplicationContext())
            {

                CategoryFeature dbEntry = await context.CategoryFeatures.FindAsync(categoryFeatureId);
                if (dbEntry != null)
                {
                    context.CategoryFeatures.Remove(dbEntry);
                    await context.SaveChangesAsync();
                }
                return dbEntry;
            }

        }

        public List<CategoryFeature> DeleteCategoryFeatures(List<CategoryFeature> categoryFeaturesToDelete)
        {
            context.CategoryFeatures.RemoveRange(categoryFeaturesToDelete);
            context.SaveChanges();

            return categoryFeaturesToDelete;
        }

    }
}
