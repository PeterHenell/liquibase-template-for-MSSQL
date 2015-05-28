# liquibase-template-for-MSSQL
Minimal Template to set up the folder structure for Liquibase to use on Windows with MSSQL

## How to use
* Clone
* Inside the cloned directory, run the setup.bat file
* A new folder called db_repository will be created with a complete setup of all folder structures needed for Liquibase to run.
* Move the db_repository folder, or the contents of it, to your actual repository. 
* Edit the Liquibase.properties file to point to your local dev environment and to include your credentials.
* Start adding objects, the liquibase way!

## Included gotchas
* Use <changeSet ... <sqlFile endDelimiter="\nGO" splitStatements="true" path="procedureName.sql" ... to handle procedures.
   * Example in submaster.xml
* It is very important to use the XSD of the correct version in each databaseChangeLog.xml file
   * Example in submaster.xml
* Use '<include relativeToChangelogFile="true" file="myTable.sql" ... to handle tables. 
  * The relativeToChangelogFile attribute makes it easier to read the xml file.
* Because SQL Server does not support CREATE OR REPLACE I  have used a sql-tagg in the databaseChangelog-file to first create an empty stub of the procedure.
  * This way the real procedure script can always use ALTER PROCEDURE
  * Another alternative is to use dynamic SQL inside the procedure-script to first check if the procedure exists and then create a dummy version.
  * It is prefered to use ALTER on subsequent modifications because then permissions will be preserved. (using DROP will remove all permissions set on the procedure/object)
* You can use GO statements in your sql scripts when using endDelimiter="\nGO", but it("GO") will be case sensitive. Go go gO will cause syntax error.
* --liquibase formatted sql tagg is very sensitive and must have no space and have correct case.
* Be careful of having GO statements in comments, as the liquibase parser wil not understand that it is inside a comment.
* The liquibase parser will split the file into batches based on the regexp \nGO. This mean that if GOTO is on a new line your deployment will probably fail or worse...