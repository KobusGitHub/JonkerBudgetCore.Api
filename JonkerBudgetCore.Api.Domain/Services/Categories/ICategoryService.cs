using JonkerBudgetCore.Api.Domain.Models.Categories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Categories
{
    public interface ICategoryService
    {

        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> UpdateCategory(CategoryModel categoryModel);
        Task<Category> AddCategory(CategoryModel categoryModel);
        Task AddCategories(List<CategoryModel> categoryModels);
    }
}
