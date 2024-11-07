using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface ICourseService
    {
        Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null);
        Task<CourseDetailModel> GetCourseDetailAsync(int courseId);
    }
}
