using System.IO.Compression;
using System.Text;

namespace BB84.EPG.Downloader;

internal class Program
{
	private static void Main(string[] args)
	{
		if (args.Length != 1)
		{
			Console.WriteLine("Usage: BB84.EPG.Downloader.exe <download_path>");
			return;
		}

		if (Directory.Exists(args[0]) == false)
		{
			Console.WriteLine($"The specified directory does not exist: {args[0]}");
			return;
		}

		string downloadPath = args[0];

		DownloadEPG("https://iptv-epg.org/files/epg-de.xml", downloadPath);
	}

	private static void DownloadEPG(string epgUrl, string downloadPath)
	{
		using HttpClient httpClient = new();
		string epgFileName = epgUrl.Split('/')
			.Last()
			.Replace("-de", string.Empty);
		string epgFilePath = Path.Combine(downloadPath, epgFileName + ".gz");

		string epgContent = httpClient.GetStringAsync(epgUrl)
			.GetAwaiter()
			.GetResult();

		epgContent = epgContent.Replace("DE - ", string.Empty);

		byte[] bytes = Encoding.UTF8.GetBytes(epgContent);

		using MemoryStream inputStream = new(bytes);
		using FileStream outputStream = File.Create(epgFilePath);
		using GZipStream gzip = new(outputStream, CompressionMode.Compress);

		inputStream.CopyTo(gzip);

		inputStream.Close();
		gzip.Close();
		outputStream.Close();
	}
}
