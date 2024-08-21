using Hangfire.Dashboard;

namespace DegreeVerify.Client
{
    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Authorize users (you can customize this logic)
            return true;
        }
    }
}