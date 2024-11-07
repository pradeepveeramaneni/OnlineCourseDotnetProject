using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface ICourseCategoryService
    {
        Task<CourseCategoryModel?> GetByIdAsync(int id);
        Task<List<CourseCategoryModel>> GetCourseCategories();

    }
}
