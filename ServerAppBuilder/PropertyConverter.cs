using Newtonsoft.Json.Linq;
using System;

namespace DataConverter.Generators
{
    public class PropertyConverter
    {

        /// <summary>
        /// Метод сопоставления типов данных SQL с типами C#
        /// </summary>
        /// <param name="sqlDataType"> тип данных SQL </param>
        /// <returns> тип C# </returns>
        public object getSharpDataType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "date":
                case "datetime":
                    return "System.DateTime";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "double":
                case "float":
                case "decimal":
                case "integer":
                case "number":
                case "numeric":
                    return "int";

                case "char":
                case "longtext":
                case "nvarchar":
                case "varchar":
                    return "string";
                case "blob":
                    return "byte[]";

                default:
                    throw new Exception("Не поддерживаемый тип данных " + sqlDataType);
            }
        }


        public string GetTypeScriptDataType(Type type)
        {
            if (type.Equals(typeof(String)))
            {
                return "string";
            }
            else if (type.Equals(typeof(Boolean)))
            {
                return "boolean";
            }
            else if (type.Equals(typeof(Int32)))
            {
                return "number";
            }
            else if (type.Equals(typeof(Int64)))
            {
                return "number";
            }
            else if (type.Equals(typeof(DateTime)))
            {
                return "Date";
            }
            else if (type.Equals(typeof(JObject)))
            {
                return "any";
            }
            else if (type.Equals(typeof(JArray)))
            {
                return "any";
            }
            else if (type.Equals(typeof(JToken)))
            {
                return "any";
            }
            else if (type.Equals(typeof(JValue)))
            {
                return "any";
            }
            else if (type.Name.StartsWith("Action"))
            {
                return "Function";
            }
            else if (type.Equals(typeof(object)))
            {
                return "any";
            }
            else if (IsCollectionType(type))
            {
                string qname = type.AssemblyQualifiedName;
                string elementTypeName = ParseCollectionType(type);
                Type elementType = ReflectionService.TypeForName(elementTypeName);
                
                string elementTypeTs = GetTypeScriptDataType(elementType);
                return elementTypeTs+"[]";
            }
            else
            {
                return "any";
                //throw new Exception("Тип данных "+type.Name+" в настоящий момент не поддерживается");
            }
        }

        private bool IsCollectionType(Type type)
        {
            throw new NotImplementedException();
        }

        private string ParseCollectionType(Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Метод сопоставления типов данных SQL с типами TypeScript
        /// </summary>
        /// <param name="cSharpType"> тип данных SQL </param>
        /// <returns> тип TypeScript </returns>
        public string GetTypeScriptDataType(string cSharpType)
        {

            switch (cSharpType.ToLower())
            {
                case "bool":
                case "boolean":

                    return "boolean";
                case "date":
                case "datetime":
                    return "Date";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "double":
                case "float":
                case "decimal":
                case "integer":
                case "number":
                case "numeric":
                    return "number";


                case "char":
                case "longtext":

                case "nvarchar":
                case "varchar":
                    return "string";
                case "blob":
                    return "byte[]";
                default:
                    return "any";
                    //throw new Exception("Не поддерживаемый тип данных "+ sqlDataType);
            }
        }


        /// <summary>
        /// Метод получения типа данных SQL совместимого с типом C#
        /// </summary>
        /// <param name="type"> тип c# </param>
        /// <returns> Тип SQL </returns>
        public string GetSqlDataType(Type type)
        {
            switch (type.Name.ToLower())
            {
                case "jarray":
                    return "List<object>";
                case "jvalue":
                    return "object";
                case "datetime":
                    return "datetime";
                case "int":
                case "int32":
                case "int64":
                case "long":
                case "double":
                case "float":
                case "decimal":
                case "integer":
                case "number":
                case "numeric":
                    return "int";
                case "string":
                    return "varchar";
                default:
                    throw new Exception($"Не совместимый тип данных: {type.Name}");
            }
        }

    }
}