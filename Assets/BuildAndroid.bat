@echo off
chcp 65001 > nul
setlocal

:: 设置Unity路径（根据你的安装位置修改）
set "UNITY_PATH=E:\Program Files\Unity\2022.3.43f1c1\Editor\Unity.exe"

:: 设置项目路径
set "PROJECT_PATH=%CD%"

:: 检查是否有-outputPath参数
set "CUSTOM_OUTPUT_PATH="
set "HAS_CUSTOM_PATH=0"

:CHECK_PARAMS
if "%1"=="" goto PARAMS_CHECKED
if "%1"=="-outputPath" (
    set "CUSTOM_OUTPUT_PATH=%2"
    set "HAS_CUSTOM_PATH=1"
    shift
    shift
    goto CHECK_PARAMS
)
shift
goto CHECK_PARAMS

:PARAMS_CHECKED
:: 设置输出路径
set "OUTPUT_PATH=%CD%\Builds\Android\MyGame.apk"

:: 提取输出目录
for %%F in ("%OUTPUT_PATH%") do set "OUTPUT_DIR=%%~dpF"

:: 创建输出目录
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

:: 执行Unity命令行打包
echo 开始打包Android APK...
echo 输出路径: %OUTPUT_PATH%
"%UNITY_PATH%" ^
-batchmode ^
-nographics ^
-quit ^
-projectPath "%PROJECT_PATH%" ^
-executeMethod EditorBuildScript.BuildAndroid ^
-outputPath "%OUTPUT_PATH%" ^
-logFile "%OUTPUT_DIR%\build.log"

:: 检查打包是否成功
if %errorlevel% neq 0 (
    echo 打包失败！查看日志: %OUTPUT_DIR%\build.log
    exit /b 1
) else (
    echo 打包成功！APK位置：%OUTPUT_PATH%
)