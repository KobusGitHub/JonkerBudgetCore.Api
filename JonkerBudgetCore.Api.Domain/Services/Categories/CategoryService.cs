using JonkerBudgetCore.Api.Auth.Providers;
using JonkerBudgetCore.Api.Domain.Models.Categories;
using JonkerBudgetCore.Api.Domain.Services.Categories;
using JonkerBudgetCore.Api.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JonkerBudgetCore.Api.Domain.Services.Roles
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserInfoProvider userInfoProvider;

        public CategoryService(ApplicationDbContext dbContext,
             IUserInfoProvider userInfoProvider)
        {
            this.dbContext = dbContext;
            this.userInfoProvider = userInfoProvider;
        }
        

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await dbContext.Categories.ToListAsync();
        
        }

        public async Task<Category> UpdateCategory(CategoryModel categoryModel)
        {

            //model.Id = customerId;

            //if (!await updateCustomerPolicy.IsValid(model))
            //{
            //    throw new PolicyViolationException(updateCustomerPolicy.PolicyViolations);
            //}

            var customerToUpdate = await dbContext.Categories.FirstOrDefaultAsync(match => match.GuidId == categoryModel.GuidId);

            if (customerToUpdate != null)
            {
                customerToUpdate.Budget = categoryModel.Budget;
                customerToUpdate.CategoryName = categoryModel.CategoryName;
                customerToUpdate.LastModifiedDateUtc = DateTime.Now;

            }

            await dbContext.SaveChangesAsync();

            return customerToUpdate;
            
        }

        public async Task<Category> AddCategory(CategoryModel categoryModel)
        {

            //if (!await createCustomerPolicy.IsValid(model))
            //{
            //    throw new PolicyViolationException(createCustomerPolicy.PolicyViolations);
            //}

            var category = new Category()
            {
                Budget = categoryModel.Budget,
                CategoryName = categoryModel.CategoryName,
                GuidId = categoryModel.GuidId,
                CreatedDateUtc = DateTime.Now
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            return category;
                       
        }

        public async Task AddCategories(List<CategoryModel> categoryModels)
        {
            foreach (var categoryModel in categoryModels)
            {
                var category = await dbContext.Categories.FirstOrDefaultAsync(match => match.GuidId == categoryModel.GuidId);

               if (category != null)
                {
                    category.Budget = categoryModel.Budget;
                    category.CategoryName = categoryModel.CategoryName;
                    category.LastModifiedDateUtc = DateTime.Now;
                }
                else
                {
                    category = new Category()
                    {
                        Budget = categoryModel.Budget,
                        CategoryName = categoryModel.CategoryName,
                        GuidId = categoryModel.GuidId,
                        CreatedDateUtc = DateTime.Now
                    };
                    dbContext.Categories.Add(category);
                    
                }
            }
            await dbContext.SaveChangesAsync();
        }
    }
}
