using System.IO;

public static class FileUtility
{
    // Tries to read the file. If it fails, makes a copy of the file and tries to read that.
    public static string ReadFile(string path)
    {
        string fileContent = "";

        try
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                fileContent = streamReader.ReadToEnd();
            }
        }
        catch (IOException)
        {
            string tempFile = Path.GetFileNameWithoutExtension(path) + ".temp";

            if(File.Exists(tempFile))
                File.Delete(tempFile);

            File.Copy(path, tempFile);

            using(FileStream fileStream     = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using(StreamReader streamReader = new StreamReader(fileStream))
            {
                fileContent = streamReader.ReadToEnd();
            }

            File.Delete(tempFile);
        }

        return fileContent;
    }
}
