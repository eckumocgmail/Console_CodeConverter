using System.Collections.Generic;
using System.Reflection;

namespace DataConverter.Generators
{
    public class DbcontextGenerator
    {
       
        /// <summary>
        /// Генерация скриптов для управления контекстом данных
        /// </summary>
        /// <param name="metadata"> модель данных </param>
        /// <returns> код для вывода в файлы </returns>
        public Dictionary<string, string> createDataContext( IDatabaseMetadata metadata )
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            string _serviceClassName = "DbContextService";
            string imports = "import { Component,Injectable } from '@angular/core';\n";
            string dbcontextCode = "\n@Injectable({providedIn: 'root'})\nexport class "+ _serviceClassName + "\n{\n" ;
            dbcontextCode += "\t constructor( \n";
            ModelGenerator generator = new ModelGenerator();
            foreach( IDataTable table in metadata.GetTables().Values)
            {   
                imports += "import { " + table.GetTableNameCapitalized() + "Service } from './" + table.GetTableNameKebabed() + ".service';\n";
                dbcontextCode += "\t\t public "+ table.getTableNameCamelized() +": "+ table.GetTableNameCapitalized() + "Service,\n";


                string wscode = "import { Component,Inject,Injectable } from '@angular/core';\n" +
                                "import { HttpClient, HttpParams } from '@angular/common/http';\n" +
                                "import { " + table.GetTableNameCapitalized() + " } from './" + table.GetTableNameKebabed() + ".model';\n";
                wscode += "\n\n@Injectable({providedIn: 'root'})\nexport class " + table.GetTableNameCapitalized() + "Service \n{\n" +
                    "\t\t constructor(  private http: HttpClient ){} \n" +
                    
                    "\t\t toHttpParams(obj: any){\n" +
                    "\t\t    const result = new HttpParams();\n" +
                    "\t\t    Object.getOwnPropertyNames(obj).forEach(name => {\n" +
                    "\t\t        result.set(name, JSON.stringify(obj[name]));\n" +
                    "\t\t    });\n" +
                    "\t\t    return result;\n" +
                    "\t\t }\n" +
                "\t\t list( resolve: ( data: " + table.GetTableNameCapitalized() + "[])=>any, reject? :(err)=>any ){ \n\t\tthis.http.get<"+table.GetTableNameCapitalized()+"[]>('api/" + table.GetTableNameCapitalized() + "').subscribe(resolve,reject);\n\t };\n" +
                    "\t\t find( id: number, resolve: (data: "+ table.GetTableNameCapitalized() + ")=>any, reject? :(err)=>any ){ \n\t\tthis.http.get('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ id: id })}).subscribe(resolve,reject);\n\t };\n" +
                    "\t\t update( id: number, obj: "+table.GetTableNameCapitalized()+",resolve,reject ){ \n\tthis.http.put('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({id: id, " + table.GetTableNameCapitalized() + ": obj })}).subscribe(resolve,reject);\n\t };\n" +
                    "\t\t create( obj: "+ table.GetTableNameCapitalized()+", resolve? :( status: number )=>any, reject? :(err)=>any ){ this.http.post('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ " + table.GetTableNameCapitalized() + " : obj })}).subscribe(resolve,reject); };\n" +
                    "\t\t remove( id: number, resolve? :(status: number)=>any, reject? :(err)=>any ){ \n\t\tthis.http.delete('api/" + table.GetTableNameCapitalized() + "',{params: this.toHttpParams({id: id})}).subscribe(resolve,reject); \n\t};\n" +
                "}";


                ModelGenerator modelGenerator = new ModelGenerator();
                //dbcontextCode += "\n" + table.multicount_name + ": " + table.singlecount_name + "[];\n";
                string tscode = modelGenerator.GenerateTypeScript(table);
                files[table.GetTableNameKebabed() + ".service.ts"] = wscode;
                files[table.GetTableNameKebabed() + ".model.ts"] = tscode;
            }
            dbcontextCode = dbcontextCode.Substring(0, dbcontextCode.Length - 2)+"){}\n"+ "}\n";
            files[Naming.ToKebabStyle(_serviceClassName) + ".service.ts"] = imports + dbcontextCode;
            return files;
        }


        /// <summary>
        /// Получение кода TypeScript, содержащего обьявление параметров
        /// </summary>
        /// <param name="parametersMap"> карта параметров имя-тип</param>
        /// <returns> код TypeScript </returns>
        public string GetParameters(Dictionary<string, string> parametersMap)
        {
            string res = "";
            foreach (var p in parametersMap)
            {
                res += $"  {p.Key}:{p.Value}";
            }
            return res;
        }


        /// <summary>
        /// Получение кода TypeScript, содержащего реализацию метода
        /// </summary>
        /// <param name="method"> информация о методе </param>
        /// <param name="implementation"> реализация метода </param>
        /// <returns> код TypeScript c реализацией метода </returns>
        public string GetMethod(MethodInfo method, string implementation)
        {
            return GetMethodDeclaration(method) + "{\n" + $"{implementation}" + "\n}";
        }


        /// <summary>
        /// Получение кода TypeScript, объявляющего метод с заданным именем и параметрами
        /// </summary>
        /// <param name="name"> имя метода </param>
        /// <param name="parameters"> параметры вызова </param>
        /// <returns> код TypeScript с методом </returns>
        public string GetMethodDeclaration(string name, Dictionary<string, object> parameters)
        {
            string declaretions = "";
            foreach (var p in parameters)
            {
                declaretions += $",{p.Key}:{p.Value}";
            }
            return $"{name}({declaretions})";
        }


        /// <summary>
        /// Получение кода TypeScript, объявляющего метод, определенный в C#
        /// </summary>
        /// <param name="method"> информация о методе </param>
        /// <returns> код TypeScript с методом </returns>
        public string GetMethodDeclaration(MethodInfo method)
        {
            return GetMethodDeclaration(method.Name, GetMethodParameters(method));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetMethodParameters(MethodInfo method)
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
