using ForMiraiProject.ViewModels;
using System;
using System.Threading.Tasks;


namespace ForMiraiProject.Services.Interfaces
{
    public interface I_UserService
    {
        /// <summary>
        /// Gets the profile of the user by their ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A Task containing the user's profile as a ViewModel or null if not found.</returns>
        Task<UserProfileViewModel?> GetUserProfileAsync(Guid userId);

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="model">The ViewModel containing the updated profile information.</param>
        /// <returns>A Task indicating success or failure.</returns>
        Task<bool> UpdateUserProfileAsync(Guid userId, UpdateProfileViewModel model);

        /// <summary>
        /// Changes the user's password securely.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="model">The ViewModel containing the old and new passwords.</param>
        /// <returns>A Task indicating success or failure.</returns>
        Task<bool> ChangeUserPasswordAsync(Guid userId, ChangePasswordViewModel model);

        /// <summary>
        /// Deletes the user's account by their ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A Task indicating success or failure.</returns>
        Task<bool> DeleteUserAccountAsync(Guid userId);
    }
}
