using System;

namespace ApplicationCore.Converter.ClientApp
{
    public class ClientAppBuilder: SourceEditor
    {

        private static ClientAppBuilder global = new ClientAppBuilder();

        private AppModule app; 
        private AppModule core;
        private AppModule components;
        private AppModule pages;
        private AppModule projects;
         

        protected  ClientAppBuilder() : base()
        {
        }


        public ClientAppBuilder( string dir ) : base()
        {
            this.dir = dir;
            while(!System.IO.File.Exists(dir + @"\angular.json"))
            {
                Console.WriteLine(dir + @"\angular.json");
                if (dir.LastIndexOf(@"\") == -1)
                {
                    throw new Exception("angular.json not found");
                }
                dir = dir.Substring(0,dir.LastIndexOf("\\"));
            }
            this.app = new AppModule(dir + @"\src\app",this);
            this.projects = new AppModule(dir + @"\projects", this);
            this.app.import(this.projects);
            this.core = new AppModule(dir + @"\src\app-core",this);   
            this.components = new AppModule(dir + @"\src\app-components",this);
            this.components.import(this.core);
            this.pages=new AppModule(dir + @"\src\app-pages",this);
            this.pages.import(this.components);
            this.app.import(this.pages);
            this.names = this.Concat(this.names, this.app.GetImportedNames());  
            this.AddSourcesFiles(dir + @"\app");
        }


        public override int Replace(string oldName, string newName)
        {
            return this.app.Replace( oldName, newName );
        }


        public void PatchImports()
        {
            this.app.PatchImports();
        }
    }
}
