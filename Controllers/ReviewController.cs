using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace ForMiraiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly I_ReviewRepository _reviewRepository;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(I_ReviewRepository reviewRepository, ILogger<ReviewController> logger)
        {
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReviewAsync([FromBody] Review review)
        {
            if (review == null)
            {
                _logger.LogWarning("Review data is null.");
                return BadRequest("Review data cannot be null.");
            }

            try
            {
                review.Content = System.Net.WebUtility.HtmlEncode(review.Content);

                if (string.IsNullOrWhiteSpace(review.Content) || review.Rating < 1 || review.Rating > 5)
                {
                    _logger.LogWarning("Invalid review data.");
                    return BadRequest("Invalid review data.");
                }

                // ฟังก์ชันนี้ไม่มีค่าผลลัพธ์ จึงไม่ใช้ตัวแปรเพื่อเก็บ
                await _reviewRepository.CreateReviewAsync(review); // เรียกฟังก์ชันนี้โดยไม่เก็บผลลัพธ์

                return CreatedAtAction(nameof(GetReviewAsync), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Review ID is invalid.");
                return BadRequest("Invalid review ID.");
            }

            try
            {
                var review = await _reviewRepository.GetReviewsByHotelIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review not found.");
                    return NotFound();
                }

                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching review.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReviewAsync(Guid id, [FromBody] Review review)
        {
            if (review == null || id != review.Id)
            {
                _logger.LogWarning("Review ID mismatch or data is null.");
                return BadRequest("Invalid review data.");
            }

            try
            {
                // ฟังก์ชันนี้ไม่มีค่าผลลัพธ์ จึงไม่ใช้ตัวแปรเพื่อเก็บ
                await _reviewRepository.UpdateReviewAsync(review); // เรียกฟังก์ชันนี้โดยไม่เก็บผลลัพธ์

                return Ok(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid review ID.");
                return BadRequest("Invalid review ID.");
            }

            try
            {
                // ฟังก์ชันนี้ส่งผลลัพธ์เป็น void ดังนั้นไม่ต้องเก็บค่า
                await _reviewRepository.DeleteReviewAsync(id); // เรียกฟังก์ชันนี้โดยไม่เก็บผลลัพธ์

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
