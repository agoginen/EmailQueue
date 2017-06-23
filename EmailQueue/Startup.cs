using EmailQueue.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmailQueue.Startup))]
namespace EmailQueue
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            dbConnection();
        }

        public void dbConnection()
        {
            DiversityTraxEntities db = new DiversityTraxEntities();
        }

    }
}
