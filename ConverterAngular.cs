
        using System;
using System.Collections.Generic;
 
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using NetCoreConstructorAngular.Data.DataConverter;
using DataConverter.Generators;
using ApplicationCore.Domain.Odbc.Controllers;

namespace NetCoreConstructorAngular.Data.DataConverter.Models { }
namespace ApplicationCore.Converter
    {
        /// <summary>
        /// Реализация обьекта управления приложением Angular.
        /// </summary>
        public class ConverterAngular
        {


            /// <summary>
            /// Путь к корневой директории.
            /// </summary>
            private string ClientAppDirectory = null;
            private string ServerAppDirectory = null;

            private Dictionary<string, string> files = new Dictionary<string, string>();

            private ModelGenerator genModels = new ModelGenerator();
            private RepositoryGenerator genRepositories = new RepositoryGenerator();
            private WebapiGenerator genControllers = new WebapiGenerator();
            private ControllerGenerator genServices = new ControllerGenerator();
            private DbcontextGenerator genTypeScript = new DbcontextGenerator();

          

            /// <summary>
            /// 
            /// </summary>
            /// <param name="dir"></param>
            /// <returns></returns>
            public static string GenerateDataModelToJavaScript(string dir)
            {
                string text = "";
                ModelGenerator generator = new ModelGenerator();
                using (ManagmentDataModel _context = new ManagmentDataModel())
                {
                    List<string> entities = new List<string>(new HashSet<string>((from navs in _context.Model.GetEntityTypes() select navs.Name).ToList()));

                    foreach (string typeName in entities)
                    {
                        Type type = ReflectionService.TypeForName(typeName);
                        string script = generator.GenerateJavascript(type);
                        string filename = dir + @"datamodel\" + Naming.ToKebabStyle(type.Name).ToLower() + ".model.ts";
                        System.IO.File.WriteAllText(filename, script);
                        text += script + "\n\n\n\n";
                    }

                    string dbcontextType = "";
                    dbcontextType += "@Service({ name: '$" + Naming.ToCamelStyle(nameof(AuthorizationDataModel)) + "' }) \n";
                    dbcontextType += "class " + nameof(AuthorizationDataModel) + "\n{\n\n";
                    foreach (string typeName in entities)
                    {
                        string entityType = typeName.LastIndexOf(".") == -1 ? typeName : typeName.Substring(typeName.LastIndexOf(".") + 1);
                        dbcontextType += '\t' + Naming.ToCamelStyle(entityType) + ": EntityRepository<" + entityType + ">;\n\n";
                    }
                    dbcontextType += " \t$http; \n\n";
                    dbcontextType += " \t$spec; \n\n";

                    dbcontextType += " \tconstructor( $http ){ \n";
                    dbcontextType += " \t   this.$spec = window['spec']; \n";
                    dbcontextType += " \t   this.$http = $http; \n";
                    foreach (string typeName in entities)
                    {
                        string entityType = typeName.LastIndexOf(".") == -1 ? typeName : typeName.Substring(typeName.LastIndexOf(".") + 1);
                        dbcontextType += "\t\t this." + Naming.ToCamelStyle(entityType) + " = new EntityRepository<" + entityType + ">('" + entityType + "',$http);\n";
                    }
                    dbcontextType += " \t} \n\n";
                    dbcontextType += "}";
                    string types = System.IO.File.ReadAllText(@$"{System.IO.Directory.GetCurrentDirectory()}\Views\asp-types.const.ts") + "\n\n";
                    System.IO.File.WriteAllText(dir + "application.data-model.ts", types + text);
                    System.IO.File.WriteAllText(dir + "application.db-context.ts", dbcontextType);

                    return dbcontextType;
                }
            }











            /// <summary>
            /// Вовращает строку импорта аннотаций TypeScript
            ///   import { InputDate, InputEmail, InputUrl ... NotNullNotEmpty } from '...';
            /// </summary>
            /// <param name="from"></param>
            /// <returns></returns>
            public static string GetAttributesImport(string from)
            {

                string import = "import {DisplayName,HttpContext,User,Request,UserClaimsPrincipal,RouteData,ViewBag,ModelState,ViewComponentContext,Url,ViewContext,ViewData,TempData,ViewEngine,Key,NotMapped,Table,CollectionType,Column,Index,type,DataType,IsCollection,ForeignProperty,Required,StringLength,MaxLength,JsonIgnore,InverseProperty,";
                List<Type> attributes =
                    new List<Type>(AssemblyReader.GetTypeExtendsFrom(typeof(AuthorizationDataModel).Assembly, nameof(Attribute)));
                /*attributes.Add(typeof(TableAttribute));
                attributes.Add(typeof(StringLengthAttribute));
                attributes.Add(typeof(RequiredAttribute));
                attributes.Add(typeof(DescriptionAttribute));
                attributes.Add(typeof(ClassDescriptionAttribute ));


                attributes.Add(typeof(ControlAttribute));
                attributes.Add(typeof(ComboboxAttribute));
                attributes.Add(typeof(SelectControlAttribute));
                attributes.Add(typeof(SelectControlWithoutValidation));
                attributes.Add(typeof(NotMappedAttribute));
                attributes.Add(typeof(KeyAttribute));
                attributes.Add(typeof(ForeignKeyAttribute));*/
                foreach (Type attr in attributes)
                {
                    import += attr.Name.Replace("Attribute", "") + ",";
                }
                if (attributes.Count > 0)
                {
                    import = import.Substring(0, import.Length - 1);
                }
                import += "} from '" + from + "';\n";
                return import;
            }



            /// <summary>
            /// Создаёт даталогическую модель в файлах TypeScript и службы доступа к 
            /// данным по HTTP через CRUD-операции.
            /// </summary>
            /// <param name="dir"></param>
            public static void GenerateDataModelToTypeScript(
                                string dir, CommonDataModel data)
            {
                string importAttributes = GetAttributesImport("./../asp-types.const");


                string imports = "";
                ModelGenerator generator = new ModelGenerator();
                List<string> allEntities = new List<string>();
                var imported = new HashSet<string>();
                data.ForEach(_context => {
                    List<string> entities = EntityUtils.GetTables((object)_context);

                    allEntities.AddRange(entities);
                    foreach (string typeName in entities)
                    {
                     
                        
                        System.Console.WriteLine(typeName);
                      
                        Type type = ReflectionService.TypeForName(typeName);
                        type = type==null? ReflectionService.TypeForShortName(typeName): type;

                        Dictionary<string, string> attributes = AttrsUtils.GetEntityContrainsts(type);
                        Dictionary<string, object> navProperties = new Dictionary<string, object>();
                        foreach (var nav in EntityUtils.GetNavigation(type))
                        {
                            var navoptions = new MyNavigationOptions(nav);
                            navoptions.IsCollection = IsCollectionType(type.GetProperty(nav.Name).PropertyType);
                            Dictionary<string, object> options = new DataSeriallizer().ToValuesMap(navoptions);
                            options["Navigation"] = nav.Name + "ID";

                            navProperties[nav.Name] = options;
                        }

                        
                        string entityType = typeName.LastIndexOf(".") == -1 ? typeName : typeName.Substring(typeName.LastIndexOf(".") + 1);
                        if (imported.Contains(entityType) == false) { 
                            imports += "import {" + entityType + "} from './data-model/" + Naming.ToKebabStyle(type.Name).ToLower() + ".model';\n";
                        }
                        string script = generator.GenerateTypeScript(attributes, type, importAttributes, navProperties);
                        string filename = dir + @"" + Naming.ToKebabStyle(type.Name).ToLower() + ".model.ts";
                        imported.Add(entityType);
                        System.IO.File.WriteAllText(filename, script);
                            
                        
                    }

                     
                });
                imports += "import {EntityRepository} from './entity-repository';\n";
                imports += "import { MyHttpClient } from './my-http-client.service';\n";
                imports += "import {EntityRepositoryFactory} from './entity-repository.factory';\n";
                imports += "import {Injectable} from '@angular/core';\n\n";

                imports += "\n";

                string dbcontextType = imports + "\n\n";
                dbcontextType += "@Injectable({ providedIn: 'root' }) \n";
                dbcontextType += "export class ApplicationDbContext\n{\n\n";
                foreach (string typeName in new HashSet<string>(allEntities))
                {
                    string entityType = typeName.LastIndexOf(".") == -1 ? typeName : typeName.Substring(typeName.LastIndexOf(".") + 1);
                    dbcontextType += '\t' + Naming.ToCamelStyle(entityType) + ": EntityRepository<" + entityType + ">;\n";
                }
                dbcontextType += " \t" + "$http: MyHttpClient; \n\n";

                dbcontextType += " \t" + "constructor( $http: MyHttpClient, $entityRepositoryFactory: EntityRepositoryFactory ){ \n";
                dbcontextType += " \t\t" + "this.$http = $http; \n";
                foreach (string typeName in new HashSet<string>(allEntities))
                {
                    string entityType = typeName.LastIndexOf(".") == -1 ? typeName : typeName.Substring(typeName.LastIndexOf(".") + 1);
                    dbcontextType += "\t\t" + "this." + Naming.ToCamelStyle(entityType) + " = $entityRepositoryFactory.create<" + entityType + ">('" + entityType + "',$http,function(){ return new " + entityType + "(); });\n";
                }
                dbcontextType += " \t} \n\n";
                dbcontextType += "}";

                string filepath = dir + @"\.." + @"\application.entity-fasade.ts";
                System.IO.File.WriteAllText(filepath, dbcontextType);



                //generator.CreateTypeScriptModel(typeof(Person))
            }

        private static bool IsCollectionType(Type propertyType)
        {
            throw new NotImplementedException();
        }

        public ConverterAngular() : this(System.IO.Directory.GetCurrentDirectory())
            {

            }






            /// <summary>
            /// Конструктор 
            /// </summary>
            /// <param name="contentRoot"> абсолютный путь к приложению </param>
            public ConverterAngular(string contentRoot)
            {
                ClientAppDirectory = contentRoot + @"\Views\ClientApp\";
                ServerAppDirectory = contentRoot;

                if (System.IO.Directory.Exists(this.ClientAppDirectory) == false)
                {
                    System.IO.Directory.CreateDirectory(this.ClientAppDirectory);
                }
            }


            public void Do()
            {
                Do(DatabaseManager.GetOdbcDatabaseManager().GetDataSource(), nameof(ManagmentDataModel));
                Do(DatabaseManager.GetOdbcDatabaseManager().GetDataSource(), nameof(AuthorizationDataModel));
            }


            /// <summary>
            /// Метод генерации сервисов доступа к контроллерам MVC
            /// </summary>
            /// <param name="myApplicationModels"></param>
            public void CreateServices(MyApplicationModel myApplicationModels)
            {
                ControllerGenerator generator = new ControllerGenerator();
                foreach (var p in generator.CreateModels().controllers)
                {
                    string kebabName = Naming.ToKebabStyle(p.Key).ToLower();
                    string typeScript = generator.AngularJsService(p.Value);
                    System.IO.File.WriteAllText(
                        @$"{ClientAppDirectory}\controllers\{kebabName}.ts", typeScript);
                }
            }

            /// <summary>
            /// Генерация контроллеров для источника данных
            /// </summary>
            /// <param name="datasource"> Источник данных </param>
            public void Do(IDataSource datasource, string dbContextName)
            {
                IDatabaseMetadata databasemetadata = datasource.GetDatabaseMetadata();
                databasemetadata.Validate();
                List<string> entityControllers = new List<string>();

                //genTypeScript.createDataContext(datasource.GetDatabaseMetadata());
                foreach (var ptable in databasemetadata.GetTables())
                {
                    Console.WriteLine(ptable.Value.name);
                    IDataTable metadata = datasource.GetDatabaseMetadata().GetTables()[ptable.Key];
                    //string apiController = genControllers.CreateEntityController(metadata);
                    //files["WebAPI\\"+Naming.ToCapitalStyle(metadata.name) + "Controller.cs"] = apiController;

                    // классы сущностей
                    //string sharpModel = genModels.CreateSharpModel(metadata);
                    //files[$"{ServerAppDirectory}Models\\" + Naming.ToCapitalStyle(metadata.singlecount_name) + ".cs"] = sharpModel;

                    // классы сущностей
                    string tsModel = genModels.GenerateTypeScript(metadata);
                    files[$"{ClientAppDirectory}\\models\\" + Naming.ToKebabStyle(metadata.GetSingleCountName()) + ".datamodel.ts"] = tsModel;

                    // webapi контроллеры
                    /*string crudRepository = genRepositories.CreateEntityRepository(metadata, dbContextName);
                    files[$"{ServerAppDirectory}Controllers\\"+Naming.ToCapitalStyle(metadata.name) + "Controller.cs"] = crudRepository;
                    entityControllers.Add(Naming.ToCapitalStyle(metadata.name) + "Controller");
                    string serviceController = this.genServices.CreateServiceController(new Models.MyControllerModel()
                    {
                        Name =      Naming.ToCapitalStyle(metadata.name) + "Controller",
                        Path =      $"/api/{Naming.ToCapitalStyle(metadata.name)}",
                        Actions =  new Dictionary<string, MyActionModel>()
                        {
                            {   "Find",
                                new MyActionModel(){
                                    Name="Find",
                                    Path=$"/api/{Naming.ToCapitalStyle(metadata.name)}/Find",
                                    Parameters = new Dictionary<string, MyParameterModel>()
                                    {
                                        { "id",
                                            new MyParameterModel(){

                                                Name = "id",
                                                Type = "int",
                                                IsOptional = false
                                        }   }
                                    }
                                }
                            },
                            {   "Remove",
                                new MyActionModel(){
                                    Name="Remove",
                                    Path=$"/api/{Naming.ToCapitalStyle(metadata.name)}/Remove",
                                    Parameters = new Dictionary<string, MyParameterModel>()
                                    {
                                        { "id",
                                            new MyParameterModel(){
                                                Name = "id",
                                                Type = "int",
                                                IsOptional = false
                                        }   }
                                    }
                                }
                            },
                            {   "Create",
                                new MyActionModel(){
                                    Name="Create",
                                    Path=$"/api/{Naming.ToCapitalStyle(metadata.name)}/Create",
                                    Parameters = new Dictionary<string, MyParameterModel>()
                                    {
                                        { "record",
                                            new MyParameterModel(){
                                                Name = "record",
                                                Type = "object",
                                                IsOptional = false
                                        }   }
                                    }
                                }
                            },
                            {   "List",
                                new MyActionModel(){
                                    Name="List",
                                    Path=$"/api/{Naming.ToCapitalStyle(metadata.name)}/List",
                                    Parameters = new Dictionary<string, MyParameterModel>()
                                    {                                   
                                    }
                                }
                            }
                        }
                    }); 
                    files[$"{ClientAppDirectory}\\src\\app\\controllers\\" + Naming.ToKebabStyle(metadata.multicount_name).ToLower() + ".service.ts"] = serviceController;
                    */
                }


                // создаем директорию
                if (System.IO.Directory.Exists($"{ClientAppDirectory}\\controllers") == false)
                {
                    System.IO.Directory.CreateDirectory($"{ClientAppDirectory}\\controllers");
                }

                // создаем сервисы взаимодействующие с контроллерам
                Dictionary<string, string> webapi = this.genServices.GetWebApi();
                foreach (var api in webapi)
                {
                    files[$"{ ClientAppDirectory}\\src\\app\\controllers\\" + Naming.ToKebabStyle(api.Key).ToLower() + ".service.ts"] = webapi[api.Key];
                }

                /*// создаём контекст данных
                files[$"{ClientAppDirectory}\\src\\app\\controllers\\" + Naming.ToKebabStyle(dbContextName.Replace("DbContext", "")).ToLower() + ".service.ts"]
                    = this.CreateWebApiDbContext(dbContextName, entityControllers);*/


                /*string dbContextCode = $"public partial class {dbContextName}: Microsoft.EntityFrameworkCore.DbContext";
                dbContextCode += "\n{";
                dbContextCode += $"\n\n\tpublic {dbContextName}():base()" + "{}";
                dbContextCode += $"\n\n\tpublic {dbContextName}(Microsoft.EntityFrameworkCore.DbContextOptions<{dbContextName}> options): base(options)" + "{}";
                foreach (var ptable in databasemetadata.Tables)
                {
                    dbContextCode += $"\n\n\tpublic virtual Microsoft.EntityFrameworkCore.DbSet<{Naming.ToCapitalStyle(ptable.Value.singlecount_name)}> {Naming.ToCapitalStyle(ptable.Value.multicount_name)}"+" { get; set; }";
                }
                dbContextCode += "\n}";
                files[dbContextName + ".cs"] = dbContextCode;*/


            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="dbContextName"></param>
            /// <param name="entityControllers"></param>
            /// <returns></returns>
            private string CreateWebApiDbContext(string dbContextName, List<string> entityControllers)
            {
                string typeScript = "import { HttpClient,HttpParams } from '@angular/common/http';\n";
                typeScript += "import { Injectable } from '@angular/core';\n\n";


                string imports = "";
                string injection = "";
                foreach (string ctrl in entityControllers)
                {
                    imports += "import {" + Naming.ToCapitalStyle(ctrl) + "Service}" + $" from './{Naming.ToKebabStyle(ctrl).ToLower().Replace("-controller", "")}.service';\n";
                    injection += "public " + Naming.ToCamelStyle(ctrl) + ": " + ctrl + "Service,\n\t\t\t\t";
                }
                if (injection.EndsWith(","))
                {
                    injection = injection.Substring(0, injection.Length - 1);
                }
                string code = $"\n\nexport class {dbContextName.Replace("DbContext", "")}Service" + "{\n";
                code += $"\t\tconstructor({injection})" + "{}";
                code += "}";
                code = "@Injectable({ providedIn: 'root' })\n" + code;
                return typeScript + imports + code;
            }


            /// <summary>
            /// Сохранение кода в файловую систему
            /// </summary>
            public void Save()
            {
                Console.WriteLine(this.ClientAppDirectory);

                if (System.IO.Directory.Exists(this.ClientAppDirectory + $"\\Models") == false)
                {
                    System.IO.Directory.CreateDirectory(this.ClientAppDirectory + $"\\Models");
                }
                if (System.IO.Directory.Exists(this.ClientAppDirectory + $"\\WebAPI") == false)
                {
                    System.IO.Directory.CreateDirectory(this.ClientAppDirectory + $"\\WebAPI");
                }
                if (System.IO.Directory.Exists(this.ClientAppDirectory + $"\\Controllers") == false)
                {
                    System.IO.Directory.CreateDirectory(this.ClientAppDirectory + $"\\Controllers");
                }
                if (System.IO.Directory.Exists(this.ClientAppDirectory + $"\\ClientApp\\src\\app\\models") == false)
                {
                    System.IO.Directory.CreateDirectory(this.ClientAppDirectory + $"\\ClientApp\\src\\app\\models");
                }

                foreach (var pfile in files)
                {
                    string filename = $"{pfile.Key}";
                    System.IO.File.WriteAllText(filename, pfile.Value);
                    Console.WriteLine(filename + " " + pfile.Value.Length + " bytes");
                };
                Console.WriteLine(this.ClientAppDirectory);
            }


            /// <summary>
            /// Генерация сервисов для доступа к базе данных
            /// </summary>
            /// <param name="odbc"> компонент ODBC </param>
            /// <param name="serviceClassName"> класс контекста данных </param>
            /// <param name="serviceFileName"> имя файла контекста данных </param>
            /// <param name="outputDir"> директория вывода </param>
            public void CreateDataContext(IDataSource odbc, string serviceClassName, string serviceFileName, string outputDir)
            {
                outputDir = !outputDir.StartsWith(System.IO.Directory.GetCurrentDirectory()) ?
                    System.IO.Directory.GetCurrentDirectory() + outputDir : outputDir;
                if (!System.IO.Directory.Exists(outputDir))
                {
                    throw new ArgumentException("outputDir");
                }

                IDatabaseMetadata mdb = odbc.GetDatabaseMetadata();
                string imports = "import { Component,Injectable } from '@angular/core';\n";
                string dbcontextCode = "\n@Injectable({providedIn: 'root'})\nexport class " + serviceClassName + "\n{\n";
                dbcontextCode += "\t constructor( \n";

                foreach (IDataTable table in mdb.GetTables().Values)
                {
                    imports += "import { " + table.GetTableNameCapitalized() + "Service } from './" + table.GetSingleCountName() + ".service';\n";
                    dbcontextCode += "\t\t public " + table.getTableNameCamelized() + ": " + table.GetTableNameCapitalized() + "Service,\n";


                    string wscode = "import { Component,Inject,Injectable } from '@angular/core';\n" +
                                    "import { HttpClient, HttpParams } from '@angular/common/http';\n" +
                                    "import { " + table.GetTableNameCapitalized() + " } from './" + table.GetSingleCountName() + ".model';\n";
                    wscode += "@Injectable({providedIn: 'root'})\nexport class " + table.GetTableNameCapitalized() + "Service \n{\n" +
                        "\t\t constructor(  private http: HttpClient ){} \n" +

                        "\t\t toHttpParams(obj: any){\n" +
                        "\t\t    const result = new HttpParams();\n" +
                        "\t\t    Object.getOwnPropertyNames(obj).forEach(name => {\n" +
                        "\t\t        result.set(name, JSON.stringify(obj[name]));\n" +
                        "\t\t    });\n" +
                        "\t\t    return result;\n" +
                        "\t\t }\n" +
                    "\t\t list( resolve: ( data: " + table.GetTableNameCapitalized() + "[])=>any, reject? :(err)=>any ){ this.http.get<" + table.GetTableNameCapitalized() + "[]>('api/" + table.GetTableNameCapitalized() + "').subscribe(resolve,reject); };\n" +
                        "\t\t find( id: number, resolve: (data: " + table.GetTableNameCapitalized() + ")=>any, reject? :(err)=>any ){ this.http.get('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ id: id })}).subscribe(resolve,reject); };\n" +
                        "\t\t update( id: number, obj: " + table.GetTableNameCapitalized() + ",resolve,reject ){ this.http.put('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({id: id, " + table.GetTableNameCapitalized() + ": obj })}).subscribe(resolve,reject); };\n" +
                        "\t\t create( obj: " + table.GetTableNameCapitalized() + ", resolve? :( status: number )=>any, reject? :(err)=>any ){ this.http.post('api/" + table.GetTableNameCapitalized() + "',{params:this.toHttpParams({ " + table.GetTableNameCapitalized() + " : obj })}).subscribe(resolve,reject); };\n" +
                        "\t\t remove( id: number, resolve? :(status: number)=>any, reject? :(err)=>any ){ this.http.delete('api/" + table.GetTableNameCapitalized() + "',{params: this.toHttpParams({id: id})}).subscribe(resolve,reject); };\n" +
                    "}";



                    //dbcontextCode += "\n" + table.multicount_name + ": " + table.singlecount_name + "[];\n";
                    string tscode = genModels.GenerateTypeScript(table);
                    System.IO.File.WriteAllText(outputDir + table.GetSingleCountName() + ".service.ts", wscode);
                    System.IO.File.WriteAllText(outputDir + table.GetSingleCountName() + ".model.ts", tscode);

                }
                dbcontextCode = dbcontextCode.Substring(0, dbcontextCode.Length - 2) + "){}\n" + "}\n";
                System.IO.File.WriteAllText(outputDir + serviceFileName + ".service.ts", imports + dbcontextCode);
            }
        }
    }