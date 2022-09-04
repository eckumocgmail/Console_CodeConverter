using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IHtmlGenerator
{
    public string Form<T>();
    public string Table<T>();
    public string List<T>();
    public string Split(string left, string right);
}

