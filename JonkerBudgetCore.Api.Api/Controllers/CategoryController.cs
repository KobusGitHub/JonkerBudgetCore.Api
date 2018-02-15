using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JonkerBudgetCore.Api.Domain.Services.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JonkerBudgetCore.Api.Domain.Models.Categories;

namespace JonkerBudgetCore.Api.Api.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    [Produces("application/json")]
    [Route("api/Categories")]
    public class CustomersController : Controller
    {
        private ICategoryService categoryService;
        private IMapper mapper;

        public CustomersController(ICategoryService categoryService,
          IMapper mapper)
        {
            this.categoryService = categoryService;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await categoryService.GetAllCategories();
            return Ok(mapper.Map<IEnumerable<CategoryModel>>(categories));
        }




        [HttpGet]
        [Route("GetTestData")]
        public async Task<IActionResult> GetTestData()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<CategoryModel> categories = new List<CategoryModel>();
            categories.Add(new CategoryModel { Id = 1, Budget = 99.99, CategoryName = "TestCategoryName", GuidId = Guid.NewGuid() });

            if (categories == null)
            {
                return BadRequest();
            }

            return Ok(categories);
        }


        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody]CategoryCreateModel createCategoryDtoIn)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var model = mapper.Map<CategoryModel>(createCategoryDtoIn);
            var category = await categoryService.AddCategory(model);

            if (category == null)
            {
                return BadRequest();
            }

            return Ok(category);
        }

        [HttpPut]
        [Route("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory([FromBody]CategoryCreateModel createCategoryDtoIn)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var model = mapper.Map<CategoryModel>(createCategoryDtoIn);
            var category = await categoryService.UpdateCategory(model);

            return Ok(category);
        }


        [HttpPost]
        [Route("AddCategories")]
        public async Task<IActionResult> AddCategories([FromBody]List<CategoryCreateModel> createCategorytDtoInList)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var models = mapper.Map<IEnumerable<CategoryModel>>(createCategorytDtoInList);
            await categoryService.AddCategories(models.ToList());

            return Ok();
        }

    }

}