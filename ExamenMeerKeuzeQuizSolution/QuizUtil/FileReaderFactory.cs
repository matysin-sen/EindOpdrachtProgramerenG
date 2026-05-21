using MeerKeuzeBL.Interface;
using MeerKeuzeDL.FileReader;

namespace QuizUtil
{
    public class FileReaderFactory
    {
        public static IFileReader CreateFileReader(string sourceFilePath, string sourceFileType, string errorLogPath)
        {
            switch (sourceFileType.Trim().ToUpper())
            {
                    case "TXT":
                    case "TXT_ACHTER":
                    {// De constructor heeft geen argumenten nodig; het pad wordt in .Read(pad) gebruikt
                        return new FileReaderAntwoordAchterVraag();
                    }
                    case "TXT_ONDER":
                    {
                        return new FileReaderAntwoordOnder();
                    } 
                    default:
                    {
                        throw new Exception("File type not supported.");
                    }
            }
        }
    }
}
