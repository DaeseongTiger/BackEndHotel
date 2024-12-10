using System;
using System.Collections.Generic;

namespace ForMiraiProject.Utilities.PagedResult
{
    /// <summary>
    /// คลาสสำหรับจัดการผลลัพธ์แบบแบ่งหน้า (Paging)
    /// </summary>
    /// <typeparam name="T">ประเภทของข้อมูลที่ต้องการจัดการ</typeparam>
    public class PagedResultBase<T>
    {
        /// <summary>
        /// รายการของข้อมูลในหน้าปัจจุบัน
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// จำนวนข้อมูลทั้งหมด
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// ดัชนีหน้าปัจจุบัน
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// ขนาดของแต่ละหน้า
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// จำนวนหน้าทั้งหมด
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// มีหน้าถัดไปหรือไม่
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        /// <summary>
        /// มีหน้าก่อนหน้าหรือไม่
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// ตัวสร้างสำหรับ PagedResult
        /// </summary>
        public PagedResultBase() {}

        /// <summary>
        /// สร้างผลลัพธ์แบบแบ่งหน้า
        /// </summary>
        /// <param name="items">รายการของข้อมูล</param>
        /// <param name="totalCount">จำนวนข้อมูลทั้งหมด</param>
        /// <param name="pageIndex">ดัชนีหน้าปัจจุบัน</param>
        /// <param name="pageSize">ขนาดของแต่ละหน้า</param>
        /// <returns>ผลลัพธ์แบบแบ่งหน้า</returns>
        public static PagedResultBase<T> Create(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            // ปรับให้ `items` ไม่เป็น null
            items ??= new List<T>();

            // ตรวจสอบค่า pageSize และ pageIndex
            if (pageSize <= 0)
            {
                throw new ArgumentException("PageSize must be greater than zero.");
            }

            if (pageIndex < 0)
            {
                throw new ArgumentException("PageIndex cannot be negative.");
            }

            return new PagedResultBase<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
