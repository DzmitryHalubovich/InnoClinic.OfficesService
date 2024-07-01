dotnet tool install -g coverlet.console

coverlet ...\tests\Offices.UnitTests\bin\Debug\net8.0\Offices.UnitTests.dll --target "dotnet" --targetargs "test --no-build" - show test coverage in console

dotnet test --collect:"XPlat Code Coverage" - create tests report file

reportgenerator -reports:...\tests\Offices.UnitTests\TestResults\84e47a6e-8a41-47bc-9e9b-9770a3b3ad95\coverage.cobertura.xml -targetdir:"coverageresults" -reporttypes:Html - generate web site with tests coverage information
