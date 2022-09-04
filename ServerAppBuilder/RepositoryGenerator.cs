


namespace DataConverter.Generators
{
    public class RepositoryGenerator
    {

        /// <summary>
        /// Метод создания класса TypeScript, реализующего CRUD операции с сущностью
        /// </summary>
        /// <param name="table"> модель данных сущности </param>
        /// <returns> код TypeScript, реализующий CRUD операции над сущностью </returns>
        public string CreateEntityRepository(IDataTable table, string dbContextClassName)
        {
            string header =
                "using System.Linq;\n"+
                "using ApplicationDb;\n using LibsDb; \n" +
                "using LibsDb.LibsEntities;\n" +               
                "using ApplicationDb.Entities;\n" +

            "using Microsoft.AspNetCore.Mvc;\n" +
                "using Microsoft.Extensions.Logging;\n\n\n" +
            "[ApiController]\n" +
                $"[Route(\"api/{Naming.ToCapitalStyle(table.name)}/[action]\")]\n" +
                $"public class {Naming.ToCapitalStyle(table.name)}Controller: ControllerBase\n";
            string body = 
                $"\tprotected {dbContextClassName} _db; \n\n\n" +
                $"\tpublic {Naming.ToCapitalStyle(table.name)}Controller( {dbContextClassName} db )"+ "{\n";            
            body += "\t\t_db=db;\n";
            body += "\t}\n\n";
            body += $"\tpublic void Create( {Naming.ToCapitalStyle(table.singlecount_name)} record )";
            body += "\t{\n";
            body += $"\t\t_db.{Naming.ToCapitalStyle(table.name)}.Add(record);\n";
            body += $"\t\t_db.SaveChanges();\n";
            body += "\t}\n\n";
            body += $"\tpublic void Remove( int id )"+ "{\n";
            body += $"\t\t_db.{Naming.ToCapitalStyle(table.name)}.Remove(this.Find(id));\n";
            body += $"\t\t_db.SaveChanges();\n";
            body += "\t}\n\n";
            body += $"\tpublic {Naming.ToCapitalStyle(table.singlecount_name)} Find( int id )";
            body += "\t{\n";
            body += $"\t\t return _db.{Naming.ToCapitalStyle(table.name)}.Find(id);\n";
            body += "\t}\n\n";
            body += $"\tpublic System.Collections.Generic.List<{Naming.ToCapitalStyle(table.singlecount_name)}> List(   )";
            body += "\t{\n";
            body += $"\t\t return _db.{Naming.ToCapitalStyle(table.name)}.ToList();\n";
            body += "\t}\n";            
            string classCode = header + "{\n" + body + "\n}";
            return classCode;
        }
    }
}
