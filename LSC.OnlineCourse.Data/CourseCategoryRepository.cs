using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public class CourseCategoryRepository : ICourseCategoryRepository
    {
         private readonly OnlineCourseDbContext _context;
        public CourseCategoryRepository(OnlineCourseDbContext dbContext)
        {
            _context = dbContext;
        }
        public Task<CourseCategory?> GetById(int id)
        {
            var data =  _context.CourseCategories.FindAsync(id).AsTask();
            return data;
        }

        public Task<List<CourseCategory>> GetCourseCategoriesAsync()
        {
            var data = _context.CourseCategories.ToListAsync();
            return data;
        }
    }
}
