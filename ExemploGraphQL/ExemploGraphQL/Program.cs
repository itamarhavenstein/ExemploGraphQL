using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace ExemploGraphQL
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri _baseAddress = new Uri("http://localhost:60065/");

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(_baseAddress);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            var bootstrapper = new Bootstrapper();
            config.DependencyResolver = bootstrapper.Resolver();

            //Criação do servidor
            var server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();
            Console.WriteLine("Web API Self hosted on " + _baseAddress + "Hit ENTER to exit... ");
            Console.ReadLine();
            server.CloseAsync().Wait();

        }
    }
}
