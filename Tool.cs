using SkiaSharp;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Net.NetworkInformation;

namespace TextToFontBMP
{
    internal class Tool
    {
        public static async Task TextToBitmap(string textInput, string fontPath, int fontSize)
        {
            var text = string.Join("", textInput.Distinct());
            var fontInfo = new FileInfo(fontPath);
            if (!fontInfo.Exists)
            {
                Console.WriteLine($"找不到字体文件:{fontInfo.FullName}");
                return;
            }
            var outPutDir = Path.Combine(fontInfo.DirectoryName ?? "/", "Output");
            if (!Directory.Exists(outPutDir))
            {
                Directory.CreateDirectory(outPutDir);
            }
            using var stream = File.OpenRead(fontPath);
            var typeface = SKTypeface.FromStream(stream);
            var paint = new SKPaint
            {
                Color = SKColors.Black, // 文本颜色
                Typeface = typeface,
                TextSize = fontSize, // 设置字体大小
                IsAntialias = false, // 开启抗锯齿

            };
            float textHeight = paint.FontMetrics.Descent * 2 - paint.FontMetrics.Ascent;
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            var taskList = new List<Task>();
            for (int i = 0; i < text.Length; i++)
            {
                var textItem = text[i];
            }
            foreach (var textItem in text)
            {


                taskList.Add(DrawText(textItem, outPutDir, fontSize, paint, textHeight));


            }
            await Task.WhenAll(taskList);
            Console.WriteLine($"用时:{sw.Elapsed.TotalMilliseconds}ms");
            Console.WriteLine($"总计:输入{textInput.Length}:去重:{text.Length}");
        }

        static Task DrawText(char textItem, string outPutDir, int fontSize, SKPaint paint, float textHeight)
        {
            var targetText = textItem.ToString();

            if (string.IsNullOrWhiteSpace(targetText)) return Task.CompletedTask;

            paint.TextAlign = SKTextAlign.Center;
            var imageInfo = new SKImageInfo(
            width: fontSize,
            height: fontSize * 2,
            colorType: SKColorType.Rgba8888,
            alphaType: SKAlphaType.Premul
            );
            using var surface = SKSurface.Create(imageInfo);
            using var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);
            canvas.Save();
            canvas.DrawText(targetText, fontSize / 2, textHeight / 2, paint);

            using SKImage image = surface.Snapshot();

            using SKBitmap bitImage = SKBitmap.FromImage(image);


            var bitData = bitImage.GetPixelSpan();
            int top = int.MaxValue, bottom = 0;
            for (int y = 0; y < bitImage.Height; y++)
            {
                for (int x = 0; x < bitImage.Width; x++)
                {
                    var pixelA = bitData[(y * bitImage.Width + x) * 4];

                    if (pixelA == 0)
                    {
                        top = Math.Min(top, y);
                        bottom = Math.Max(top, y);

                    }
                }
            }


            var offset = 0;
            if (bottom > fontSize)
            {
                offset = bottom - fontSize;
                offset = offset + (top - offset) / 2;
            }

            using var result = surface.Snapshot(new SKRectI { Left = 0, Right = fontSize, Top = offset, Bottom = fontSize + offset });

            using SKData data = result.Encode();

            var fileName = $"{textItem}_{Convert.ToInt64(textItem)}.png";
            using var imageStream = new FileStream(Path.Combine(outPutDir, fileName), System.IO.FileMode.Create);
            data.SaveTo(imageStream);

            //Console.WriteLine(fileName);
            return Task.CompletedTask;
        }
    }
}
