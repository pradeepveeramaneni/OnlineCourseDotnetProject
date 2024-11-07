using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public  async Task<ActionResult<List<CourseModel>>> Get() { 
            var courses=await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpGet("Category/{categoryId}")]
        public async Task<ActionResult<List<CourseModel>>> GetAllCoursesByCategoryIDAsync([FromRoute] int categoryId)
        {
            var courses = await _courseService.GetAllCoursesAsync(categoryId);
            return Ok(courses);
        }

        [HttpGet("Detail/{courseId}")]
        public async Task<ActionResult<CourseDetailModel>> GetCourseDetailAsync(int courseId)
        {
            var courseDetail = await _courseService.GetCourseDetailAsync(courseId);
            if (courseDetail == null)
            {
                return NotFound();
            }
            return Ok(courseDetail);
        }
    }
}
