using System;
using System.Collections.Generic;

namespace Laboratory_Work_One
{
    class Program
    {

        public const string ORIGIN_PATH = @"Z:\UIRS\PHOTO";

        public const string RESULT_PATH = @"Z:\UIRS\CSV";
        public const string RESULTSIGNS_PATH = @"G:\J\8-й семестр\УИРС\csvLab1Stats";

        static void Main(string[] args)
        {
            Console.WriteLine("Началась обработка и запись изображений на диск! ...");

           // ImageProcessing.SelectOutlineImage($"{ORIGIN_PATH}\\Photo 9", $"{RESULT_PATH}\\Report 9", $"{RESULTSIGNS_PATH}\\Signs 9");

            
            List<List<List<double>>> listTestOfPoints = ImageProcessing.GetListPoin(new List<string>()
            {
                RESULTSIGNS_PATH + @"\Stats 1",
                RESULTSIGNS_PATH + @"\Stats 2",
                RESULTSIGNS_PATH + @"\Stats 3",
                RESULTSIGNS_PATH + @"\Stats 4",
                RESULTSIGNS_PATH + @"\Stats 5",
            }, "test");

            List<List<List<double>>> listCheckOfPoints = ImageProcessing.GetListPoin(new List<string>()
            {
                RESULTSIGNS_PATH + @"\Stats 1",
                RESULTSIGNS_PATH + @"\Stats 2",
                RESULTSIGNS_PATH + @"\Stats 3",
                RESULTSIGNS_PATH + @"\Stats 4",
                RESULTSIGNS_PATH + @"\Stats 5",
            }, "point");

            ImageProcessing.CheckClassPointOfList(listCheckOfPoints, listTestOfPoints);

            Console.WriteLine("Завершено!");

            Console.ReadKey();
        }
    }
}
