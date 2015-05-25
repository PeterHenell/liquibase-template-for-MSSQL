using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedgateToLiquibase
{
    class Program
    {
        public bool OverwriteExistingMasterFiles { get; private set; }
        public long Uniquifier { get; set; }

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        public Program()
        {
            Uniquifier = 0;
            OverwriteExistingMasterFiles = false;
        }

        private void Run()
        {
            string redgateRepoPath = @"C:\src\Workspaces\DataWarehouse\DWH_All\Dev\Rel_002_Trinity\Databases\DWH_PRES_DB";
            string liquibaseRepoPath = @"C:\src\github\liquibase-template-for-MSSQL\db_repository";

            Handle(Path.Combine(redgateRepoPath, "Tables"), Path.Combine(liquibaseRepoPath, "Tables"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Schemas"), Path.Combine(liquibaseRepoPath, "Security", "Schemas"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Users"), Path.Combine(liquibaseRepoPath, "Security", "Users"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Roles"), Path.Combine(liquibaseRepoPath, "Security", "Roles"));
            Handle(Path.Combine(redgateRepoPath, "Views"), Path.Combine(liquibaseRepoPath, "Views"), true);
            Handle(Path.Combine(redgateRepoPath, "Stored Procedures"), Path.Combine(liquibaseRepoPath, "Stored Procedures"), true);
            Handle(Path.Combine(redgateRepoPath, "Functions"), Path.Combine(liquibaseRepoPath, "Functions"), true);
            Handle(Path.Combine(redgateRepoPath, "Types", "User-defined Data Types"), Path.Combine(liquibaseRepoPath, "Types", "User-defined Data Types"));

            Handle(Path.Combine(redgateRepoPath, "Storage", "Partition Schemes"), Path.Combine(liquibaseRepoPath, "Storage", "Partition Schemes"), true);
            Handle(Path.Combine(redgateRepoPath, "Storage", "Partition Functions"), Path.Combine(liquibaseRepoPath, "Storage", "Partition Functions"), true);
        }

        private void Handle(string redgateObjectPath, string liquibaseObjectPath, bool runOnChanges = false)
        {
            var objects = new List<FileInfo>();

            foreach (var file in System.IO.Directory.GetFiles(redgateObjectPath))
            {
                FileInfo fi = new FileInfo(file);
                objects.Add(fi);
                var targetPath = Path.Combine(liquibaseObjectPath, fi.Name);
                CreateLiquifiedFile(fi, targetPath, runOnChanges);

                Console.WriteLine(fi.Name);
            }

            CreateMasterFile(objects, liquibaseObjectPath);
        }

        private void CreateMasterFile(List<FileInfo> sqlScripts, string liquibaseFolderPath)
        {
            var targetFilePath = Path.Combine(liquibaseFolderPath, "master.xml");

            if (!OverwriteExistingMasterFiles)
            {
                // Do not overwrite existing master files.
                if (File.Exists(targetFilePath))
                    return;
            }

            string masterContentTemplate = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<databaseChangeLog
    xmlns=""http://www.liquibase.org/xml/ns/dbchangelog""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xmlns:ext=""http://www.liquibase.org/xml/ns/dbchangelog-ext""
    xsi:schemaLocation=""http://www.liquibase.org/xml/ns/dbchangelog http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-3.1.xsd
    http://www.liquibase.org/xml/ns/dbchangelog-ext http://www.liquibase.org/xml/ns/dbchangelog/dbchangelog-ext.xsd"">

{0}
</databaseChangeLog>";

            StringBuilder sb = new StringBuilder();
            foreach (var fi in sqlScripts)
            {
                sb.AppendLine(string.Format(@"        <include file=""{0}"" relativeToChangelogFile=""true"" />", fi.Name));
            }

            File.WriteAllText(targetFilePath, string.Format(masterContentTemplate, sb.ToString()));
        }

        private void CreateLiquifiedFile(FileInfo fi, string targetPath, bool runOnChanges)
        {
            using (var writer = new StreamWriter(targetPath))
            using (var reader = new StreamReader(fi.FullName))
            {
                
                bool firstDDL = true;
                
                writer.WriteLine(string.Format(@"--liquibase formatted sql"));
                writer.WriteLine(GetChangeSetComment(fi, runOnChanges));
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Trim().Equals("GO"))
                    {
                        // sometimes the GO statement is not at the begining of the line
                        line = "GO";
                    }
                    if (line.StartsWith("CREATE") || line.StartsWith("ALTER") || line.StartsWith("DROP"))
                    {
                        if (!firstDDL)
                        {
                            writer.WriteLine(GetChangeSetComment(fi, runOnChanges));    
                        }

                        firstDDL = false;
                    }
                    writer.WriteLine(line);
                }
            }
        }

        private string GetChangeSetComment(FileInfo fi, bool runOnChanges)
        {
            return string.Format(@"
--changeSet {0}:Initial-{1}-{2} endDelimiter:\nGO splitStatements:true stripComments:false runOnChange:{3}", "PeHe",
                                            fi.Name.Replace(".sql", "").Replace(".", "-"),
                                            Uniquifier++,
                                            runOnChanges);
        }

        //private static void CleanFolder(string folderPath)
        //{
        //    foreach (System.IO.FileInfo file in new DirectoryInfo(folderPath).GetFiles())
        //    {
        //        file.Delete();
        //    }
        //}
    }
}
