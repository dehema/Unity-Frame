@SET EXCEL_FOLDER=..\ExcelFile
@SET JSON_FOLDER=..\ExcelFile
@SET EXE=Excel2JsonEX.CMD.exe

@ECHO Converting excel files in folder %EXCEL_FOLDER% ...
for /f "delims=" %%i in ('dir /b /a-d /s %EXCEL_FOLDER%\*.xlsx') do (
    @echo  processing %%~nxi 
    @CALL %EXE% --excel %EXCEL_FOLDER%\%%~nxi --json %JSON_FOLDER%\%%~ni.json --header 3 --exclude_prefix # --s
)