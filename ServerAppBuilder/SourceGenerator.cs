using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

public class SourceGenerator
{
   
    //razor component       
    public List<string> GenerateRazorPages(List<string> entities)
    {
        string contextName = "";
        List<string> commands = new List<string>();
        foreach (dynamic pair in entities)
        {
            string entity = pair.name;
            commands.Add($"dotnet aspnet-codegenerator razorpage Edit Edit -m {entity} -dc {contextName} -outDir Pages/{entity}");
            commands.Add($"dotnet aspnet-codegenerator razorpage Delete Delete -m {entity} -dc {contextName} -outDir Pages/{entity}");
            commands.Add($"dotnet aspnet-codegenerator razorpage List List -m {entity} -dc {contextName} -outDir Pages/{entity}");
            commands.Add($"dotnet aspnet-codegenerator razorpage Form Form -m {entity} -dc {contextName} -outDir Pages/{entity}");
            commands.Add($"dotnet aspnet-codegenerator controller -name {entity}Controller  --model {entity} --dataContext {contextName} --useDefaultLayout --readWriteActions");

        }
        return commands;
            
    }

     

    /// <summary>
    /// Выполнение инструкции через командную строку
    /// </summary>
    /// <param name="command"> команда </param>
    /// <returns></returns>
    private string Execute(string command)
    {
        ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C " + command);

        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        string response = process.StandardOutput.ReadToEnd();
        return response;
    }

    /// <summary>
    /// Выполнение инструкции через командную строку
    /// </summary>
    /// <param name="command"> команда </param>
    /// <returns></returns>
    private void Execute(string command, Func<string, int> listener)
    {
        Thread work = new Thread(new ThreadStart(() => {
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C " + command);

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
            string response = process.StandardOutput.ReadToEnd();
            listener(response);
        }));
        work.IsBackground = true;
        work.Start();
    }
}

