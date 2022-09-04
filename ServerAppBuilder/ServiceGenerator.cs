using DataConverter.Generators;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TeleReportsDataProvider.Data.DataConverter.Generators
{
    public class ServiceGenerator: ControllerGenerator
    {
        public Dictionary<string, string> Generate(IServiceCollection services)
        {
            Dictionary<string, string> scripts = new Dictionary<string, string>();
            var converter = new MyApplicationModelController();
            foreach (var service in services)
            {
              
                Type type = null;
                try
                {
                    type = ReflectionService.TypeForName(service.ServiceType.Name);
                }catch(Exception ex)
                {
                    ex.ToString().WriteToConsole();
                    continue;
                }
                if( type != null)
                {
                    if(type.Assembly.GetName() == Assembly.GetExecutingAssembly().GetName())
                    {
                        var model = MyApplicationModelController.CreateModel(type);
                        string script = AngularJsService(model);
                        scripts[type.Name] = script;
                    }
                }
                
            }
            return scripts;
        }
    }
}
