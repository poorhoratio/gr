# Migration Report

I tried to keep this simple. In a "real" app I would have done some things differently. The project was created with Visual Studio 2019. Assuming that is installed you should be able to open the .sln file and compile/run it easily. It's using .NET 5, so you will need that SDK. You can also run it in Visual Studio Code.

At one point the code was also generating a .sql file with possible fixes. Little Bobby Tables would love me for that. That's not how I would do that in the real world. I removed that code, so the code isn't actually updating anything, it's only creating a csv file.

App settings
-----------------

The database settings are contained in appsettings.json. If both databases are running on localhost you may not need to update these values. The default values expect the old accounts database to be running on port 5432 and the new accounts database on port 5433.


Console arguments
-----------------

Two console options are supported by default:

* `-h|--help` Shows help.
* `-p|--path` Optional output path. If a path is not provided the executing directory will be used. The file created will be [path]\MigrationReport.csv.
