using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Drawing;
using System.Web;
using System.Threading.Tasks;

namespace ExcelCS
{
    class Program
    {
        static Tuple<string, double, double>[] ratios = {
                Tuple.Create("lower bound", 1.0, 9999.0), // intentionally large height
                Tuple.Create("1 x 1.91", 1.0, 1.91),
                Tuple.Create("9 x 16", 9.0, 16.0),
                Tuple.Create("3 x 4", 3.0, 4.0),
                Tuple.Create("1 x 1", 1.0, 1.0),
                Tuple.Create("4 x 3", 4.0, 3.0),
                Tuple.Create("16 x 9", 16.0, 9.0),
                Tuple.Create("1.91 x 1", 1.91, 1.0),
            };

        static void cropHeightAndPadWidth(int width, int height, int index, string[] cells, ref List<string> output)
        {
            // pad width
            var paddingHeight = height;
            var paddingWidth = Math.Round(height * ratios[index].Item2 / ratios[index].Item3);

            // for padding scenario we need to remove the "c" query param from the url
            var paddingUrl = new UriBuilder($"{cells[2]}&w={paddingWidth}&h={paddingHeight}");
            var newQueryString = HttpUtility.ParseQueryString(paddingUrl.Query);
            newQueryString.Remove("c");
            var newPaddingUrl = paddingUrl.Uri.AbsoluteUri.Substring(0, paddingUrl.Uri.AbsoluteUri.IndexOf("?")) + "?" + newQueryString.ToString();

            // crop heigth
            var croppingWidth = width;
            var croppingHeight = Math.Round(width * ratios[index].Item3 / ratios[index].Item2);
            var croppingUrl = $"{cells[2]}&w={croppingWidth}&h={croppingHeight}";

            output.AddRange(new List<string> {
                width.ToString(),
                height.ToString(),
                ratios[index].Item1,
                paddingWidth.ToString(),
                paddingHeight.ToString(),
                newPaddingUrl,
                croppingWidth.ToString(),
                croppingHeight.ToString(),
                croppingUrl
            });
        }

        static void padHeightAndCropWidth(int width, int height, int index, string[] cells, ref List<string> output)
        {
            // pad heigth
            var paddingWidth = width;
            var paddingHeight = Math.Round(width * ratios[index].Item3 / ratios[index].Item2);

            // for padding scenario we need to remove the "c" query param from the url
            var paddingUrl = new UriBuilder($"{cells[2]}&w={paddingWidth}&h={paddingHeight}");
            var newQueryString = HttpUtility.ParseQueryString(paddingUrl.Query);
            newQueryString.Remove("c");
            var newPaddingUrl = paddingUrl.Uri.AbsoluteUri.Substring(0, paddingUrl.Uri.AbsoluteUri.IndexOf("?")) + "?" + newQueryString.ToString();

            // crop width
            var croppingHeight = height;
            var croppingWidth = Math.Round(height * ratios[index].Item2 / ratios[index].Item3);
            var croppingUrl = $"{cells[2]}&w={croppingWidth}&h={croppingHeight}";

            output.AddRange(new List<string> {
                width.ToString(),
                height.ToString(),
                ratios[index].Item1,
                paddingWidth.ToString(),
                paddingHeight.ToString(),
                newPaddingUrl,
                croppingWidth.ToString(),
                croppingHeight.ToString(),
                croppingUrl
            });
        }

        static void noCropOrPad(int width, int height, int index, string[] cells, ref List<string> output, string customMsg)
        {
            var newWidth = width;
            var newHeight = height;
            var newUrl = $"{cells[2]}&w={newWidth}&h={newHeight}"; // not used
            output.AddRange(new List<string> { width.ToString(), height.ToString(), ratios[index].Item1, customMsg });
        }

        static void Main(string[] args)
        {
            var allLines = File.ReadAllLines(@"D:\excel\10000 Images.csv");
            using (var target = File.AppendText(@"D:\excel\10000 Images_Processed.csv"))
            {
                target.Write("AdType,MSANTemplateId,ImageUrl,OriginalWidth,OriginalHeight,FitToRatio,PaddingWidth,PaddingHeight,PaddingUrl,CroppingWidth,CroppingHeight,CroppingUrl\r\n");
                target.Flush();

                for(int i = 0; i < allLines.Length; ++i)
                {
                    Console.WriteLine(i);
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

                        // when a image does match desired ratio, two possible solution
                        // solution #1. image is narrower than ratio, either pad width, or crop height
                        // solution #2. image is wider than ratio, either crop width, or pad height

                        // special cases:
                        // #1 ratios smaller than 1:1.91, always fix them to 1:1.91
                        // #2 ratios larger than 1.91:1, always fix them to 1.91:1

                        var index = 1;

                        for (; index < ratios.Length; index++)
                        {
                            if (Math.Ceiling(width * ratios[index].Item3) /* height */ == Math.Ceiling(height * ratios[index].Item2) /* width */)
                            {
                                noCropOrPad(width, height, index, cells, ref output, "Perfect ratio no need to crop or pad");
                                break;

                            }
                            else if (Math.Ceiling(width * ratios[index].Item3) /* height */ > Math.Ceiling(height * ratios[index].Item2) /* width */)
                            {
                                continue;
                            }
                            else
                            {
                                if (index == 1)
                                {
                                    // special case #1, ratio smaller than 1:1.91
                                    cropHeightAndPadWidth(width, height, index, cells, ref output);
                                    break;
                                }

                                // compare current ratio against last one
                                var ratio = (double)width / height;
                                if (Math.Abs(ratio - ratios[index].Item2 / ratios[index].Item3) < Math.Abs(ratio - ratios[index - 1].Item2 / ratios[index - 1].Item3))
                                {
                                    // ratio is on the left of index, image is taller than ratio, solution #1
                                    cropHeightAndPadWidth(width, height, index, cells, ref output);
                                }
                                else
                                {
                                    // ratio is on the right of index-1, image is shorter than ratio, solution #2
                                    padHeightAndCropWidth(width, height, index - 1, cells, ref output);
                                }
                                break;
                            }
                        }

                        if (index == ratios.Length)
                        {
                            // special case #2, ratio is larger than 1.91:1
                            padHeightAndCropWidth(width, height, index - 1, cells, ref output);
                        }

                        Console.WriteLine(String.Join(',', output));
                        target.WriteLine(String.Join(',', output));
                        target.Flush();
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.ToString());
                        output.Add("Exception when retrieving image size mostly due to bad image link");
                        Console.WriteLine(String.Join(',', output));
                        target.WriteLine(String.Join(',', output));
                        target.Flush();
                    }
                }
            }
        }
    }
}