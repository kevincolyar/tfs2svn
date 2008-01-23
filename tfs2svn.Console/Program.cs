using System;
using System.IO;
using System.Text.RegularExpressions;
using Colyar.SourceControl.Tfs2Svn;
using tfs2svn.Console.Properties;

namespace tfs2svn.Console
{
    class Program
    {
        private static readonly StreamWriter errorLog = new StreamWriter("error_log.txt"); 

        static void Main(string[] args)
        {
#if DEBUG
            // Testing (REMOVE FOR RELEASE)---------------
            args = new string[2];
            args[0] = @"https://tfs.dcpud.net/AMMPS";
            args[1] = @"file:///C:/svn/AMMPS";
            // ------------------------------------------
#endif
            switch(args.Length)
            {
                case 1:
                    Convert(args[0]);
                    break;
                case 2:
                    Convert(args[0], args[1]);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

        #region Private Methods

        private static void Convert(string tfsPath, string svnPath)
        {
            string workingCopyPath = Path.GetTempPath() + "tfs2svn";
            //string svnBinFolder = @"C:\Program Files\Subversion\bin";
            string svnBinFolder = Settings.Default.SvnBinFolder;
            Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter(tfsPath, svnPath, true, 1, workingCopyPath, svnBinFolder, true);
            HookupEventHandlers(tfs2svnConverter);
            tfs2svnConverter.Convert();
        }

        private static void Convert(string configPath)
        {
            Tfs2SvnConverter tfs2svnConverter = ParseConfigurationFile(configPath);
            HookupEventHandlers(tfs2svnConverter);
            tfs2svnConverter.Convert();
        }

        private static void PrintUsage()
        {
            System.Console.WriteLine("--------------------------------------------");
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("--------------------------------------------");
            System.Console.WriteLine(">tfs2svn tfsPath svnPath");
            System.Console.WriteLine("Or");
            System.Console.WriteLine(">tfs2svn config.txt");
        }

        private static void HookupEventHandlers(Tfs2SvnConverter tfs2svnConverter)
        {
            tfs2svnConverter.BeginChangeSet += BeginChangeSet;
            tfs2svnConverter.EndChangeSet += EndChangeSet;
        }

        private static Tfs2SvnConverter ParseConfigurationFile(string path)
        {
            string fileContents = new StreamReader(path).ReadToEnd();

            string svnPath = GetSvnPath(fileContents);
            string tfsPath = GetTfsPath(fileContents);
            bool overwrite = GetOverwriteOption(fileContents);
            string workingCopyPath = Path.GetTempPath() + "tfs2svn";
            //string svnBinFolder = @"C:\Program Files\Subversion\bin";
            string svnBinFolder = Settings.Default.SvnBinFolder;
            Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter(tfsPath, svnPath, overwrite, 1, workingCopyPath, svnBinFolder, true);

            AddUserMappings(tfs2svnConverter, fileContents);

            return tfs2svnConverter;
        }
        private static string GetSvnPath(string fileContents)
        {
            Regex regex = new Regex(@"(?<svnpath>svnpath:\s+(?<path>[\w:\./\\]+))", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(fileContents))
                return match.Groups["path"].Value;

            return "";
        }
        private static string GetTfsPath(string fileContents)
        {
            Regex regex = new Regex(@"(?<tfspath>tfspath:\s+(?<path>[\w:\./\\]+))", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(fileContents))
                return match.Groups["path"].Value;

            return "";
        }
        private static bool GetOverwriteOption(string fileContents)
        {
            Regex regex = new Regex(@"(?<overwrite>overwrite:\s+(?<value>(true|false|0|1)))", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(fileContents))
                return bool.Parse(match.Groups["value"].Value);

            return true;
        }
        private static void AddUserMappings(Tfs2SvnConverter tfs2svnConverter, string fileContents)
        {
            Regex regex = new Regex(@"(?<usermapping>(usermapping:\s+(?<regex>\w+), (?<username>\w+), (?<password>\w+)))", RegexOptions.IgnoreCase);

            foreach (Match match in regex.Matches(fileContents))
                tfs2svnConverter.AddUsernameMapping(match.Groups["regex"].Value, match.Groups["username"].Value);
        }

        #endregion

        #region Event Handlers

        static void BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            System.Console.WriteLine("Begin Changeset: " + changeset);
        }
        static void EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            System.Console.WriteLine("End Changeset: " + changeset);
        }

        #endregion
    }
}
