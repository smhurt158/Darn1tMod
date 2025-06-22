set baseFolder=E:\SmallProjects\RoR2Mod
set r2BaseFolder=C:\Users\hurt3\AppData\Roaming\r2modmanPlus-local\RiskOfRain2

set netVersion=2.1

set modVersion=0.2.1

set profile=Developer
::set profile=EmptyDev


devenv %baseFolder%\RoR2Mods.sln /build Debug /project Darn1tMod\Darn1tMod.csproj

copy %baseFolder%\Darn1tMod\bin\Debug\netstandard%netVersion%\Darn1tMod.dll %r2BaseFolder%\cache\darn1t-Darn1tMod\%modVersion%\Darn1tMod.dll
copy %baseFolder%\Darn1tMod\bin\Debug\netstandard%netVersion%\Darn1tMod.dll %r2BaseFolder%\profiles\%profile%\BepInEx\plugins\darn1t-Darn1tMod\Darn1tMod\Darn1tMod.dll