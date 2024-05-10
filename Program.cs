// See https://aka.ms/new-console-template for more information
using TextToFontBMP;
Console.WriteLine($"参数1[字体文件]::{args.Skip(0).FirstOrDefault()}");
Console.WriteLine($"参数2[字表文件]::{args.Skip(1).FirstOrDefault()}");
Console.WriteLine($"参数3[图片大小]::{args.Skip(2).FirstOrDefault()}");

if (args.Length < 3)
{
    Console.WriteLine("参数不够");
    Console.ReadLine();
    return;

}
else
{
    await Tool.TextToBitmap(File.ReadAllText(args[1]), args[0], int.Parse(args[2]));

}
