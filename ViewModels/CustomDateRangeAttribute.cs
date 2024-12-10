using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    // Custom validation attribute for date range
    public class CustomDateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // ตรวจสอบว่า value ไม่เป็น null และเป็นชนิด DateTime
            if (value is DateTime dateValue)
            {
                // ถ้าวันที่อยู่ในอนาคต ให้ถือว่าไม่ถูกต้อง
                if (dateValue >= DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage ?? "วันที่ไม่ถูกต้อง.");
                }
            }
            else if (value != null) // ถ้า value ถูกส่งมาแต่ไม่ใช่ชนิด DateTime
            {
                return new ValidationResult("รูปแบบวันที่ไม่ถูกต้อง.");
            }

            // คืนค่าผลลัพธ์ที่ไม่เป็น null
            return ValidationResult.Success ?? new ValidationResult("Validation succeeded.");
        }
    }
}
