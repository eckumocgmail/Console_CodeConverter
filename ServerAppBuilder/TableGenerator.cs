using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Text.RegularExpressions;

namespace DataConverter.Generators
{
    public class TableGenerator: PropertyConverter
    {

        public TableMetaData GenerateModel(Type type)
        {
            var target = ReflectionService.CreateWithDefaultConstructor<object>(type);
            JObject jobject = JObject.FromObject(target);
            return this.GenerateModel(type.Name, jobject);
        }

        /// <summary>
        /// Метод получения структуы данных из JSON-обьекта
        /// </summary>
        /// <param name="json"> json </param>
        /// <returns> структура данных </returns>
        public TableMetaData GenerateModel(string name, string json)
        {
            JObject jobject = JsonConvert.DeserializeObject<JObject>(json);
            return this.GenerateModel(name, jobject);
        }


        /// <summary>
        /// Метод получения структуы данных из JSON-обьекта
        /// </summary>
        /// <param name="json"> json </param>
        /// <returns> структура данных </returns>
        public TableMetaData GenerateModel(string name, JObject jobject)
        {
            Regex intRegex = new Regex(@"[\d]");
            Regex floatRegex = new Regex(@"^-?[0-9][0-9,\.]+$");
            TableMetaData metadata = new TableMetaData();
            metadata.name = name;
            foreach (var p in jobject)
            {    
                if(p.Key.ToLower() == "id")
                {
                    metadata.pk = Naming.ToCapitalStyle(p.Key);
                }
                metadata.columns[Naming.ToCapitalStyle(p.Key)] = new ColumnMetaData()
                {
                    name = Naming.ToCapitalStyle(p.Key),
                    nullable = true,
                    type = 
                        intRegex.IsMatch(p.Value.ToString()) ? "int" :
                        intRegex.IsMatch(p.Value.ToString()) ? "float" :
                        "string"
                };
            }
             
            return metadata;
        }




        
    }
}
