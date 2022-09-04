using ApplicationCore.Converter.Models;


using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
 
 
    [Route("[controller]/[action]")]
    public class MyApplicationModelController: Controller
    {

        [HttpGet]
        [HttpOptions]
        public virtual IActionResult Index() => Json(CreateModels());


        /// <summary>
        /// Получение данных о контроллерах реализованных в данной сборке
        /// </summary>
        /// <returns> карта контроллеров </returns>
        [NonAction]
        public MyApplicationModel CreateModels()
        {
            var controllers = AssemblyReader.GetControllers(typeof(AuthorizationDataModel).Assembly);
            if (controllers == null || controllers.Count == 0)
            {
                throw new Exception("Контроллеры не найдены в приложении");
            }


            MyApplicationModel controllersMap = new MyApplicationModel();
            foreach (Type controllerType in controllers)
            {
                if (controllerType.IsAbstract) continue;
                var model = CreateModel(controllerType);
                controllersMap[controllerType.Name] = model;
            }

            WriteLine(controllersMap);
            return controllersMap;
        }

    private void WriteLine(MyApplicationModel controllersMap)
    {
        Console.WriteLine(controllersMap.ToJsonOnScreen());
    }

    public static MyControllerModel CreateModel(string type)
            => CreateModel(ReflectionService.TypeForShortName(type));

        [NonAction]
        public static MyControllerModel CreateModel(Type controllerType)
        {
            var uri = "/";
            var attrs = AttrsUtils.ForExtendedTypes(controllerType);
            if (attrs.ContainsKey("AreaAttribute")) uri += attrs["AreaAttribute"].ToString() + "/";
            if (attrs.ContainsKey("ForBusinessResourceAttribute")) uri += attrs["ForBusinessResourceAttribute"].ToString() + "/";
            string path = PathForController(controllerType);
          

            
            MyControllerModel model = new MyControllerModel()
            {
                Name = controllerType.Name.Replace("`1", ""),
                Path = "/"+ controllerType.Name,
                Actions = new Dictionary<string, MyActionModel>()
            };



          
            foreach (MethodInfo method in controllerType.GetMethods())
            {
                if (typeof(Controller).GetMethods().Select(m => m.Name).Contains(method.Name)) continue;
              

                    Dictionary<string, string> attributes = AttrsUtils.ForMethod(controllerType, method.Name);
                    Dictionary<string, object> pars = new Dictionary<string, object>();
                    model.Actions[method.Name] = new MyActionModel()
                    {
                        Name = method.Name,
                        Attributes = attributes,
                        Method = AttrsUtils.ParseHttpMethod(attributes),
                        Parameters = new Dictionary<string, MyParameterDeclarationModel>(),
                        Path = model.Path + "/" + method.Name
                    };
                    foreach (ParameterInfo par in method.GetParameters())
                    {
                        model.Actions[method.Name].Parameters[par.Name] = new MyParameterDeclarationModel()
                        {
                            Name = par.Name,
                            Type = par.ParameterType.Name,
                            IsOptional = par.IsOptional
                        };
                    }
              
            }
      
            return model;
        }
        /// <summary>
        /// Метод получения публичных методов типа
        /// </summary>
        /// <param name="type"> тип </param>
        /// <returns> открытые методы </returns>
        private string GetPublicMethods(Type type)
        {
            string actions = "";
            foreach (MethodInfo method in GetOwnPublicMethods(type))
            {
                string path = "/" + type.Name;
                actions += "\n\t" +
                    method.Name + "(" + GetMethodParametersString(method) + "){ \n\t\t\t" +
                        "return this.http.get('" + path + "/" + method.Name + "',{params:" +
                            GetMethodParametersBlock(method) + "});\n\t\t\t}\n";
                /* new {
                 name = method.Name,
                 returns= method.ReturnType.Name,
                 args = ReflectionService.GetMethodParameters(method),
                 func = method.Name+"("+ ReflectionService.GetMethodParametersString(method) + 
                     "){ return this.http.get('" + path + "/"+method.Name+"',{params:"+
                         ReflectionService.GetMethodParametersBlock(method) + "});}",

                });*/
            }
            return actions;
        }


        [NonAction]
        public string GetMethodParametersBlock(MethodInfo method)
        {
            string s = "{";
            bool needTrim = false;
            foreach (var pair in GetMethodParameters(method))
            {
                needTrim = true;
                s += pair.Key + ':' + pair.Key + ",";
            }
            if (needTrim == true)
                return s.Substring(0, s.Length - 1) + "}";
            else
            {
                return s + "}";
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetMethodParameters(MethodInfo method)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            foreach (ParameterInfo pinfo in method.GetParameters())
            {
                args[pinfo.Name] = new
                {
                    type = pinfo.ParameterType.Name,
                    optional = pinfo.IsOptional,
                    name = pinfo.Name
                };
            }
            return args;
        }


        public string GetMethodParametersString(MethodInfo method)
        {
            bool needTrim = false;
            string s = "";
            foreach (var p in GetMethodParameters(method))
            {
                needTrim = true;
                s += p.Key + ",";// +":"+ p.Value + ",";
            }
            return needTrim == true ? s.Substring(0, s.Length - 1) : s;
        }

        public static MyControllerModel CreateModelOf(object model)
        {
            return MyApplicationModelController.CreateModel(model.GetType());            
        }

        /// <summary>
        /// <button>ok</button>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetOwnPublicMethods(Type type)
        {
             var actions = (from m in new List<MethodInfo>(type.GetMethods())
                    where m.IsPublic &&
                            !m.IsStatic &&
                            m.DeclaringType.Name == type.Name
                    select m).ToList<MethodInfo>();
            return actions.Where(name => type.BaseType!=null&&type.BaseType.GetMethods().Select(m=>m.Name).ToList().Contains(name.Name) == false);
   
        }






        public static string BusinessResourceFor(Type type)
        {
            var attrs = AttrsUtils.GetEntityContrainsts(type);
            if (attrs.ContainsKey(nameof(AreaAttribute)))
            {
                return attrs[nameof(AreaAttribute)];
            }
            else
            {
                return null;
            }
        }



        private static string PathForController(Type controllerType)
        {
       
            var attrs = AttrsUtils.ForExtendedTypes(controllerType);
            string route =
                attrs.ContainsKey("RouteAttribute") ? attrs["RouteAttribute"] :
                attrs.ContainsKey("Route") ? attrs["Route"] :
                "";
            string path = route
                    .Replace("[controller]", controllerType.Name.Replace("Controller", ""))
                    .Replace("[action]", "Index");
            return path;
            string role = BusinessResourceFor(controllerType);
            /*if (role != null)
            {
                return "/" + role + "/" + controllerType.Name.Replace("Controller", "");
            }
            else
            {
                return "/" + controllerType.Name.Replace("Controller", "");
            }*/
        }
    }
 