set baseFolder=E:\SmallProjects\RoR2Mod
set r2BaseFolder=C:\Users\hurt3\AppData\Roaming\r2modmanPlus-local\RiskOfRain2

devenv %baseFolder%\RoR2Mods.sln /build Debug /project Darn1tMod\Darn1tMod.csproj

copy %baseFolder%\Darn1tMod\bin\Debug\netstandard2.1\Darn1tMod.dll %r2BaseFolder%\cache\darn1t-Darn1tMod\0.1.0\Darn1tMod.dll
copy %baseFolder%\Darn1tMod\bin\Debug\netstandard2.1\Darn1tMod.dll %r2BaseFolder%\profiles\EmptyDev\BepInEx\plugins\darn1t-Darn1tMod\Darn1tMod\Darn1tMod.dll