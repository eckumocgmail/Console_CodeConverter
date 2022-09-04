

namespace DataConverter.Generators
{
    public class WebapiGenerator
    {
        /// <summary>
        /// Метод создания класса c#, реализующего CRUD операции с сущностью
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код c#, реализующий CRUD операции над сущностью </returns>
        public string CreateEntityController( IDataTable table )
        {
            string wscode = "import { Component,Inject,Injectable } from '@angular/core';\n" +
                            "import { HttpClient, HttpParams } from '@angular/common/http';\n" +
                            "import { " + table.GetTableNameCapitalized() + " } from './" + table.GetTableNameKebabed() + ".model';\n";
            wscode += "\n\n@Injectable({providedIn: 'root'})\nexport class " + table.GetTableNameCapitalized() + "Service \n{\n" +
                "\t constructor(  private http: HttpClient ){} \n" +

                "\t toHttpParams(obj: any){\n" +
                "\t    const result = new HttpParams();\n" +
                "\t    Object.getOwnPropertyNames(obj).forEach(name => {\n" +
                "\t        result.set(name, JSON.stringify(obj[name]));\n" +
                "\t    });\n" +
                "\t    return result;\n" +
                "\t }\n" +
            "\t list( resolve: ( data: " + table.GetTableNameCapitalized() + "[])=>any, reject? :(err)=>any ){ \n\t\tthis.http.get<" + table.GetTableNameCapitalized() + "[]>('api/" + table.GetTableNameCapitalized() + "').subscribe(resolve,reject);\n\t };\n" +
                "\t find( id: number, resolve: (data: " + table.GetTableNameCapitalized() + ")=>any, reject? :(err)=>any ){ \n\t\tthis.http.get('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ id: id })}).subscribe(resolve,reject);\n\t };\n" +
                "\t update( id: number, obj: " + table.GetTableNameCapitalized() + ",resolve,reject ){ \n\t\tthis.http.put('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({id: id, " + table.GetTableNameCapitalized() + ": obj })}).subscribe(resolve,reject);\n\t };\n" +
                "\t create( obj: " + table.GetTableNameCapitalized() + ", resolve? :( status: number )=>any, reject? :(err)=>any ){ \n\t\tthis.http.post('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ " + table.GetTableNameCapitalized() + " : obj })}).subscribe(resolve,reject); };\n" +
                "\t remove( id: number, resolve? :(status: number)=>any, reject? :(err)=>any ){ \n\t\tthis.http.delete('api/" + table.GetTableNameCapitalized() + "',{params: this.toHttpParams({id: id})}).subscribe(resolve,reject); \n\t};\n" +
            "}";
         
            return wscode;
        }        
    }
}
