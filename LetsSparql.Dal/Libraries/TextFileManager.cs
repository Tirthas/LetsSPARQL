using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LetsSparql.Common.Libraries
{
    public static class TextFileManager
    {
        /// <summary>
        /// Reads text from the given file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ReadTextFromFile(string fileName)
        {            
            var curFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LetsSparql", fileName);

            if (File.Exists(curFileName)) // If file already exists
            {
                string text = File.ReadAllText(curFileName);
                return text;                
            }
            else // If file does not exists
            {
                return null;          
            }
        }

        /// <summary>
        /// Writes text to the given file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public static void WriteTextToFile(string fileName, string content)
        {
            var curFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LetsSparql");
            if (!Directory.Exists(curFolder))
                Directory.CreateDirectory(curFolder);

            var curFileName = Path.Combine(curFolder, fileName);

            if (File.Exists(curFileName)) // If file already exists
            {
                File.WriteAllText(curFileName, String.Empty); // Clear file                
            }
            else // If file does not exists
            {
                File.Create(curFileName).Close(); // Create file               
            }
            using (StreamWriter sw = File.AppendText(curFileName))
            {
                sw.WriteLine(content); // Write text to .txt file
            }
        }
    }
}
