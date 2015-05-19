@echo off
FOR /F %%A IN ('WMIC OS GET LocalDateTime ^| FINDSTR \.') DO @SET B=%%A

./liquibase-app/liquibase --logLevel=info --logFile=log/liquibase-%B:~0,4%-%B:~4,2%-%B:~6,2%-%B:~8,2%_%B:~10,2%.log --changeLogFile=update.xml update
