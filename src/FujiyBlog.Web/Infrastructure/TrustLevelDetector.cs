using System.Web;

namespace FujiyBlog.Web.Infrastructure
{
    public class TrustLevelDetector
    {
        private static AspNetHostingPermissionLevel? currentTrustLevel;
        public static AspNetHostingPermissionLevel CurrentTrustLevel
        {
            get
            {
                if (!currentTrustLevel.HasValue)
                {
                    currentTrustLevel = GetCurrentTrustLevel();
                }
                return currentTrustLevel.Value;
            }
        }

        private static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach (AspNetHostingPermissionLevel trustLevel in new[]
                                                                    {
                                                                        AspNetHostingPermissionLevel.Unrestricted,
                                                                        AspNetHostingPermissionLevel.High,
                                                                        AspNetHostingPermissionLevel.Medium,
                                                                        AspNetHostingPermissionLevel.Low,
                                                                        AspNetHostingPermissionLevel.Minimal
                                                                    })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch (System.Security.SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }
    }
}