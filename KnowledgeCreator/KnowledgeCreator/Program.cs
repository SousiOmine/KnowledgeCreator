using System;
using System.IO;
using System.Text;

class Program
{
	static void Main(string[] args)
	{
		string folderPath;

		if (args.Length > 0)
		{
			folderPath = args[0];
		}
		else
		{
			Console.Write("フォルダパスを入力してください: ");
			folderPath = Console.ReadLine();
		}

		if (Directory.Exists(folderPath))
		{
			StringBuilder markdownContent = new StringBuilder();
			ProcessDirectory(folderPath, markdownContent);

			string outputFilePath = Path.Combine(folderPath, "merged_content.md");
			File.WriteAllText(outputFilePath, markdownContent.ToString());
			Console.WriteLine($"Markdownファイルを生成しました: {outputFilePath}");
		}
		else
		{
			Console.WriteLine("指定されたフォルダが存在しません。");
		}
	}

	static void ProcessDirectory(string targetDirectory, StringBuilder markdownContent)
	{
		string[] fileEntries = Directory.GetFiles(targetDirectory);
		foreach (string fileName in fileEntries)
		{
			ProcessFile(fileName, markdownContent);
		}

		string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
		foreach (string subdirectory in subdirectoryEntries)
		{
			ProcessDirectory(subdirectory, markdownContent);
		}
	}

	static void ProcessFile(string filePath, StringBuilder markdownContent)
	{
		if (IsBinaryFile(filePath))
		{
			return;
		}

		markdownContent.AppendLine($"## {filePath}");
		markdownContent.AppendLine("```");
		markdownContent.AppendLine(File.ReadAllText(filePath));
		markdownContent.AppendLine("```");
	}

	static bool IsBinaryFile(string filePath)
	{
		using (FileStream stream = File.OpenRead(filePath))
		{
			const int SampleSize = 64;
			byte[] buffer = new byte[SampleSize];
			int bytesRead = stream.Read(buffer, 0, SampleSize);
			for (int i = 0; i < bytesRead; i++)
			{
				if (buffer[i] > 0x7F && buffer[i] < 0x20 && buffer[i] != 0x09 && buffer[i] != 0x0A && buffer[i] != 0x0D)
				{
					return true;
				}
			}
		}
		return false;
	}
}
