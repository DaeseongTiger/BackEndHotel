using System;
using System.Collections.Generic;

namespace ForMiraiProject.Models
{
    public class PagedResult<T>
    {
        /// <summary>
        /// รายการข้อมูลในหน้าปัจจุบัน
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// จำนวนรายการทั้งหมดในระบบ
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// หน้าปัจจุบัน (เริ่มจาก 0)
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// จำนวนรายการต่อหน้า
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// จำนวนหน้าทั้งหมด
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// มีหน้าก่อนหน้าหรือไม่
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// มีหน้าถัดไปหรือไม่
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        /// <summary>
        /// ค่าบูลที่บอกว่าเป็นหน้าแรกหรือไม่
        /// </summary>
        public bool IsFirstPage => PageIndex == 0;

        /// <summary>
        /// ค่าบูลที่บอกว่าเป็นหน้าสุดท้ายหรือไม่
        /// </summary>
        public bool IsLastPage => PageIndex + 1 >= TotalPages;

        /// <summary>
        /// สร้าง `PagedResult` สำหรับหน้าที่กำหนด
        /// </summary>
        /// <param name="items">รายการข้อมูลในหน้าปัจจุบัน</param>
        /// <param name="totalCount">จำนวนรายการทั้งหมด</param>
        /// <param name="pageIndex">หน้าปัจจุบัน</param>
        /// <param name="pageSize">จำนวนรายการต่อหน้า</param>
        /// <returns>ผลลัพธ์ที่จัดรูปแบบการแบ่งหน้า</returns>
        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
