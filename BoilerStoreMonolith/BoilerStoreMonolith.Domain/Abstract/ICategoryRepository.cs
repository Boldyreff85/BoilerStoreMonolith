using System.Collections.Generic;
using BoilerStoreMonolith.Domain.Entities;

namespace BoilerStoreMonolith.Domain.Abstract
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> Categories { get; }
        void SaveCategory(Category category);
        Category DeleteCategory(int categoryId);
        List<Category> DeleteCategories(List<Category> categoriesToDelete);
    }
}