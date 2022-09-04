
using Managment.DataModel;
using NetCoreConstructorAngular.Data.DataConverter.Models;
using System;
using System.Collections.Generic;
using TeleReportsDataProvider.Data;
namespace Managment.DataModel { }
namespace DataConverter.Generators
{
    public class ModelGenerator: PropertyConverter
    {
        



        /// <summary>
        /// Метод создания класса TypeScript, реализующего модель данных сущности
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код TypeScript, реализующий модель данных сущности </returns>
        public string GenerateJavascript(Type type)
        {
            MyMessageModel model = new MyMessageModel(type);
            //string typescript = "import { Len,Editable,Email,Format,Help,Hidden,Icon,InputType,Label,NotNullNotEmpty,RegExp,UrlValidation,Image,Select,Day,File,Month,Password,Phone,QR,URL,Week,Year } from 'src/app/app-ui/ui-forms/input-form/annotations/asp-types.const';\n\n" +

            string javascript = "";
            var attrs = AttrsUtils.GetEntityContrainsts(type);
            foreach(var attr in attrs)
            {
                javascript += "" + "@" + attr.Key.Replace("Attribute", "") + $"('{attr.Value}')\n";
            }
            
            javascript += $@"class {model.Name}" + "{" + $@"" + "\n\n\n";
            foreach (var prop in model.Properties)
            {
                foreach (var attr in prop.Attributes)
                {
                    javascript += "\t @" + attr.Key.Replace("Attribute","") + $"('{attr.Value}')\n";
                }

                string typescriptType = 
                  
                    GetTypeScriptDataType(prop.Type);
                //System.Console.WriteLine(typescriptType + " : " + prop.Type);
                string defaultValueString =
                    typescriptType == "string" ? "''" :
                    typescriptType == "number" ? "0" :
                    typescriptType == "Date" ? "new Date()" :
                    "null";
                javascript += "\t " + prop.Name + $" :{typescriptType}={defaultValueString};\n\n\n";
            }
            
            javascript += "}" + $@"";
            return javascript;
        }

        

        /// <summary>
        /// Метод создания класса TypeScript, реализующего модель данных сущности
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код TypeScript, реализующий модель данных сущности </returns>
        public string GenerateTypeScript(
            Dictionary<string,string> Attributes, 
            Type type, 
            string import,
            Dictionary<string, object> navigationProperties=null)

        {
            MyMessageModel model = new MyMessageModel(type);
            string typescript =
                // @"import { TextLength,BindProperty,InputDateTime,Required,InputMultilineText, InputPhone,InputEmail,InputUrl,RusText,Key,InputDate, NotMapped,InputHidden,Len,Editable,InputEmailAttribute ,Format,Help,InputHiddenAttribute,Icon,InputType,Label,NotNullNotEmpty, UrlValidation,InputImageAttribute,Select,Day,KeyAttribute,InputBinaryAttribute,InputDateTimeAttribute,InputMonthAttribute,InputDateAttribute,InputMultilineTextAttribute,InputPasswordAttribute,InputPhoneAttribute,InputUrlAttribute} from 'src/app/app-ui/ui-forms/input-form/annotations/asp-types.const';" + "\n\n" +
                import+"\n\n";


            HashSet<string> entityTypesForImport = new HashSet<string>();
            List<string> propertyNames = new List<string>(navigationProperties.Keys);
         

            foreach (var attr in Attributes)
            {
                typescript += "" + "@" + attr.Key.Replace("Attribute", "") + $"('{attr.Value}')\n";
            }

            typescript += ""+ $@"export class {model.Name}" + "{" + $@""+ "\n\n\n";

            var props = ReflectionService.GetOwnPropertyNames(type);
            Dictionary<string, object> navPropertyIndicators = null;
            foreach (var prop in model.Properties)
            {
                
                if(AttrsUtils.IsInput(type, prop.Name) == false)
                {
                    continue;
                }

               
                System.Console.WriteLine(type.Name+" "+prop.Name);
                var p = type.GetProperty(prop.Name);
                object mp = ParseProperty(type,p );
                foreach (var attr in prop.Attributes)
                {
                    typescript += "\t"+"@" + attr.Key.Replace("Attribute","") + $"('{attr.Value}')\n";
                }
                if (navigationProperties != null)
                {
                    if (navigationProperties.ContainsKey(prop.Name))
                    {
                        navPropertyIndicators=
                            (Dictionary<string,object>)navigationProperties[prop.Name];
                        foreach(var pindicator in navPropertyIndicators)
                        {
                            typescript += "\t" + "@" + pindicator.Key.Replace("Attribute", "") + $"('{pindicator.Value}')\n";                            
                        }
                    }
                }
                if (IsCollectionType(mp.GetType()))
                {
                    typescript += "\t" + "@CollectionType"+$"('{ReflectionService.GetValueFor(mp,"Type")}')\n";
                }
                

                string typescriptType = null;                
                if (IsExtendedFrom(ReflectionService.TypeForName(prop.Type), nameof(BaseEntity)))
                {
                    typescriptType = "any";
                    //typescriptType = prop.Type;
                    //entityTypesForImport.Add(typescriptType);
                }
               
                else
                {
                    
                    typescriptType = GetTypeScriptDataType(prop.Type);
                }
                    
                if (navPropertyIndicators != null)
                {
                    object isCollection = navPropertyIndicators["IsCollection"];
                    /*if (navPropertyIndicators["IsCollection"] == "true" || navPropertyIndicators["IsCollection"] == Boolean.TrueString)
                    {
                        typescriptType += "[]";
                        typescript += "\t" + prop.Name + $" :{typescriptType}=[];\n\n\n";
                    }*/
                    
                    if ((bool)ReflectionService.GetValueFor(mp, "IsCollection") )
                    {
                        typescriptType = ReflectionService.GetValueFor(mp, "Type") + "[]";
                        entityTypesForImport.Add((string)ReflectionService.GetValueFor(mp, "Type"));
                        typescript += "\t" + prop.Name + $" :{typescriptType}=[];\n\n\n";
                    }
                    else
                    {
                        string defaultValueString =
                            typescriptType == "boolean" ? "false" :
                            typescriptType == "string" ? "''" :
                            typescriptType == "number" ? "0" :
                            typescriptType == "Date" ? "new Date()" :
                            "null";
                        typescript += "\t" + prop.Name + $" :{typescriptType}={defaultValueString};\n\n\n";
                    }              
                    
                }
                else
                {
                    string defaultValueString =
                        typescriptType == "boolean" ? "false" :
                        typescriptType == "string" ? "''" :
                        typescriptType == "number" ? "0" :
                        typescriptType == "Date" ? "new Date()" :
                        "null";
                    typescript += "\t" + prop.Name + $" :{typescriptType}={defaultValueString};\n\n\n";
                }
          
            }
            typescript += "\t"+"@InputHidden(\"True\")\n";
            typescript += "\t"+"type: string = '"+type.FullName+"';\n\n\n";
            typescript += "}"+$@"";
            if (entityTypesForImport.Count > 0)
            {                
                foreach(string entityType in entityTypesForImport)
                {
                    if (entityType != type.Name)
                    {
                        typescript = "import {" + entityType + "} from './" + Naming.ToKebabStyle(entityType) + ".model';\n" + typescript;
                    }
                    
                }                
            }
            return typescript;
        }

