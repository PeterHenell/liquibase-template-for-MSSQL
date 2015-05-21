@echo off

ECHO Creating Directories
mkdir db_repository
mkdir db_repository\deploy\functions
mkdir db_repository\deploy\jobs
mkdir db_repository\deploy\schema\tables
mkdir db_repository\deploy\schema\views
mkdir db_repository\deploy\schema\indexes
mkdir db_repository\deploy\sequences
mkdir db_repository\deploy\triggers
mkdir db_repository\deploy\types
mkdir db_repository\deploy\procedures
mkdir db_repository\deploy\schemas


ECHO Creating xml structures
copy template_files\submaster.xml db_repository\deploy\functions\master.xml
copy template_files\submaster.xml db_repository\deploy\jobs\master.xml
copy template_files\submaster.xml db_repository\deploy\schema\tables\master.xml
copy template_files\submaster.xml db_repository\deploy\schema\views\master.xml
copy template_files\submaster.xml db_repository\deploy\schema\indexes\master.xml
copy template_files\submaster.xml db_repository\deploy\sequences\master.xml
copy template_files\submaster.xml db_repository\deploy\triggers\master.xml
copy template_files\submaster.xml db_repository\deploy\types\master.xml
copy template_files\submaster.xml db_repository\deploy\procedures\master.xml
copy template_files\submaster.xml db_repository\deploy\schemas\master.xml
copy template_files\deploymaster.xml db_repository\deploy\master.xml

copy template_files\update.xml db_repository\
copy template_files\Liquibase.properties.template db_repository\Liquibase.properties
copy template_files\run_liquibase.bat db_repository\

ECHO Adding liquibase binaries
robocopy template_files\liquibase-app db_repository\liquibase-app


ECHO Your Repository is now setup in the db_repository folder
ECHO Edit the Liquibase.properties files to configure it to a specific server/dbName
ECHO DONE
