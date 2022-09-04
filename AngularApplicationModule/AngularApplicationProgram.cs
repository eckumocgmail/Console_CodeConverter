using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IAngularApplication
{
    public string Build();
    public string Build(string env);
    public string Test();
    public string Serve();
    public string Serve(int port);
}

public class AngularApplicationProgram: ConsoleProgram<AngularApplicationProgram>
{
    private readonly string _dir;

    public int _port { get; }

    private AngularApplicationProgram(string dir, int port, Action<string> OnMessage)
    {
        this._dir = dir;
        this._port = port;
    }
       
    internal static void Run(params string[] args)
    {
             
        if (args.Length == 0)
        {
            Console.WriteLine("Укажите путь к директории проекта angular");
            Console.Write(">");
            string path = @"D:\Projects\Angular_AppContainers";// Console.ReadLine();
            Run(path, 5003, Console.WriteLine);
           
        }
        else
        {
            foreach (string arg in args)                
                Run(arg, 4201, Console.WriteLine);                                
        }
        Thread.Sleep(Timeout.Infinite);
            
    }

    public static Task Run(string arg, int port, Action<string> OnMessage)
    {            
        return Process.GetCurrentProcess().Commit<Task>(() => {


            if (System.IO.Directory.Exists(arg) == false)
                throw new ArgumentException("Аргумент задан не правильно. " +
                    "В качестве параметров передавайте путь каталогу, который содержит проект Angular. " +
                    "Каталог не существует. ");

            if (System.IO.File.Exists(Path.Combine(arg, "angular.json")) == false)
                throw new ArgumentException("Аргумент задан не правильно. " +
                    "В качестве параметров передавайте путь каталогу, который содержит проект Angular. " +
                    "В каталоге отсутсвует файл angular.json");

            return Task.Run(() => {
                ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $"/C {arg.Substring(0, 2)} && cd \"{arg}\" && cd && ng serve --port {port}");

                info.RedirectStandardError = true;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
                var stream = process.StandardOutput;
                string readed;
                string message = "";
                                       
                while ((readed = stream.ReadLine()) != null)                     
                    OnMessage(readed);                 
            });                             
        });
            
    }
}
