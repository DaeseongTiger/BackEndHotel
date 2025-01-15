using Microsoft.AspNetCore.Mvc;
using System.Net;
using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.Services;

namespace ForMiraiProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    // Constructor to inject ICouponService
    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await _couponService.GetAllCouponsAsync();
        return Ok(coupons);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCouponById(Guid id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        return coupon is not null ? Ok(coupon) : NotFound("Coupon not found.");
    }    

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCoupon(Guid id, [FromBody] Coupon updatedCoupon)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingCoupon = await _couponService.GetCouponByIdAsync(id);
        if (existingCoupon is null)
            return NotFound("Coupon not found.");

        await _couponService.UpdateCouponAsync(id, updatedCoupon);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCoupon(Guid id)
    {
        var existingCoupon = await _couponService.GetCouponByIdAsync(id);
        if (existingCoupon is null)
            return NotFound("Coupon not found.");

        await _couponService.DeleteCouponAsync(id);
        return NoContent();
    }

    [HttpPost("validate/{code}")]
    public async Task<IActionResult> ValidateCoupon(string code)
    {
        var isValid = await _couponService.ValidateCouponCodeAsync(code);
        return Ok(new { isValid });
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCoupons()
    {
        var activeCoupons = await _couponService.GetActiveCouponsAsync();
        return Ok(activeCoupons);
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredCoupons()
    {
        var expiredCoupons = await _couponService.GetExpiredCouponsAsync();
        return Ok(expiredCoupons);
    }
}
