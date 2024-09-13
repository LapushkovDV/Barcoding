rem set Path=C:\Program Files (x86)\Pervasive Software\PSQL\bin\;%Path% 
set ProjDir=%~p0
set Atlantis=D:\Work\5.5.26.0\
set TPU=D:\Work\bin\
set DataBase=D:\Work\DataTest 
set WorkerId=0
rem %Atlantis%exe\vip.exe /XML2REP:%TPU%C_AlterCumulative_Regist.xml  /database.databasename=%DataBase%
rem  /database.databasename=%DataBase%

del c:\temp\%WorkerId% /Y
mkdir c:\temp
mkdir c:\temp\%WorkerId%

c:
cd c:\temp\%WorkerId%
start %TPU%atlwp.exe /c:%ProjDir%\GALNET.CFG /#WorkerId=%WorkerId%