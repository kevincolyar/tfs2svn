using System;
using System.IO;
using System.Text.RegularExpressions;
using Colyar.SourceControl.Tfs2Svn;

namespace Colyar.SourceControl
{
    class Program
    {
        private static readonly StreamWriter errorLog = new StreamWriter("error_log.txt"); 

        static void Main(string[] args)
        {
            // Testing (REMOVE FOR RELEASE)---------------
            args = new string[2];
            args[0] = @"https://tfs.dcpud.net/AMMPS";
            args[1] = @"file:///C:/svn/AMMPS";
            // ------------------------------------------

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
            Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter(tfsPath, svnPath, true);
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
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Usage:");
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine(">tfs2svn tfsPath svnPath");
            Console.WriteLine("Or");
            Console.WriteLine(">tfs2svn config.txt");
        }

        static void Report(int changeset, string path, string committer, string comment)
        {
            //Console.WriteLine("------------------------------------------------------------------------");
            //Console.WriteLine("Changeset: " + changeset);
            //Console.WriteLine("Path     : " + path);
            //Console.WriteLine("Committer: " + committer);
            //Console.WriteLine("Comment  : " + comment);
        }
        private static void HookupEventHandlers(Tfs2SvnConverter tfs2svnConverter)
        {
            tfs2svnConverter.BeginChangeSet += BeginChangeSet;
            tfs2svnConverter.EndChangeSet += EndChangeSet;
            tfs2svnConverter.FileAdded += FileAdded;
            tfs2svnConverter.FileDeleted += FileDeleted;
            tfs2svnConverter.FileEdited += FileEdited;
            tfs2svnConverter.FileRenamed += FileRenamed;
            tfs2svnConverter.FileBranched += FileBranched;
            tfs2svnConverter.FolderAdded += FolderAdded;
            tfs2svnConverter.FolderDeleted += FolderDeleted;
            tfs2svnConverter.FolderRenamed += FolderRenamed;
            tfs2svnConverter.FolderBranched += FolderBranched;

            tfs2svnConverter.SubversionConsoleOutput += tfs2svnConverter_SvnConsoleOutput;
            tfs2svnConverter.SubversionCommandError += tfs2svnConverter_SubversionCommandError;
        }

        static void tfs2svnConverter_SubversionCommandError(string input, string output, DateTime dateTime)
        {
            errorLog.WriteLine("----------------------------------------");
            errorLog.WriteLine(dateTime);
            errorLog.WriteLine(input);
            errorLog.WriteLine(output);
            errorLog.WriteLine("----------------------------------------");
        }
        private static Tfs2SvnConverter ParseConfigurationFile(string path)
        {
            string fileContents = new StreamReader(path).ReadToEnd();

            string svnPath = GetSvnPath(fileContents);
            string tfsPath = GetTfsPath(fileContents);
            bool overwrite = GetOverwriteOption(fileContents);
            Tfs2SvnConverter tfs2svnConverter = new Tfs2SvnConverter(tfsPath, svnPath, overwrite);

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
                tfs2svnConverter.AddUserMapping(match.Groups["regex"].Value, 
                                                match.Groups["username"].Value, 
                                                match.Groups["password"].Value);
        }

        #endregion

        #region Event Handlers

        static void BeginChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            Console.WriteLine("Begin Changeset: " + changeset);
        }
        static void EndChangeSet(int changeset, string committer, string comment, DateTime date)
        {
            Console.WriteLine("End Changeset: " + changeset);
        }
        static void FileAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        static void FileEdited(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        static void FileDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        static void FileBranched(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }
        static void FileRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }
        static void FolderAdded(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        static void FolderDeleted(int changeset, string path, string committer, string comment, DateTime date)
        {
            Report(changeset, path, committer, comment);
        }
        static void FolderBranched(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }
        static void FolderRenamed(int changeset, string oldPath, string newPath, string committer, string comment, DateTime date)
        {
            Report(changeset, newPath, committer, comment);
        }
        static void tfs2svnConverter_SvnConsoleOutput(string output)
        {
            Console.Write(output);
        }

        #endregion
    }
}
