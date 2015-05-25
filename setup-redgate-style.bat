@echo off

ECHO Creating Directories
mkdir db_repository
mkdir db_repository\Assemblies
mkdir db_repository\Data
mkdir "db_repository\Database Triggers"
mkdir db_repository\Defaults
mkdir "db_repository\Extended Properties"
mkdir db_repository\Functions
mkdir db_repository\Rules
mkdir "db_repository\Search property Lists"
mkdir db_repository\Security
mkdir "db_repository\Security\Asymetric Keys"
mkdir db_repository\Security\Certificates
mkdir db_repository\Security\Roles
mkdir db_repository\Security\Schemas
mkdir "db_repository\Security\Symetric Keys"
mkdir db_repository\Security\Users
mkdir db_repository\Sequences
mkdir "db_repository\Service Broker\Contracts"
mkdir "db_repository\Service Broker\Event Notificatoins"
mkdir "db_repository\Service Broker\Message Types"
mkdir "db_repository\Service Broker\Queues"
mkdir "db_repository\Service Broker\Remote Service Bindings"
mkdir "db_repository\Service Broker\Routes"
mkdir "db_repository\Service Broker\Services"
mkdir "db_repository\Storage"
mkdir "db_repository\Storage\Full Text Catalogs"
mkdir "db_repository\Storage\Full Text Stoplists"
mkdir "db_repository\Storage\Partition Functions"
mkdir "db_repository\Storage\Partition Schemes"
mkdir "db_repository\Stored Procedures"
mkdir db_repository\Synonyms
mkdir db_repository\Tables
mkdir db_repository\Types
mkdir "db_repository\Types\User-defined Data Types"
mkdir "db_repository\Types\XML Schema Collections"
mkdir db_repository\Views

mkdir db_repository\log

ECHO Creating xml structures
copy template_files\deploymaster_redgate.xml db_repository\update.xml

copy template_files\submaster.xml db_repository\Assemblies\master.xml
copy template_files\submaster.xml db_repository\Data\master.xml
copy template_files\submaster.xml "db_repository\Database Triggers\master.xml"
copy template_files\submaster.xml db_repository\Defaults\master.xml
copy template_files\submaster.xml "db_repository\Extended Properties\master.xml"
copy template_files\submaster.xml db_repository\Functions\master.xml
copy template_files\submaster.xml db_repository\Rules\master.xml
copy template_files\submaster.xml "db_repository\Search property Lists\master.xml"
copy template_files\submaster.xml "db_repository\Security\Asymetric Keys\master.xml"
copy template_files\submaster.xml "db_repository\Security\Certificates\master.xml
copy template_files\submaster.xml "db_repository\Security\Roles\master.xml
copy template_files\submaster.xml db_repository\Security\Schemas\master.xml
copy template_files\submaster.xml "db_repository\Security\Symetric Keys\master.xml"
copy template_files\submaster.xml db_repository\Security\Users\master.xml
copy template_files\submaster.xml db_repository\Sequences\master.xml
copy template_files\submaster.xml "db_repository\Service Broker\Contracts\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Event Notificatoins\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Message Types\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Queues\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Remote Service Bindings\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Routes\master.xml"
copy template_files\submaster.xml "db_repository\Service Broker\Services\master.xml"
copy template_files\submaster.xml "db_repository\Storage\Full Text Catalogs\master.xml"
copy template_files\submaster.xml "db_repository\Storage\Full Text Stoplists\master.xml"
copy template_files\submaster.xml "db_repository\Storage\Partition Functions\master.xml"
copy template_files\submaster.xml "db_repository\Storage\Partition Schemes\master.xml"
copy template_files\submaster.xml "db_repository\Stored Procedures\master.xml"
copy template_files\submaster.xml db_repository\Synonyms\master.xml
copy template_files\submaster.xml db_repository\Tables\master.xml
copy template_files\submaster.xml "db_repository\Types\User-defined Data Types\master.xml"
copy template_files\submaster.xml "db_repository\Types\XML Schema Collections\master.xml"
copy template_files\submaster.xml db_repository\Views\master.xml

copy template_files\Liquibase.properties.template db_repository\Liquibase.properties
copy template_files\Liquibase.properties.template db_repository\Liquibase.properties.template
copy template_files\run_liquibase.bat db_repository\

ECHO Adding liquibase binaries
robocopy template_files\liquibase-app db_repository\liquibase-app  /E

ECHO Your Repository is now setup in the db_repository folder
ECHO Edit the Liquibase.properties files to configure it to a specific server/dbName
ECHO DONE
