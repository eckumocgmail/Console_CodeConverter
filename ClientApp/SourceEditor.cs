using System;
using System.Collections.Generic;

namespace ApplicationCore.Converter.ClientApp
{
    public class SourceEditor
    {
        private static string[] ClassModificators = new string[] { "abstract" };

        public Dictionary<string, string> names = null;
        protected  string dir = null;


        protected  SourceEditor(){
            this.names = new Dictionary<string, string>();
        }


        public SourceEditor( string sourceDir ){                      
            this.names = new Dictionary<string, string>();
            this.AddSourcesFiles( this.dir = sourceDir );
            this.AddAngularModules();
        }


        public Dictionary<string, string> Concat(Dictionary<string, string> d1, Dictionary<string, string> d2)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var p in d1)
            {
                result[p.Key] = p.Value;
            }
            foreach (var p in d2)
            {
                result[p.Key] = p.Value;
            }
            return result;
        }


        private void AddAngularModules()
        {
            foreach (string name in new string[] { "NgModuleRef","IterableChangeRecord", "IterableDiffers", "IterableDiffer", "PipeTransform","AfterContentInit", "AfterViewInit", "TransformPipe", "Pipe", "ComponentRef","DoCheck","ComponentRef", "TemplateRef", "ViewContainerRef", "SimpleChanges", "Injector", "ViewContainerRef", "ComponentFactoryResolver","Injector","ViewChild", "ViewChildren", "PlatformRef", "Inject", "Input", "Output", "ApplicationRef", "Type", "ErrorHandler", "Compiler", "Component", "Directive", "Injectable", "NgModule", "OnInit", "OnChanges", "OnDestroy", "AfterContentChecked", "AfterViewInit", "AfterViewChecked", "ElementRef", "EventEmitter" })
            {
                this.names[name] = "@angular/core";
            }
            foreach (string name in new string[] { "FormBuilder", "AsyncValidatorFn", "AsyncValidator", "ReactiveFormsModule","FormsModule","Validators", "FormControl", "FormGroup", "AbstractControl", "ValidationErrors" })
            {
                this.names[name] = "@angular/forms";
            }
            foreach (string name in new string[] { "Resolve","ActivatedRouteSnapshot", "RoutesRecognized", "RouterStateSnapshot", "UrlTree", "ActivatedRoute", "RouterEvent", "Routes", "RouteConfigLoadEnd", "ResolveStart", "ResolveEnd", "Route", "RouterPreloader", "CanDeactivate", "CanActivateChild", "CanActivate", "CanLoad", "Router", "RouterModule" })
            {
                this.names[name] = "@angular/router";
            }
            foreach (string name in new string[] { "BrowserAnimationsModule", "NoopAnimationsModule" })
            {
                this.names[name] = "@angular/platform-browser/animations";
            }
            foreach (string name in new string[] { "HubConnectionBuilder", "HubConnection", "LogLevel", "HttpTransportType", "HubConnectionState", "ILogger" })
            {
                this.names[name] = "@microsoft/signalr";
            }
            this.names["MatIconRegistry"] = "@angular/material/icon";
            this.names["DomSanitizer"] = "@angular/platform-browser";
            this.names["CommonModule"] = "@angular/common";
            this.names["BrowserModule"] = "@angular/platform-browser";
            this.names["HttpHeaders"] = "@angular/common/http";
            this.names["HttpClient"] = "@angular/common/http";
            this.names["HttpParams"] = "@angular/common/http";
            this.names["HttpUrlEncodingCodec"] = "@angular/common/http";
            this.names["HTTP_INTERCEPTORS"] = "@angular/common/http";
            this.names["HttpClientModule"] = "@angular/common/http";
            this.names["HttpErrorResponse"] = "@angular/common/http";
            this.names["HttpEvent"] = "@angular/common/http";
            this.names["HttpHandler"] = "@angular/common/http";
            this.names["HttpInterceptor"] = "@angular/common/http";
            this.names["HttpRequest"] = "@angular/common/http";
            this.names["Subscriber"] = "rxjs";            
            this.names["Observable"] = "rxjs";
            this.names["throwError"] = "rxjs";
            this.names["Location"] = "@angular/common";
            this.names["of"] = "rxjs";
            foreach (string name in new string[] { "map", "catchError", "every", "tap", "take", "delay", "debounce" })
            {
                this.names[name] = "rxjs/operators";
            }
            foreach (string name in new string[] { "trigger", "transition", "style", "animate" })
            {
                this.names[name] = "@angular/animations";
            }
            foreach (string name in new string[] { "getSupportedInputTypes", "supportsPassiveEventListeners", "supportsScrollBehavior", "Platform" })
            {
                this.names[name] = "@angular/cdk/platform";
            }
            Dictionary<string, string> material = new Dictionary<string, string>()
            {
                { "async","@angular/core/testing"},
                { "TestBed","@angular/core/testing"},
                { "ComponentFixture","@angular/core/testing"},
                { "A11yModule","@angular/cdk/a11y"},
                { "DragDropModule","@angular/cdk/drag-drop"},
                { "PortalModule","@angular/cdk/portal"},
                { "ScrollingModule","@angular/cdk/scrolling"},
                { "CdkStepperModule","@angular/cdk/stepper"},
                { "CdkTableModule","@angular/cdk/table"},
                { "CdkTreeModule","@angular/cdk/tree"},
                { "MatAutocompleteModule","@angular/material/autocomplete"},
                { "MatBadgeModule","@angular/material/badge"},
                { "MatBottomSheetModule","@angular/material/bottom-sheet"},
                { "MatButtonModule","@angular/material/button"},
                { "MatButtonToggleModule","@angular/material/button-toggle"},
                { "MatCardModule","@angular/material/card"},
                { "MatCheckboxModule","@angular/material/checkbox"},
                { "MatChipsModule","@angular/material/chips"},
                { "MatStepperModule","@angular/material/stepper"},
                { "MatDatepickerModule","@angular/material/datepicker"},
                { "MatDialogModule","@angular/material/dialog"},
                { "MatDividerModule","@angular/material/divider"},
                { "MatExpansionModule","@angular/material/expansion"},
                { "MatGridListModule","@angular/material/grid-list"},
                { "MatIconModule","@angular/material/icon"},
                { "MatInputModule","@angular/material/input"},
                { "MatListModule","@angular/material/list"},
                { "MatMenuModule","@angular/material/menu"},
                { "MatFormFieldModule","@angular/material/form-field"},
                { "MatPaginatorModule","@angular/material/paginator"},
                { "MatProgressBarModule","@angular/material/progress-bar"},
                { "MatProgressSpinnerModule","@angular/material/progress-spinner"},
                { "MatRadioModule","@angular/material/radio"},
                { "MatSelectModule","@angular/material/select"},
                { "MatSidenavModule","@angular/material/sidenav"},
                { "MatSliderModule","@angular/material/slider"},
                { "MatSlideToggleModule","@angular/material/slide-toggle"},
                { "MatSnackBarModule","@angular/material/snack-bar"},
                { "MatSortModule","@angular/material/sort"},
                { "MatTableModule","@angular/material/table"},
                { "MatTabsModule","@angular/material/tabs"},
                { "MatToolbarModule","@angular/material/toolbar"},
                { "MatTooltipModule","@angular/material/tooltip"},
                { "MatTreeModule","@angular/material/tree"},
                { "OverlayModule","@angular/cdk/overlay"},
                { "DragDrop","@angular/cdk/drag-drop"},
                { "Portal","@angular/cdk/portal"},
                { "CdkStepper","@angular/cdk/stepper"},
                { "CdkTable","@angular/cdk/table"},
                { "CdkTree","@angular/cdk/tree"},
                { "MatAutocomplete","@angular/material/autocomplete"},
                { "MatBadge","@angular/material/badge"},
                { "MatBottomSheet","@angular/material/bottom-sheet"},
                { "MatButton","@angular/material/button"},
                { "MatButtonToggle","@angular/material/button-toggle"},
                { "MatCard","@angular/material/card"},
                { "MatCheckbox","@angular/material/checkbox"},             
                { "MatStepper","@angular/material/stepper"},
                { "MatDatepicker","@angular/material/datepicker"},
                { "MatDialog","@angular/material/dialog"},
                { "MatDivider","@angular/material/divider"},            
                { "MatGridList","@angular/material/grid-list"},
                { "MatIcon","@angular/material/icon"},
                { "MatInput","@angular/material/input"},
                { "MatList","@angular/material/list"},
                { "MatMenu","@angular/material/menu"},
                { "MatRipple","@angular/material/core"},
                { "MatPaginator","@angular/material/paginator"},
                { "MatProgressBar","@angular/material/progress-bar"},
                { "MatProgressSpinner","@angular/material/progress-spinner"},
                { "MatSelect","@angular/material/select"},
                { "MatSidenav","@angular/material/sidenav"},
                { "MatSlider","@angular/material/slider"},
                { "MatSlideToggle","@angular/material/slide-toggle"},
                { "MatSnackBar","@angular/material/snack-bar"},
                { "MatSort","@angular/material/sort"},
                { "MatTable","@angular/material/table"},
                { "MatDialogRef","@angular/material/dialog"},
                { "MatToolbar","@angular/material/toolbar"},
                { "MatTooltip","@angular/material/tooltip"},
                { "MatTree","@angular/material/tree"},
                { "Overlay","@angular/cdk/overlay"}
            };
            this.names = Concat( this.names, material);
        }


        /// <summary>
        /// AddSourceFiles from directory
        /// </summary>
        /// <param name="path"></param>
        protected  void AddSourcesFiles(string path)
        {
            foreach( var p in this.GetSourcesFiles(path) )
            {
                this.names[p.Key] = p.Value;
            }
        }


        /// <summary>
        /// Search source files in path and parse names from each,
        /// then go to each subdirectory and do it for new path.
        /// </summary>
        /// <param name="path">path for source directory</param>
        /// <returns>dictionary with keyset of identifiers and values equals source filepath</returns>
        protected  Dictionary<string, string> GetSourcesFiles( string path )
        {
            Dictionary<string, string> sources = new Dictionary<string, string>();
            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                if (file.IndexOf("public-api.ts") != -1 || file.IndexOf("projects.ts") != -1) continue;
                if (file.IndexOf("material-modules.ts") != -1)
                {
                   
                }
                if (file.ToLower().EndsWith(".ts") == true)
                {
                    Console.WriteLine(file);           
                    string shortFilename = GetFileShortName(file);
                    string text = System.IO.File.ReadAllText(file);
                    List<string> ids = GetExportedNames(text);
                    foreach (string id in ids)
                    {
                        sources[id] = GetImportFromSection(file);
                    }
                    //string type = GetTypeFromShortFileName(shortFilename);
                    //string name = GetIdentifier(text, type);
                    //sources[name] = file;
                }
            }           
            return sources;
        }


        private List<string> GetExportedNames(string text)
        {     
            List<int> exportIndexes = GetExportIndexes(text);
            List<string> ids = new List<string>();
            foreach (int index in exportIndexes)
            {
                int i = index + 6;
                List<string> modificators = new List<string>();
                string afterExport, exportedType;

                afterExport = text.Substring(i);
                while (afterExport.StartsWith(" "))
                {
                    afterExport = afterExport.Substring(1);
                    i++;
                }
                exportedType = afterExport.Substring(0, GetFirstSeparatorIndex(afterExport));
                if (exportedType == "abstract")
                {
                    i += exportedType.Length;
                    modificators.Add(exportedType);
                    afterExport = text.Substring(i);
                    while (afterExport.StartsWith(" "))
                    {
                        afterExport = afterExport.Substring(1);
                        i++;
                    }
                    exportedType = afterExport.Substring(0, GetFirstSeparatorIndex(afterExport));
                }

                i += exportedType.Length;
                afterExport = text.Substring(i);
                while (afterExport.StartsWith(" "))
                {
                    afterExport = afterExport.Substring(1);
                    i++;
                }
                string id = afterExport.Substring(0, GetFirstSeparatorIndex(afterExport)).Trim();
                ids.Add(id);
            }
            return ids;
        }


        /// <summary>
        /// Search each export keyword in source
        /// </summary>
        /// <param name="text">source text</param>
        /// <returns>indexes of export keyword</returns>
        protected  List<int> GetExportIndexes(string text)
        {
            List<int> exportIndexes = new List<int>();
            int exportIndex = text.IndexOf("export ");
            if (exportIndex != -1)
            {
                exportIndexes.Add(exportIndex);
            }
            int startSearch = exportIndex + 1;
            while (startSearch != -1)
            {
                if(startSearch + 1 >= text.Length)
                {
                    break;
                }
                exportIndex = text.Substring(startSearch + 1).IndexOf("export ");
                if (exportIndex != -1)
                {
                    startSearch += exportIndex+1;
                    exportIndexes.Add(startSearch);
                }
                else
                {
                    break;
                }
            }
            return exportIndexes;
        }


        /// <summary>
        /// Parse first identifier from source text with type.
        /// </summary>
        /// <param name="text">Script of source</param>
        /// <param name="type">Searched type</param>
        /// <returns>declared identifier</returns>
        protected  string GetIdentifier(string text, string type)
        {
            string name = text.Substring(text.IndexOf(type + " ") + type.Length + 1);
            name = name.Substring(0, GetFirstSeparatorIndex(name));
            return name;
        }


        private static dynamic CODES = new
        {
            separators= new string[] { " ", ":", "(", "\n", "\r", "<", "{", ";", "=" }
        };


        /// <summary>
        /// Find first index of chart one of separators.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected  int GetFirstSeparatorIndex(string script)
        {
            int i=-1;
            int[] indexes = new int[] { script.IndexOf(" "),
                                        script.IndexOf(":"),
                                        script.IndexOf("("),
                                        script.IndexOf("\n"),
                                        script.IndexOf("\r"),
                                        script.IndexOf("<"),
                                        script.IndexOf(","),
                                        script.IndexOf("("),
                                        script.IndexOf("{"),
                                        script.IndexOf(";"), 
                                        script.IndexOf("=") };
            List<int> indexesList = new List<int>(indexes);
            indexesList.Sort();
            foreach( int index in indexesList)
            {
                if( index != -1)
                {
                    i = index;
                    break;
                }
            }            
            return i==-1? script.Length: i;
        }


        /// <summary>
        /// Parse filename and parse type between two comas.
        /// </summary>
        /// <example>
        /// GetTypeFromShortFileName("app.component.ts");
        /// <= component
        /// </example>
        /// <param name="shortFilename">short filename</param>
        /// <returns></returns>
        protected  string GetTypeFromShortFileName(string shortFilename)
        {
            int i1 = shortFilename.IndexOf(".");
            int i2 = shortFilename.LastIndexOf(".");
            string type = shortFilename.Substring(i1 + 1, i2 - i1 - 1);
            return type;
        }


        /// <summary>
        /// Return short filename from absolutely path.
        /// </summary>
        /// <param name="file">absolutely path of file.</param>
        /// <returns>short filename</returns>
        protected  string GetFileShortName(string file)
        {
            return file.Substring(Math.Max(file.LastIndexOf("/"), file.LastIndexOf("\\")) + 1);
        }


        public virtual int Replace(string s1,string s2)
        {
            int n = 0;
            Console.WriteLine("PatchImports: " + this.dir);
            foreach (string file in System.IO.Directory.GetFiles(this.dir))
            {
                string sourceFileText = System.IO.File.ReadAllText(file);

                List<string> ownNames = this.GetExportedNames(sourceFileText);
                if (file.ToLower().EndsWith(".ts") == true)
                {
                    string text = System.IO.File.ReadAllText(file);
                     
                    while( text.IndexOf(s1)!=-1 )
                    {
                        text = text.Replace(s1,s2);
                        n++;
                    }
                    System.IO.File.WriteAllText(file, text);
                }
            }
            return n;
        }


        /// <summary>
        /// Rewrite source files with correct imports
        /// </summary>
        protected  void PatchImports( Dictionary<string,string> globalNameSpace )
        {
            Console.WriteLine("PatchImports: "+this.dir);
            Dictionary<string, string> ns = this.CreateRelativeNameSpace( globalNameSpace );
            foreach (string file in System.IO.Directory.GetFiles(this.dir))
            {
                string sourceFileText = System.IO.File.ReadAllText(file);
                List<string> ownNames = this.GetExportedNames(sourceFileText);
                if (file.ToLower().EndsWith(".ts") == true)
                {                    
                    string text = "";
                    foreach (string line in System.IO.File.ReadAllText(file).Split("\n"))
                    {
                        string newline = line;
                        if (line.IndexOf("import ") == -1 && !String.IsNullOrEmpty(line))
                        {
                            text += line + "\n";
                        } 
                    }                    
                    string header = "";
                    if (sourceFileText.IndexOf("Highcharts") != -1)
                    {
                        header += "//import * as Highcharts from 'highcharts';\n";
                    }
                    foreach ( var pair in ns)
                    {
                        string name = pair.Key;
                        string source = pair.Value;
                        if (ownNames.Contains(name) ==true)
                        {
                            continue;
                        }                  
                        if ( text.IndexOf( name ) != - 1)
                        {
                            header += $"import " + "{" + name + "} from '" + source + "';\n";
                        }
                    }
                    string script = header + "\n" + text;
                    System.IO.File.WriteAllText(file, script);
                }
            }           
        }


        private Dictionary<string, string> CreateRelativeNameSpace(Dictionary<string, string> globalNameSpace)
        {
            Dictionary<string, string> ns = new Dictionary<string, string>();            
            foreach( var pair in globalNameSpace)
            {
                ns[pair.Key] = this.CreateRelativePath( this.dir, pair.Value);
            }
            return ns;
        }


        private string CreateRelativePath( string dir, string file)
        {
            if(file.StartsWith("rxjs")   ||file.StartsWith("@"))
            {
                return file;
            }
            string[] dirPaths = dir.Substring(@"A:\".Length).Split("\\");
            string[] filePaths = file.Substring(@"A:\".Length).Split("/");
            int i = 0;
            int n = Math.Min(dirPaths.Length, filePaths.Length);
            for ( i=0; i<n; i++)
            {
                if (!dirPaths[i].ToLower().Equals(filePaths[i].ToLower()))
                {
                    break;
                }
            }
            string res = ".";
            for ( int j=i; j<dirPaths.Length; j++)
            {
                res += "/..";
            }
            for (int j = i; j<filePaths.Length; j++)
            {
                res += "/" + filePaths[j];
            }
            return res;
        }


        /// <summary>
        /// Convert filepath for script format
        /// </summary>
        /// <param name="file">absolutely filepath</param>
        /// <returns></returns>
        protected  string GetImportFromSection(string file)
        {
            while (file.IndexOf(@"\")!=-1)
            {
                file = file.Replace(@"\","/");
            }
            //Console.WriteLine(file.Substring(this.dir.Length, file.LastIndexOf(".")));
            return file.Substring( 0, file.LastIndexOf("."));
        }


        protected  string GetImportFromSectionRelativeToDir(string file)
        {
            while (file.IndexOf(@"\") != -1)
            {
                file = file.Replace(@"\", "/");
            }
            //Console.WriteLine(file.Substring(this.dir.Length, file.LastIndexOf(".")));
            return file.Substring(0, file.LastIndexOf("."));
        }
    }
}
