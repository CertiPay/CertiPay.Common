@echo off

powershell -NoProfile -ExecutionPolicy bypass -Command "%~dp0.build\build.ps1 %*;"
exit /B %errorlevel%