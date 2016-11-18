using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SignalRMvc5.Startup))]

namespace SignalRMvc5
{
    /// <summary>
    /// Чтобы задействовать фукциональность SignalR, добавим в проект следующий класс с названием Startup:
    /// </summary>
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
