using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService _courseCategoryService;
        public CourseCategoryController(ICourseCategoryService courseCategoryService)
        {
            _courseCategoryService = courseCategoryService;
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category= await _courseCategoryService.GetByIdAsync(id);
            if (category == null) { 
            return NotFound();
            }
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var catogories=await _courseCategoryService.GetCourseCategories();
            return Ok(catogories);
        }
    }
}
