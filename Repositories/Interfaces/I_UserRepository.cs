using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_UserRepository : I_GenericRepository<User>
    {
        /// <summary>
        /// ดึงข้อมูลผู้ใช้ตามอีเมล
        /// </summary>
        /// <param name="email">อีเมลของผู้ใช้</param>
        /// <returns>ข้อมูลผู้ใช้หรือ null ถ้าไม่พบ</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// ดึงข้อมูลผู้ใช้ตาม ID
        /// </summary>
        /// <param name="userId">ID ของผู้ใช้</param>
        /// <returns>ข้อมูลผู้ใช้หรือ null ถ้าไม่พบ</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// ค้นหาผู้ใช้ตามบทบาท
        /// </summary>
        /// <param name="role">บทบาทของผู้ใช้</param>
        /// <returns>รายการผู้ใช้ที่มีบทบาทที่กำหนด</returns>
        Task<IEnumerable<User>> FindUsersByRoleAsync(string role);

        /// <summary>
        /// ค้นหาผู้ใช้ตามบทบาท พร้อมรองรับการแบ่งหน้า (Paging)
        /// </summary>
        /// <param name="role">บทบาทของผู้ใช้</param>
        /// <param name="pageIndex">หน้าที่ต้องการ</param>
        /// <param name="pageSize">จำนวนรายการต่อหน้า</param>
        /// <returns>รายการผู้ใช้ที่มีบทบาทที่กำหนด</returns>
        Task<IEnumerable<User>> FindUsersByRoleAsync(string role, int pageIndex, int pageSize);

        /// <summary>
        /// อัปเดตข้อมูลของผู้ใช้
        /// </summary>
        /// <param name="user">วัตถุ User ที่มีข้อมูลใหม่</param>
        /// <returns>ค่าบูลที่บอกว่าการอัปเดตสำเร็จหรือไม่</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// ลบผู้ใช้
        /// </summary>
        /// <param name="user">วัตถุ User ที่ต้องการลบ</param>
        /// <returns>ค่าบูลที่บอกว่าการลบสำเร็จหรือไม่</returns>
        Task<bool> DeleteUserAsync(User user);

        /// <summary>
        /// ค้นหาผู้ใช้ตามคำค้นหา (Search Term)
        /// </summary>
        /// <param name="searchTerm">คำค้นหาที่ต้องการ</param>
        /// <returns>รายการผู้ใช้ที่ตรงกับคำค้นหา</returns>
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

        /// <summary>
        /// ตรวจสอบว่าผู้ใช้อีเมลนี้มีอยู่ในระบบหรือไม่
        /// </summary>
        /// <param name="email">อีเมลที่ต้องการตรวจสอบ</param>
        /// <returns>ค่าบูลที่บอกว่าอีเมลนี้มีอยู่หรือไม่</returns>
        Task<bool> EmailExistsAsync(string email);

        
    }
}
