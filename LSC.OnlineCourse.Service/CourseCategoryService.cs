using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public class CourseCategoryService:ICourseCategoryService
    {
        private readonly ICourseCategoryRepository _courseCategoryRepository;
        public CourseCategoryService(ICourseCategoryRepository categoryRepository) { 
            _courseCategoryRepository = categoryRepository;
        }

       public async Task<CourseCategoryModel?> GetByIdAsync(int id)
        {
            var data= await _courseCategoryRepository.GetById(id);
            return data==null? null:new CourseCategoryModel() { 
                CategoryId=data.CategoryId,
                CategoryName=data.CategoryName,
                Description=data.Description,
         
            };

        }

        public async Task<List<CourseCategoryModel>> GetCourseCategories()
        {
            var data=await _courseCategoryRepository.GetCourseCategoriesAsync();
            var modelData = data.Select(x => new CourseCategoryModel() 
            { 
            CategoryId = x.CategoryId,
            CategoryName=x.CategoryName,
            Description=x.Description,
            }).ToList();
            return modelData;

        }
    }
}
