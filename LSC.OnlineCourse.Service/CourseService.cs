﻿using AutoMapper;
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
        private readonly IMapper mapper;

        public CourseService(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            this.mapper = mapper;


        }
        public Task<List<CourseModel>> GetAllCoursesAsync(int? categoryId = null)
        {
            return _courseRepository.GetAllCoursesAsync(categoryId);
        }

        public Task<CourseDetailModel> GetCourseDetailAsync(int courseId)
        {
            return _courseRepository.GetCourseDetailAsync(courseId);
        }
        public async Task AddCourseAsync(CourseDetailModel model)
        {
            // Map the CourseModel to the Course entity
            var courseEntity = new Course
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CourseType = model.CourseType,
                SeatsAvailable = model.SeatsAvailable,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                InstructorId = model.InstructorId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                SessionDetails = model.SessionDetails.Select(session => new SessionDetail
                {
                    Title = session.Title,
                    Description = session.Description,
                    VideoUrl = session.VideoUrl,
                    VideoOrder = session.VideoOrder
                }).ToList()
            };

            await _courseRepository.AddCourseAsync(courseEntity);
        }

        public async Task UpdateCourseAsync(CourseDetailModel courseModel)
        {
            var course = await _courseRepository.GetCourseByIdAsync(courseModel.CourseId);
            if (course == null)
            {
                throw new Exception("Course not found");
            }

            // Update course fields
            course.Title = courseModel.Title;
            course.Description = courseModel.Description;
            course.Price = courseModel.Price;
            course.CourseType = courseModel.CourseType;
            course.SeatsAvailable = courseModel.SeatsAvailable;
            course.Duration = courseModel.Duration;
            course.CategoryId = courseModel.CategoryId;
            course.InstructorId = courseModel.InstructorId;
            course.StartDate = courseModel.StartDate;
            course.EndDate = courseModel.EndDate;

            var existingSessionIds = course.SessionDetails.Select(s => s.SessionId).ToList();
            var updatedSessionIds = courseModel.SessionDetails.Select(s => s.SessionId).ToList();
            // Remove sessions that are not in the updated list
            var sessionsToRemove = course.SessionDetails.Where(s => !updatedSessionIds.Contains(s.SessionId)).ToList();
            foreach (var session in sessionsToRemove)
            {
                course.SessionDetails.Remove(session);
                _courseRepository.RemoveSessionDetail(session); // This removes the session from the database
            }

            // Update or add session details
            foreach (var sessionModel in courseModel.SessionDetails)
            {
                var existingSession = course.SessionDetails.FirstOrDefault(s => s.SessionId == sessionModel.SessionId);
                if (existingSession != null)
                {
                    // Update existing session details
                    existingSession.Title = sessionModel.Title;
                    existingSession.Description = sessionModel.Description;
                    existingSession.VideoUrl = sessionModel.VideoUrl;
                    existingSession.VideoOrder = sessionModel.VideoOrder;
                }
                else
                {
                    // Add new session details
                    var newSession = new SessionDetail
                    {
                        Title = sessionModel.Title,
                        Description = sessionModel.Description,
                        VideoUrl = sessionModel.VideoUrl,
                        VideoOrder = sessionModel.VideoOrder,
                        CourseId = course.CourseId
                    };
                    course.SessionDetails.Add(newSession);
                }
            }

            // Call repository to update the course along with its session details
            await _courseRepository.UpdateCourseAsync(course);
        }

        public async Task DeleteCourseAsync(int courseId)
        {
            await _courseRepository.DeleteCourseAsync(courseId);
        }

        public async Task<List<InstructorModel>> GetAllInstructorsAsync()
        {
            var instructors = await _courseRepository.GetAllInstructorsAsync();
            return mapper.Map<List<InstructorModel>>(instructors);
        }

        public Task<bool> UpdateCourseThumbnail(string courseThumbnailUrl, int courseId)
        {
            return _courseRepository.UpdateCourseThumbnail(courseThumbnailUrl, courseId);
        }


    }
}