        private bool IsExtendedFrom(Type type, string v)
        {
            throw new NotImplementedException();
        }

        private object ParseProperty(Type type, System.Reflection.PropertyInfo p)
        {
            throw new NotImplementedException();
        }

        private bool IsCollectionType(Type type)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Метод создания класса TypeScript, реализующего модель данных сущности
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код TypeScript, реализующий модель данных сущности </returns>
        public string GenerateTypeScript( IDataTable table )
        {
            string header = $"class {Naming.ToCapitalStyle(table.GetSingleCountName())}\n";
            string body = "";
            foreach (string columnName in table.Properties.Keys)
            {
                IColumnMetaData column = table.GetColumnMetaData(columnName);
                body += "\t" + Naming.ToCamelStyle(column.name) + ": " + GetTypeScriptDataType(column.GetTypeName()) + "; \n";
            }
            string classCode = header + "{\n" + body + "}";
            return classCode;
        }


        /// <summary>
        /// Метод создания класса TypeScript, реализующего модель данных сущности
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код TypeScript, реализующий модель данных сущности </returns>.
        
        public string GenerateCSharp(IDataTable table)
        {
            string header = 
                $"[System.ComponentModel.DataAnnotations.Schema.Table(\"{table.name}\")]\n" +
                $"public class {Naming.ToCapitalStyle(table.GetSingleCountName())}\n";
            string body = "";
            
            foreach (string columnName in table.Properties.Keys)
            {
                IColumnMetaData column = table.GetColumnMetaData(columnName);
                if(column.primary == true || table.getPrimaryKey().ToLower()==column.name.ToLower())
                {
                    body += $"\t [System.ComponentModel.DataAnnotations.Key()]\n";
                }      
                foreach (var annotation in table.GetAnnotations(columnName))
                {
                    body += $"\t {annotation}\n";
                    
                }
                body += $"\t public {getSharpDataType(column.GetTypeName())} {Naming.ToCapitalStyle(column.name)}"+ "{ get; set; }\n\n";


                body += "\n\n";
            }
            //body += $"\t public System.DateTime Created " + "{ get; set; }\n\n";
            //body += $"\t public System.DateTime Updated " + "{ get; set; }\n\n";
            //body += $"\t public int Version " + "{ get; set; }\n\n";
            //body += $"\t public int Popularity " + "{ get; set; }\n\n";
            //body += $"\t public int Rating " + "{ get; set; }\n\n";

            foreach ( var fk in table.fk)
            {
                body += $"\t [System.ComponentModel.DataAnnotations.Schema.ForeignKey(\"P{ Naming.ToCapitalStyle(fk.Value)}Id\")]\n";
                body += $"\t public int P{ Naming.ToCapitalStyle(fk.Value)}Id " + "{ get; set; }\n";
                body += $"\t public virtual {Naming.ToCapitalStyle(fk.Value)} P{Naming.ToCapitalStyle(fk.Value)}" + "{ get; set; }\n";
            }
            string classCode = header + "{\n" + body + "}";
            return classCode;
        }


        
    }
}
