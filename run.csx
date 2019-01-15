public async static Task Run(QueueMessageModel queueMessage, string Id, TraceWriter log)
{

using (var imageStream = new MemoryStream())
        {
            // Open imageBlob from our Azure Storage
            imageBlob.DownloadToStream(imageStream);
            imageStream.Seek(0, SeekOrigin.Begin);

            // Check if CDR file
            if (queueMessage.Extension.ToLower() == "cdr")
            {
                var libCdrRootPath = @"D:\home\libcdr";
                var libCdrExe = @"cdr2xhtml.exe";
                var libCdrExePath = Path.Combine(libCdrRootPath, libCdrExe);
                var tempDirectoryName = @"temp";
                var tempFilePath = Path.Combine(libCdrRootPath, tempDirectoryName);

                // Save CDR to temp folder
                EnsureTempFileExists(tempFilePath);
                var guidString = Guid.NewGuid().ToString();
                var cdrFilePath = Path.ChangeExtension(Path.Combine(tempFilePath, guidString), ".cdr");
                var fileStream = File.Create(cdrFilePath);
                versionStream.CopyTo(fileStream);
                fileStream.Close();
                versionStream.Seek(0, SeekOrigin.Begin);

                // Call cdr2xhtml.exe and convert to SVG
                var svgFilePath = Path.ChangeExtension(cdrFilePath, ".svg");
                using (var process = new System.Diagnostics.Process())
                {
                    process.StartInfo.FileName = libCdrExePath;
                    process.StartInfo.Arguments = cdrFilePath;
                    process.StartInfo.WorkingDirectory = libCdrRootPath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();

                    log.Info(libCdrExePath);
                    log.Info(cdrFilePath);

                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    log.Info(output);  
                    
                    var exitCode = process.ExitCode;
                    log.Info(exitCode.ToString());
                    if (exitCode != 0)                    
                    {
                        string err = process.StandardError.ReadToEnd();
                        log.Info(err);
                    }
                }

                // Clean up temp files
                File.Delete(cdrFilePath);
            }
}

public class QueueMessageModel
{
    public string Company { get; set; }
    public string Name { get; set; }
    public string Extension { get;set; }
    public string Path { get;set; }
}