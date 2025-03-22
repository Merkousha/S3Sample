using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System;

class Program
{
    private const string ObjectKey = "uploaded-sample.pdf";        // کلید آبجکت (نام فایل)
    private const string FileName = "sample.pdf";               // نام فایل در مسیر جاری پروژه
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        string ServiceURL = configuration["ServiceURL"];
        string AccessKey = configuration["AccessKey"];
        string SecretKey = configuration["SecretKey"];
        string BucketName = configuration["BucketName"];
        
        try
        {
            var config = new AmazonS3Config
            {
                ServiceURL = ServiceURL, // تنظیم آدرس MinIO
                ForcePathStyle = true    // استفاده از Path-style برای MinIO
            };

            using var client = new AmazonS3Client(AccessKey, SecretKey, config);

            // تعیین مسیر فایل از مسیر جاری پروژه
            string filePath = Path.Combine("D:\\", FileName);

            // بررسی وجود فایل قبل از آپلود
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File '{FileName}' does not exist in the current directory.");
                return;
            }

            // ساخت درخواست آپلود
            var request = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = ObjectKey,      // نام فایل در باکت
                FilePath = filePath,  // مسیر فایل برای آپلود
                ContentType = "application/octet-stream" // نوع محتوا (می‌توانید بر اساس نوع فایل تغییر دهید)
            };

            // آپلود فایل
            var response = await client.PutObjectAsync(request);
            Console.WriteLine($"File uploaded successfully with status: {response.HttpStatusCode}");
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error encountered on server. Message:'{e.Message}'");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unknown error encountered. Message:'{e.Message}'");
        }
    }
}