using ApplicationCore.Converter.Models;
using DataConverter.Generators;
using System;
using System.Collections.Generic;
using TeleReportsDataProvider.Data.DataConverter.TypeScriptModel;

namespace TeleReportsDataProvider.Data.DataConverter.Converters
{
    public class ModelConverter: PropertyConverter
    {

        public TypeScriptClassModel ToTypeScriptModel( Type type )
        {
            var model = new TypeScriptClassModel();
            model.Constructor = new global::TypeScriptConstructorModel()
            {
                Parameters = new Dictionary<string, MyParameterDeclarationModel>() {
                    { "hashcode", new MyParameterDeclarationModel(){
                        Name="hashcode",
                        IsOptional = false,
                        Type = "number"
                    } },
                    { "json", new MyParameterDeclarationModel(){
                        Name="json",
                        IsOptional = true,
                        Type = "string"
                    } },
                }
            };
            model.Constructor.Implementation += "\t\tthis.hashcode = hashcode;\n";
            model.Constructor.Implementation += "\t\tif(json)Object.assign(this,JSON.parse(json));\n";
            foreach (var property in type.GetProperties())
            {
                TypeScriptPropertyModel tsprop = ToTypeScriptModel(property);
                model.AddProperty(tsprop);
            }
            foreach (var method in type.GetMethods())
            {
                if (method.Name.StartsWith("remove_") == true || method.Name.StartsWith("add_") == true || method.Name.StartsWith("get_") == true || method.Name.StartsWith("set_") == true)
                    continue;
                TypeScriptActionModel tsaction = ToTypeScriptModel(method);
                model.AddMethod(tsaction);
            }
            return model;
        }



        public TypeScriptActionModel ToTypeScriptModel(System.Reflection.MethodInfo method)
        {
            TypeScriptActionModel action = new TypeScriptActionModel();
            action.Name = Naming.ToCamelStyle(method.Name);
            string tsParamMap = "\t\tconst pars = {\n";
            
            
            foreach (var parameter in method.GetParameters())
            {
                var par = new MyParameterDeclarationModel();
                par.Type = GetTypeScriptDataType(parameter.ParameterType);
                par.Name = Naming.ToCamelStyle(parameter.Name);
                par.IsOptional = par.IsOptional;
                action.Parameters[par.Name] = par;

                tsParamMap += $"\t\t\t{Naming.ToCamelStyle(parameter.Name)}: {Naming.ToCamelStyle(parameter.Name)},\n";
            }
            if (tsParamMap.EndsWith(",\n"))
            {
                tsParamMap = tsParamMap.Substring(0, tsParamMap.Length - 2);
            }
            tsParamMap += "\n\t\t}\n";
            action.Implementation = tsParamMap;
            action.Implementation += "\t\t" + "return window['$app'].$invoke.$method(''+this.hashcode, '" + method.Name + "',{Name: '@nav.Name'});\n";
            return action;
        }



        public TypeScriptPropertyModel ToTypeScriptModel(System.Reflection.PropertyInfo property)
        {
            return new TypeScriptPropertyModel()
            {
                Name = Naming.ToCamelStyle(property.Name),
                Type = GetTypeScriptDataType(property.PropertyType),
                IsOptional = false
                
            };
        }
    }
}
