using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ContactBook.Startup))]
namespace ContactBook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
