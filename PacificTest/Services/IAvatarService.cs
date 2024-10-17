namespace PacificTest.Services
{
    public interface IAvatarService
    {
        string GetDefaultAvatar();

        /// <summary>
        /// Returns user avatar URL based on the user identifier.
        /// </summary>
        Task<string> GetAvatar(string? UserIdentifier);
    }
}