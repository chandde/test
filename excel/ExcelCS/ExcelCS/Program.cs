using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Drawing;

//class Ratio
//{
//    public Ratio(string a, double b, double c)
//    {
//        desc = a;
//        width = b;
//        height = c;
//    }

//    public string desc;
//    public double width;
//    public double height;
//}

namespace ExcelCS
{
    class Program
    {
        static void Main(string[] args)
        {
            Tuple<string, double, double>[] ratios = {
                Tuple.Create("strange ratio, unchanged", 1.0, 1000.0), // intentionally large y, so anything falls into this and 1:1.91 would favor 1:1.91
                Tuple.Create("1 x 1.91", 1.0, 1.91),
                Tuple.Create("9 x 16", 9.0, 16.0),
                Tuple.Create("3 x 4", 3.0, 4.0),
                Tuple.Create("1 x 1", 1.0, 1.0),
                Tuple.Create("4 x 3", 4.0, 3.0),
                Tuple.Create("16 x 9", 16.0, 9.0),
                Tuple.Create("1.91 x 1", 1.91, 1.0),
                Tuple.Create("", 1000.0, 1.0) // intentionally large x, so anything falls into this and 1.91:1 would faovr 1.91:1
            };

            var allLines = File.ReadAllLines(@"D:\git\misc\excel\10000 Images.csv");
            using (var missed = File.AppendText(@"D:\git\misc\excel\10000 Images_1_Missed.csv"))
            {
                using (var target = File.AppendText(@"D:\git\misc\excel\10000 Images_1.csv"))
                {

                    // skip 1st line
                    for (int i = 1; i < allLines.Length; ++i)
                    {
                        // to avoid ddos
                        // Thread.Sleep(50);

                        var line = allLines[i];
                        var cells = line.Split(",");
                        var output = new List<string>(cells);

                        try
                        {

                            byte[] imageData = new WebClient().DownloadData(cells[2]);
                            MemoryStream imgStream = new MemoryStream(imageData);
                            var img = Image.FromStream(imgStream);

                            int width = img.Width;
                            int height = img.Height;

                            for (var index = 1; index < ratios.Length; index++)
                            {
                                if (width * ratios[index].Item3 /* height */ == height * ratios[index].Item2 /* width */)
                                {
                                    // found a perfect match, no adjustment needed
                                    var newWidth = width;
                                    var newHeight = height;
                                    var newUrl = $"{cells[2]}&w={newWidth}&h={newHeight}";
                                    output.AddRange(new List<string> { width.ToString(), height.ToString(), ratios[index].Item1, newWidth.ToString(), newHeight.ToString(), newUrl });
                                    break;

                                }
                                else if (width * ratios[index].Item3 /* height */ > height * ratios[index].Item2 /* width */)
                                {
                                    continue;

                                }
                                else
                                {
                                    // compare current ratio against last one
                                    var ratio = (double)width / height;
                                    if (Math.Abs(ratio - ratios[index].Item2 / ratios[index].Item3) < Math.Abs(ratio - ratios[index - 1].Item2 / ratios[index - 1].Item3))
                                    {
                                        // use index, keep width, adjust height
                                        var newWidth = width;
                                        var newHeight = Math.Round(width * ratios[index].Item3 / ratios[index].Item2);
                                        var newUrl = $"{cells[2]}&w={newWidth}&h={newHeight}";
                                        output.AddRange(new List<string> { width.ToString(), height.ToString(), ratios[index].Item1, newWidth.ToString(), newHeight.ToString(), newUrl });
                                    }
                                    else
                                    {
                                        if (index == 1)
                                        {
                                            // strange images that favor the 1:1000 ratio, e.g. original ratio is 1:4 (closer to 0 comparing with 1:1.91)
                                            // for these we do not change the ratio, output as is
                                            var newWidth = width;
                                            var newHeight = height;
                                            var newUrl = cells[2];
                                            output.AddRange(new List<string> { width.ToString(), height.ToString(), ratios[index - 1].Item1 });
                                        }
                                        else
                                        {
                                            // use index - 1, keep height, adjust width
                                            var newWidth = Math.Round(height * ratios[index - 1].Item2 / ratios[index - 1].Item3);
                                            var newHeight = height;
                                            var newUrl = $"{cells[2]}&w={newWidth}&h={newHeight}";
                                            output.AddRange(new List<string> { width.ToString(), height.ToString(), ratios[index - 1].Item1, newWidth.ToString(), newHeight.ToString(), newUrl });
                                        }
                                    }
                                    break;

                                }
                            }
                            Console.WriteLine(i);
                            Console.WriteLine(String.Join(',', output));
                            target.WriteLine(String.Join(',', output));
                            target.Flush();
                        }
                        catch (Exception e)
                        {
                            Console.Write(e.ToString());
                            Console.WriteLine(String.Join(',', output));
                            missed.WriteLine(String.Join(',', output));
                            missed.Flush();
                        }
                    }

                    Console.WriteLine("Hello World!");
                }
            }
        }
    }
}
