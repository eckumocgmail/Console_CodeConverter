using System;
using System.Collections.Generic;
using System.Linq;

public class UserBusinessResourceSegmentGenerator
{
    protected string _dir;

    public UserBusinessResourceSegmentGenerator(string dir)
    {
        _dir = dir;
    }


    /// <summary>
    /// 
    /// <\summary>
    /// <param name="BusinessResourceName"><\param>
    public void Generate(  BusinessResource businessResource, 
        params 
        BusinessFunction[] businessFunctions ){
                        
        createDirectory(@$"{_dir}\Areas\{businessResource.Name}Face");
        createDirectory(@$"{_dir}\Areas\{businessResource.Name}Face\Controllers");
        createDirectory(@$"{_dir}\Areas\{businessResource.Name}Face\Views");
        createDirectory(@$"{_dir}\Areas\{businessResource.Name}Face\Services");
        createUserService(@$"{_dir}\Areas\{businessResource.Name}Face\{businessResource.Name}Service");
        createUserController(@$"{_dir}\Areas\{businessResource.Name}Face\Controllers\{businessResource.Name}Controller");
        createNavigationView(@$"{_dir}\Areas\{businessResource.Name}Face\Views\{businessResource.Name}\{businessResource.Name}Menu");
        createUserHome(@$"{_dir}\Areas\{businessResource.Name}Face\Views\{businessResource.Name}\{businessResource.Name}Home");
        createUserStats(@$"{_dir}\Areas\{businessResource.Name}Face\Views\{businessResource.Name}\{businessResource.Name}Stats");
    }



    /// <summary>
    /// 
    /// <\summary>
    /// <param name="BusinessResourceName"><\param>
    public void GenerateSegment(string BusinessResourceName, List<BusinessFunction> functions)
    {
        foreach(var function in functions)
        {
            var input = function.Input;
            

        }
    }




    private void createUserController(string filename)
    {
        System.IO.File.WriteAllText(filename, "");
    }

    private void createUserStats(string filename)
    {
    }
    private void createUserStats(string BusinessResourceName, List<BusinessFunction> functions)
    {
    }

    private void createUserHome(string filename)
    {
    }

    private void createNavigationView(string filename)
    {
    }

    private void createUserService(string filename)
    {
    }



    private void createDirectory(string relativePath)
    {
        string path =  relativePath;
        if(System.IO.Directory.Exists(path) == false)
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}