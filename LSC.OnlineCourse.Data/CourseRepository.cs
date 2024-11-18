using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Data
{
    public class CourseRepository : ICourseRepository
    {
        private readonly OnlineCourseDbContext _context;
      public CourseRepository(OnlineCourseDbContext dbContext) { 
         _context = dbContext;
        }

        public async Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {
           var query=_context.Courses.Include(x=>x.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query=query.Where(x=>x.CategoryId == categoryId.Value);
            }
            var courses=await query.Select(x => new CourseModel()
             {
                 CourseId = x.CourseId,
                 Title = x.Title,
                 Description = x.Description,
                 Price = x.Price,
                 CourseType = x.CourseType,
                 SeatsAvailable = x.SeatsAvailable,
                 Duration = x.Duration,
                 CategoryId = x.CategoryId,
                 InstructorId = x.InstructorId,
                 StartDate = x.StartDate,
                 EndDate = x.EndDate,
                 Category = new CourseCategoryModel()
                 {
                     CategoryId = x.Category.CategoryId,
                     CategoryName = x.Category.CategoryName,
                     Description = x.Category.Description
                 },
                 UserRating = new UserRatingModel
                 {
                     CourseId = x.CourseId,
                     AverageRating = x.Reviews.Any() ? Convert.ToDecimal(x.Reviews.Average(r => r.Rating)) : 0,
                     TotalRating = x.Reviews.Count(),
                 }
             }).ToListAsync();
            return courses;

        }

        public async Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
            var course =await _context.Courses
                .Include(x => x.Category)
                .Include(x => x.Reviews)
                .Include(x => x.SessionDetails)
                .Where(x => x.CourseId == courseId)
                .Select(x => new CourseDetailModel()
                {
                    CourseId = courseId,
                    Title = x.Title,
                    Description = x.Description,
                    Price = x.Price,
                    CourseType = x.CourseType,
                    SeatsAvailable = x.SeatsAvailable,
                    Duration = x.Duration,
                    CategoryId = x.CategoryId,
                    InstructorId = x.InstructorId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Category = new CourseCategoryModel()
                    {
                        CategoryId = x.Category.CategoryId,
                        CategoryName = x.Category.CategoryName,
                        Description = x.Category.Description
                    },
                    Reviews = x.Reviews.Select(x => new UserReviewModel
                    {
                        CourseId = x.CourseId,
                        UserName = x.User.DisplayName,
                        Rating = x.Rating,
                        Comments = x.Comments,
                        ReviewDate = x.ReviewDate,
                    }).OrderByDescending(o => o.Rating).Take(10).ToList(),
                    SessionDetails = x.SessionDetails.Select(x => new SessionDetailModel
                    {
                        SessionId = x.SessionId,
                        CourseId = x.CourseId,
                        Title = x.Title,
                        Description = x.Description,
                        VideoUrl = x.VideoUrl,
                        VideoOrder = x.VideoOrder,

                    }).OrderBy(o => o.VideoOrder).ToList(),
                    UserRating = new UserRatingModel
                    {
                        CourseId = x.CourseId,
                        AverageRating = x.Reviews.Any() ? Convert.ToDecimal(x.Reviews.Average(r => r.Rating)):0,
                        TotalRating=x.Reviews.Count(),
                    }
                }).FirstOrDefaultAsync();

            return course;
        }

        public async Task AddCourseAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
                .Include(c => c.SessionDetails)
                //.Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public void RemoveSessionDetail(SessionDetail sessionDetail)
        {
            _context.SessionDetails.Remove(sessionDetail);
        }

        public Task<List<Instructor>> GetAllInstructorsAsync()
        {
            return _context.Instructors.ToListAsync();
        }

        public async Task<bool> UpdateCourseThumbnail(string courseThumbnailUrl, int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.Thumbnail = courseThumbnailUrl;
            }

            return await _context.SaveChangesAsync() > 0;
        }


    }
}
