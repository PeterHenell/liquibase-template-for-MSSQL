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
            string[] databases = new string[] {  "DWH_TEMP"		
                                                ,"DWH_JOURNAL"	
                                                ,"DWH_TOOLKIT"	
                                                ,"DWH_MANUAL"	
                                                ,"DWH_APP"		
                                                ,"DWH_HISTORY"	
                                                ,"DWH_BASE"		
                                               // ,"DWH_PRES"		
                                               // ,"DWH_UNIT_TEST"	
                                                };

            foreach (var db in databases)
                p.Run(db);
        }

        public Program()
        {
            Uniquifier = 0;
            OverwriteExistingMasterFiles = true;
        }

        private void Run(string dbName)
        {
            string redgateRepoPath = Path.Combine(@"C:\src\Workspaces\DataWarehouse\DWH_All\Dev\Rel_004_Morpheus\Databases", dbName + "_DB");
            string liquibaseRepoPath = Path.Combine(@"C:\src\liquibased_databases", dbName);

            Handle(Path.Combine(redgateRepoPath, "Tables"), Path.Combine(liquibaseRepoPath, "Tables"));
            Handle(Path.Combine(redgateRepoPath, "Assemblies"), Path.Combine(liquibaseRepoPath, "Assemblies"));

            Handle(Path.Combine(redgateRepoPath, "Security", "Schemas"), Path.Combine(liquibaseRepoPath, "Security", "Schemas"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Users"), Path.Combine(liquibaseRepoPath, "Security", "Users"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Roles"), Path.Combine(liquibaseRepoPath, "Security", "Roles"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Asymmetric Keys"), Path.Combine(liquibaseRepoPath, "Security", "Asymmetric Keys"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Certificates"), Path.Combine(liquibaseRepoPath, "Security", "Certificates"));
            Handle(Path.Combine(redgateRepoPath, "Security", "Symmetric Keys"), Path.Combine(liquibaseRepoPath, "Security", "Symmetric Keys"));

            Handle(Path.Combine(redgateRepoPath, "Views"), Path.Combine(liquibaseRepoPath, "Views"), true);
            Handle(Path.Combine(redgateRepoPath, "Stored Procedures"), Path.Combine(liquibaseRepoPath, "Stored Procedures"), true, false);
            Handle(Path.Combine(redgateRepoPath, "Functions"), Path.Combine(liquibaseRepoPath, "Functions"), true);
            Handle(Path.Combine(redgateRepoPath, "Types", "User-defined Data Types"), Path.Combine(liquibaseRepoPath, "Types", "User-defined Data Types"));
            Handle(Path.Combine(redgateRepoPath, "Types", "XML Schema Collections"), Path.Combine(liquibaseRepoPath, "Types", "XML Schema Collections"));

            Handle(Path.Combine(redgateRepoPath, "Storage", "Partition Schemes"), Path.Combine(liquibaseRepoPath, "Storage", "Partition Schemes"), true);
            Handle(Path.Combine(redgateRepoPath, "Storage", "Partition Functions"), Path.Combine(liquibaseRepoPath, "Storage", "Partition Functions"), true);

            Handle(Path.Combine(redgateRepoPath, "Data"), Path.Combine(liquibaseRepoPath, "Data"));
            Handle(Path.Combine(redgateRepoPath, "Database Triggers"), Path.Combine(liquibaseRepoPath, "Database Triggers"));

            Handle(Path.Combine(redgateRepoPath, "Defaults"), Path.Combine(liquibaseRepoPath, "Defaults"));
            Handle(Path.Combine(redgateRepoPath, "Extended Properties"), Path.Combine(liquibaseRepoPath, "Extended Properties"));
            Handle(Path.Combine(redgateRepoPath, "Rules"), Path.Combine(liquibaseRepoPath, "Rules"));
            Handle(Path.Combine(redgateRepoPath, "Search property Lists"), Path.Combine(liquibaseRepoPath, "Search property Lists"));
            Handle(Path.Combine(redgateRepoPath, "Sequences"), Path.Combine(liquibaseRepoPath, "Sequences"));
            Handle(Path.Combine(redgateRepoPath, "Synonyms"), Path.Combine(liquibaseRepoPath, "Synonyms"));

            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Contracts"), Path.Combine(liquibaseRepoPath, "Service Broker", "Contracts"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Event Notifications"), Path.Combine(liquibaseRepoPath, "Service Broker", "Event Notifications"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Message Types"), Path.Combine(liquibaseRepoPath, "Service Broker", "Message Types"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Queues"), Path.Combine(liquibaseRepoPath, "Service Broker", "Queues"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Remote Service Bindings"), Path.Combine(liquibaseRepoPath, "Service Broker", "Remote Service Bindings"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Routes"), Path.Combine(liquibaseRepoPath, "Service Broker", "Routes"));
            Handle(Path.Combine(redgateRepoPath, "Service Broker", "Services"), Path.Combine(liquibaseRepoPath, "Service Broker", "Services"));
        }

        private void Handle(string redgateObjectPath, string liquibaseObjectPath, bool runOnChanges = false, bool addChangeSetForEachDDL = true)
        {
            var objects = new List<FileInfo>();

            foreach (var file in System.IO.Directory.GetFiles(redgateObjectPath))
            {
                FileInfo fi = new FileInfo(file);
                objects.Add(fi);
                var targetPath = Path.Combine(liquibaseObjectPath, fi.Name);
                CreateLiquifiedFile(fi, targetPath, runOnChanges, addChangeSetForEachDDL);

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

        private void CreateLiquifiedFile(FileInfo fi, string targetPath, bool runOnChanges, bool addChangeSetForEachDDL)
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
                    if (line.Trim().ToUpper().Equals("GO"))
                    {
                        // sometimes the GO statement is not at the begining of the line
                        line = "GO";
                    }
                    if (line.StartsWith("CREATE") || line.StartsWith("ALTER") || line.StartsWith("DROP"))
                    {
                        if (!firstDDL && addChangeSetForEachDDL)
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
                                            fi.Name.Replace(" ", "").Replace(".sql", "").Replace(".", "-"),
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
