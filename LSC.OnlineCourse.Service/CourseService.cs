using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
            
        }
        public Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {
            return _courseRepository.GetAllCoursesAsync(categoryId);    
        }

        public Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
            return _courseRepository.GetCourseDetailAsync(courseId);
        }
    }
}
