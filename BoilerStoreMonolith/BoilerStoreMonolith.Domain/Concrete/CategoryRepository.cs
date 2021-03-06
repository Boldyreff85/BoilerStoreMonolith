﻿using BoilerStoreMonolith.Domain.Abstract;
using BoilerStoreMonolith.Domain.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BoilerStoreMonolith.Domain.Concrete
{
    public class CategoryRepository : ICategoryRepository
    {
        private ApplicationContext context = new ApplicationContext();

        public IEnumerable<Category> Categories
        {
            get { return context.Categories; }
        }

        public void SaveCategory(Category category)
        {
            if (category.Id == 0)
            {
                context.Categories.Add(category);
            }
            else
            {
                Category dbEntry = context.Categories.Find(category.Id);
                if (dbEntry != null)
                {
                    dbEntry.Id = category.Id;
                    dbEntry.Name = category.Name;
                    dbEntry.ImageData = category.ImageData;
                    dbEntry.ImageMimeType = category.ImageMimeType;

                }
            }
            context.SaveChanges();
        }

        public Category DeleteCategory(int categoryId)
        {
            Category dbEntry = context.Categories.Find(categoryId);
            if (dbEntry != null)
            {
                context.Categories.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public List<Category> DeleteCategories(List<Category> categoriesToDelete)
        {
            if (categoriesToDelete.Any())
            {
                context.Categories.RemoveRange(categoriesToDelete);
                context.SaveChanges();
            }
            return categoriesToDelete;
        }

    }
}
