using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;

namespace Laboratory_Work_One
{
    static class ImageProcessing
    {
        public const int WIDTH = 1024;
        public const int HEIGHT = 1024;

        public const int START_RANGE_X = 400;
        public const int END_RANGE_X = 600;

        public const int START_RANGE_Y = 400;
        public const int END_RANGE_Y = 600;

        public const string PATH = @"C:\Users\toni_\OneDrive\Рабочий стол\Исходные объекты\";
        public const string RESULT_PATH = @"C:\Users\toni_\OneDrive\Рабочий стол\Результ\";

        public static void PerformConversion(string originNameFile, string outNameFolder)
        {
            var image = GetImageFromPath(PATH + originNameFile);

            Image<Gray, byte> greyImage = ToGray(image);
            greyImage = GetMedianFilter(greyImage);

            Image<Gray, byte> binarizedImage = GetBinarizedImage(greyImage);

            Gray color = new Gray(255);
            Gray black = new Gray(0);

            for (int i = 0; i < greyImage.Height; i++)
            {
                for (int j = 0; j < greyImage.Width; j++)
                {
                    if (!binarizedImage[j, i].Equals(black))
                    {
                        greyImage[j, i] = color;
                    }
                }
            }

            Directory.CreateDirectory(RESULT_PATH + outNameFolder);

            for (int i = 0; i <= 360; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    var random = new Random();
                    var movedToX = random.Next(START_RANGE_X, END_RANGE_X);
                    var movedToY = random.Next(START_RANGE_Y, END_RANGE_Y);

                    var resultImage = GetMovedImage(Rotate(greyImage, i), movedToX, movedToY);

                    resultImage.Save($"{RESULT_PATH}{outNameFolder}\\{i}-{movedToX}-{movedToY}.png");
                }
            }
        }

        private static Bitmap GetMovedImage(Image<Gray, byte> img, int movedToX, int movedToY)
        {
            Gray black = new Gray(0);

            Image<Gray, byte> binarizedImage = GetBinarizedImage(img);

            var sumX = 0;
            var sumY = 0;
            var count = 0;

            var originalShapeParams = new ShapeParams(WIDTH, HEIGHT);

            for (int width = 0; width < img.Width; width++)
            {
                for (int height = 0; height < img.Height; height++)
                {
                    if (binarizedImage[row: width, column: height].Equals(black))
                    {
                        count++;
                        sumX += width;
                        sumY += height;

                        originalShapeParams.SetShapeParams(byWidth: width, byHeight: height);
                    }
                }
            }

            var centerX = sumX / count;
            var centerY = sumY / count;

            var oldBitmap = img.Bitmap;
            var newBitmap = new Bitmap(WIDTH, HEIGHT);

            using (Graphics g = Graphics.FromImage(newBitmap)) { g.Clear(Color.White); }

            for (int width = originalShapeParams.Top; width <= originalShapeParams.Bottom; width++)
            {
                for (int height = originalShapeParams.Left; height <= originalShapeParams.Right; height++)
                {
                    var calcWidth = width + -(centerX - movedToX);
                    var calcHeight = height + -(centerY - movedToY);

                    var isOverRight = calcWidth >= WIDTH;
                    var isOverLeft = calcWidth < 0;

                    var isOverTop = calcHeight < 0;
                    var isOverBottom = calcHeight >= HEIGHT;

                    if (isOverRight || isOverLeft || isOverTop || isOverBottom) break;

                    newBitmap.SetPixel(calcWidth, calcHeight, oldBitmap.GetPixel(width, height));
                }
            }

            return newBitmap;
        }

        public static Image<Bgr, byte> GetImageFromPath(string path)
        {
            return new Image<Bgr, byte>(path);
        }

        public static Image<Gray, byte> ToGray(Image<Bgr, byte> img)
        {
            return img.Convert<Gray, byte>();
        }

        public static Image<Gray, byte> GetMedianFilter(Image<Gray, byte> img)
        {
            CvInvoke.MedianBlur(img, img, 5);

            return img;
        }

        public static Image<Gray, byte> GetBinarizedImage(Image<Gray, byte> img)
        {
            return img.ThresholdBinary(new Gray(150), new Gray(250));
        }

        public static Image<Gray, byte> Rotate(Image<Gray, byte> img, double angle)
        {
            return img.Rotate(angle, new Gray(255));
        }
    }
}
