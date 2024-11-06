using LSC.OnlineCourse.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LSC.OnlineCourse.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        

        private readonly OnlineCourseDbContext onlineCourseDbContext1;


        public WeatherForecastController(OnlineCourseDbContext onlineCourseDbContext)
        {
            this.onlineCourseDbContext1=onlineCourseDbContext;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            var courses = onlineCourseDbContext1.Courses.ToList();
            return Ok(courses);
        }
    }
}
