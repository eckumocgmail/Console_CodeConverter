using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using DataConverter.Generators;

namespace ApplicationCore.Converter.ClientApp
{
    public class FrontendDeveloper
    {


        public static void CreateDataLevelApp()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, object> GenerateHttpServicesForControllers(string outputDir)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            HashSet<object> controllers = new HashSet<object>();
            foreach (Type type in AssemblyReader.GetControllers(Assembly.GetExecutingAssembly()))
            {
                List<object> actions = GetPublicMethods(type);
                string actionsString = toStrings(actions);
                controllers.Add(new
                {
                    name = type.Name,
                    type = "import { HttpClient } from '@angular/common/http';  " +
                        "class " + type.Name + "HttpService{ constructor( private http: HttpClient ){} \n\n" +
                        actionsString + "}"
                });
                System.IO.File.WriteAllText(
                    outputDir + type.Name.ToLower() + ".httpservice.ts",
                    "import { HttpClient } from '@angular/common/http'; \n" +
                    "import { Injectable } from '@angular/core';\n\n" +
                        "@Injectable({ providedIn: 'root' })" +
                        "export class " + type.Name + "HttpService{ \n\n constructor( private http: HttpClient ){} \n\n  " +
                        actionsString + "\n}");
            }
            map["controllers"] = controllers;
            return map;
        }


        private static string toStrings(List<object> actions)
        {
            string str = "";
            foreach (object action in actions)
            {
                str += action + ";\n  \n  ";
            }
            return str;
        }

        /// <summary>
        /// <button>ok</button>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<MethodInfo> GetOwnPublicMethods(Type type)
        {
            return (from m in new List<MethodInfo>(type.GetMethods())
                    where m.IsPublic &&
                            !m.IsStatic &&
                            m.DeclaringType.FullName == type.FullName
                    select m).ToList<MethodInfo>();
        }


        private static List<object> GetPublicMethods(Type type)
        {
            DbcontextGenerator gen = new DbcontextGenerator();
            List<object> actions = new List<object>();
            foreach (MethodInfo method in GetOwnPublicMethods(type))
            {
                string path = null;
                int lastIndex = (type.Name.IndexOf("Hub") != -1) ? type.Name.IndexOf("Hub") :
                                (type.Name.IndexOf("Controller") != -1) ? type.Name.IndexOf("Controller") : type.Name.Length;
                path = type.Name.Substring(0, lastIndex);
                Dictionary<string, object> parameters = GetMethodParameters(method);
                string methodDeclaration = gen.GetMethodDeclaration(method.Name, GetMethodParameters(method));

                Dictionary<string, string> pars = new Dictionary<string, string>();
                foreach (var p in GetMethodParameters(method))
                {
                    pars[p.Key] = p.Key;
                }
                actions.Add(methodDeclaration + "{ return this.http.get('" + path + "/" + method.Name + "',{params:" + gen.GetParameters(pars) + "});}");
                //       new
                //    {
                //name = method.Name,
                //returns= method.ReturnType.Name,
                //args = ReflectionService.GetMethodParameters(method),
                //func = method.Name+"("+ ReflectionService.GetMethodParametersString(method) + "){ return this.http.get('" + path + "/"+method.Name+"',{params:"+ ReflectionService.GetMethodParametersBlock(method) + "});}",
                //   });
            }
            return actions;
        }


        public static string GetMethodParametersBlock(MethodInfo method)
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


        public static string GetMethodParametersString(MethodInfo method)
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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMethodParameters(MethodInfo method)
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


    }
}
