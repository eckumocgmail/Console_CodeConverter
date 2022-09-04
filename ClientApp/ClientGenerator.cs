using ApplicationCore.Converter.Models;

using DataConverter.Generators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ApplicationCore.Converter.Models { }
namespace CoreCommon.DataConverter.Generators.JavaScript
{
    public class ClientGenerator
    {
        public static string Create(string typeofController)
        {
            return Create(ReflectionService.TypeForName(typeofController));
        }
            
        public static string Create(Type typeofController)
        {
            var controllerModel = MyApplicationModelController.CreateModel(typeofController);
            string typeScript = "function " + Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "( pelement ){";
            typeScript += "\n\n\t\t\tconsole.log(pelement.id,'" + Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "');\n\n" +
            "\n\n\t\t\tfunction toHttpParams(obj)" +
            "\n\t\t\t{ " +
            "\n\t\t\t     let result = { }; " +
            "\n\t\t\t     Object.getOwnPropertyNames(obj).forEach(name => { " +
            "\n\t\t\t         result[name] = window['convertToHttpMessageParam'](obj[name]); " +
            "\n\t\t\t     }); " +
            "\n\t\t\t     return result; " +
            "\n\t\t\t}           \n\n\n";
            foreach (var actionKV in controllerModel.Actions)
            {
                MyActionModel actionModel = actionKV.Value;
                string tsName = Naming.ToCamelStyle(actionModel.Name);
                string tsParamDeclaration = "";
                string tsParamMap = "{\n";
                foreach (var key in actionModel.Parameters.Keys)
                {
                    tsParamDeclaration += key + ",";
                    tsParamMap += $"\t\t\t\t{key}: {key},\n";
                }
                if (tsParamMap.EndsWith(",\n"))
                {
                    tsParamMap = tsParamMap.Substring(0, tsParamMap.Length - 2);
                }
                tsParamMap += "\n\t\t\t\t} ";
                if (tsParamDeclaration.EndsWith(","))
                {
                    tsParamDeclaration = tsParamDeclaration.Substring(0, tsParamDeclaration.Length - 1);
                }
                typeScript += $"\t\t\tthis.{tsName}=function( {tsParamDeclaration} )" + "{\n";
                typeScript += $"\t\t\t\tlet pars = toHttpParams({tsParamMap});\n";
                typeScript += $"\t\t\t\tconsole.log('{tsName}', pars);\n ";
                typeScript += "\t\t\t\treturn window['https']({ url: '" + actionModel.Path + "', type: '" + Naming.ToCapitalStyle(controllerModel.Name).Replace("Controller", "Factory") + "', params: pars, headers: { 'Content-Type': 'text/json', Scope: pelement.id, Model: pelement?pelement.id.replace('view-',''): '' }, method: '" + actionModel.Method + "',}).then((r)=>{ window['checkout'](); return r; });\n";
                typeScript += "\t\t\t}\n\n\n";
            }
            typeScript += "}\n\n\n";
            return typeScript;
        }
    }
}
