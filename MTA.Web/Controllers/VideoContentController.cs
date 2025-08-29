using Microsoft.AspNetCore.Mvc;
using MTA.Domain.Entities;
using MTA.Domain.Interfaces;

namespace MTA.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoContentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public VideoContentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/videocontent/random?count=5
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomVideos(int count = 5)
        {
            var allVideos = await _unitOfWork.Repository<VideoContent>().GetAllAsync();
            if (!allVideos.Any()) return NotFound("No video contents available.");

            var randomVideos = allVideos.OrderBy(x => Guid.NewGuid()).Take(count).ToList();
            return Ok(randomVideos);
        }

        // GET: api/videocontent
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var videos = await _unitOfWork.Repository<VideoContent>().GetAllAsync();
            return Ok(videos);
        }

        // GET: api/videocontent/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var video = await _unitOfWork.Repository<VideoContent>().GetByIdAsync(id);
            if (video == null) return NotFound();
            return Ok(video);
        }

        // POST: api/videocontent
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VideoContent video)
        {
            await _unitOfWork.Repository<VideoContent>().AddAsync(video);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = video.Id }, video);
        }

        // PUT: api/videocontent/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] VideoContent video)
        {
            if (id != video.Id) return BadRequest();
            _unitOfWork.Repository<VideoContent>().UpdateAsync(video);
            await _unitOfWork.SaveChangesAsync();
            return Ok(video);
        }

        // DELETE: api/videocontent/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var repo = _unitOfWork.Repository<VideoContent>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            repo.DeleteAsync(entity.Id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
