// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Immutable;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.XObjects;
using static System.Net.Mime.MediaTypeNames;
using static UglyToad.PdfPig.Core.PdfSubpath;

string pdfFilePath =    $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\target\\2024-stu\\pictures\\test.pdf";
string targetFolderPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\target\\2024-stu\\pictures";
string targetFolderPath1 = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\target\\2024-stu\\pictures\\student.csv";
string targetFolderPath2 = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\target\\2024-stu\\pictures\\OmittedPages.csv";
string targetFolderPath3 = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\target\\2024-stu\\pictures\\Name.csv";

//  rename the file name for proper recognation
string targetFolderPath4 = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\2024-stu\\Name-Extrated-from-pdf.csv";
string txtFilePath1 = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\2024-stu\\SPECIAL STUDENT LIST.csv";


//string text1 = File.ReadAllText(txtFilePath1);

//string[] sentences1 = text1.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//var newsentence = new List<string>();


//foreach (string sentence in sentences1)
//{
//    if (!sentence.Contains(",,,,,,,,,,,,,,,,,,"))
//    {
//        newsentence.Add(sentence);
//        //Console.WriteLine(string.Join(",", sentence));
//        File.WriteAllLines(targetFolderPath4, newsentence);
//    }
  

//}


try
{
    using (PdfDocument pdfDocument = PdfDocument.Open(pdfFilePath))
    {
        List<string> omittedPages = new List<string>();
        List<string> texts = new List<string>();
        List<string> regNum = new List<string>();
        List<string> names = new List<string>();
     
        var imag = "";
        var img = 0;
        // int imageCount = 50715
        foreach (Page page in pdfDocument.GetPages())
        {
            var text = ContentOrderTextExtractor.GetText(page, true);
            int num = text.IndexOf("Student Image");
            var small = text.Remove(0, num);
               var textses = small.Replace("Student Image", " ").Replace("\r\n", ",").Trim().Split(',').ToList();
                texts = small.Replace("Student Image", " ").Replace("\r\n", " ").Trim().Replace("  ", ",").Split(',').ToList();
              var  textss = small.Replace("Student Image", " ").Replace("\r\n", " ").Trim().Replace("  ", ",").Split(',').ToList();
                texts.RemoveAt(texts.Count - 1);
            foreach (var item in textss)
            {
                if (item.Contains("NCA/"))
                {
                    names.Add(item);
                }
               
            }

            List<XObjectImage> images = page.GetImages().Cast<XObjectImage>().ToList();
         
            if (page.Number == 1 && images.Count != texts.Count)
            {
                images.RemoveAt(0);
            }

            if (images.Count == texts.Count)
            {

                foreach (var (x, y) in texts.Zip(images))
                {
                    if (x.Contains('/'))
                    {

                        var xValue = x.Trim();
                        var result = xValue.Split(' ');
                        imag = result[0].Replace('/', '_');
                        byte[] imageRawBytes = y.RawBytes.ToArray();
                        using (FileStream stream = new FileStream($"{targetFolderPath}\\{imag}.jpg", FileMode.Create, FileAccess.Write))
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(imageRawBytes);
                            writer.Flush();
                        }
                        regNum.Add(imag);
                    }

                }

            }
            else
            {
               
                foreach (XObjectImage item in images)
                {
                    
                    byte[] imageRawBytes = item.RawBytes.ToArray();
                    using (FileStream stream = new FileStream($"{targetFolderPath}\\{img}.jpg", FileMode.Create, FileAccess.Write))
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(imageRawBytes);
                        writer.Flush();
                    }
                  
                    img++;
                }
                
                omittedPages.Add("Page: " + page.Number.ToString());
                
            }

        }

        File.WriteAllLines(targetFolderPath1, regNum);
        File.WriteAllLines(targetFolderPath2,omittedPages);
        File.WriteAllLines(targetFolderPath3, names);
      
        Console.WriteLine($"{regNum.Count} File written successfully!!!");
        Console.WriteLine($"{regNum.Count} Picture exported Successfully !!!");
        Console.WriteLine($"{omittedPages.Count} pages were skip due to irregularities!!!");
    }

}
catch (Exception e)
{
    var message = e.Message;
    Console.WriteLine(message);


}




















