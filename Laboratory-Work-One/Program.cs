using System;
using System.Collections.Generic;

namespace Laboratory_Work_One
{
    class Program
    {

        public const string ORIGIN_PATH = @"Z:\UIRS\PHOTO";

        public const string RESULT_PATH = @"Z:\UIRS\CSV";
        public const string RESULTSIGNS_PATH = @"Z:\UIRS\SIGNS";

        static void Main(string[] args)
        {
            Console.WriteLine("Началась обработка и запись изображений на диск! ...");

            ImageProcessing.SelectOutlineImage($"{ORIGIN_PATH}\\Photo 9", $"{RESULT_PATH}\\Report 9",$"{RESULTSIGNS_PATH}\\Signs 9");
            //ImageProcessing.SelectOutlineImage($"{ORIGIN_PATH}\\Photo 2", $"{RESULT_PATH}\\Report 2", $"{RESULTSIGNS_PATH}\\Signs 2");

            Console.WriteLine("Завершено!");

            Console.ReadKey();
        }
    }
}
