namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// A class that holds strings containing sensitive information.
    /// Additional members can be added with more partials at compile time.
    /// </summary>
    public static partial class Secrets
    {
#if DEBUG
        /// <summary>
        /// The client ID used for OneDrive login.
        /// </summary>
        public static string ClientId { get; } = string.Empty;

        /// <summary>
        /// The tenant ID used for OneDrive login.
        /// </summary>
        public static string TenantId { get; } = string.Empty;

        /// <summary>
        /// The redirect URL used for OneDrive login.
        /// </summary>
        public static string RedirectUrl { get; } = string.Empty;
#endif
    }
}
