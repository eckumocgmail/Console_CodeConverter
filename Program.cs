using System;

namespace Console_CodeConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var angular = new ConverterAngular();
            angular.CreateServices(new MyApplicationModel());
        }
    }
}
