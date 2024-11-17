using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;
using LSC.OnlineCourse.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public class VideoRequestService: IVideoRequestService
    {
        private readonly ILogger<VideoRequestService> logger;
        private readonly IMapper _mapper;
        private readonly IVideoRequestRepository repository;
        private readonly IConfiguration configuration;

        public VideoRequestService(IVideoRequestRepository repository, IMapper mapper, ILogger<VideoRequestService> logger, IConfiguration configuration) {
            this.repository = repository;
            this._mapper = mapper;
            this.logger = logger;
            this.configuration = configuration;
            
        }

        public async Task<VideoRequestModel> CreateAsync(VideoRequestModel model)
        {
            var videoRequest=_mapper.Map<VideoRequest>(model);
            var createdVideoRequest = await repository.AddAsync(videoRequest);
            return _mapper.Map<VideoRequestModel>(createdVideoRequest);
            
        }

        public async Task DeleteAsync(int id)
        {
           await repository.DeleteAsync(id);
         }

        public async Task<List<VideoRequestModel>> GetAllAsync()
        {
            var videoRequests=await repository.GetAllAsync();
            return _mapper.Map<List<VideoRequestModel>>(videoRequests);
        }

        public async Task<VideoRequestModel?> GetByIdAsync(int id)
        {
            var videoRequests = await repository.GetByIdAsync(id);
            return videoRequests==null ? null : _mapper.Map<VideoRequestModel>(videoRequests);
        }

        public async Task<IEnumerable<VideoRequestModel>> GetByUserIdAsync(int userId)
        {
            var videoRequests = await repository.GetAllAsync();
            return _mapper.Map<IEnumerable<VideoRequestModel>>(videoRequests);

        }

        public async Task<VideoRequestModel> UpdateAsync(int id, VideoRequestModel model)
        {

                var existingVideoRequest= await repository.GetByIdAsync(id);

            if (existingVideoRequest == null)
            {
                throw new KeyNotFoundException();
            }

            model.UserId= existingVideoRequest.UserId;
            _mapper.Map(model,existingVideoRequest);
            var UpadatedvideoRequest = await repository.UpdateAsync(existingVideoRequest);

            return _mapper.Map<VideoRequestModel>(UpadatedvideoRequest);

        }
    }
}
