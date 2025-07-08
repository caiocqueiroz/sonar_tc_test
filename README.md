# sonar_tc_test

This repository contains a .NET 8 console application with SonarQube analysis integrated into a TeamCity CI pipeline. The pipeline runs in Docker containers, and the application includes intentional code duplications to trigger SonarQube warnings. The application also includes comprehensive unit tests with code coverage.

## TeamCity Build Configuration

To set up the TeamCity build configuration, follow these steps:

1. Create a new build configuration in TeamCity
2. Add the SonarQube Begin Analysis, Build, Test, and SonarQube End Analysis steps as described below
3. Configure the build parameters to point to your SonarQube instance

## Docker Setup Instructions

# Run TeamCity Server
docker run --name tcsrv-env-test-sonartc \
    -v ~/data:/data/teamcity_server/datadir \
    -v ~/data/logs:/opt/teamcity/logs \
    -v ~/data/config:/data/config \
    -p 8111:8111 \
    --user 0 \
    jetbrains/teamcity-server

# Run Agent TeamCity
docker run -e SERVER_URL="http://tcsrv-env-test-sonartc:8111" \
    -v ~/data/agent:/data/teamcity_agent/conf \
    --user 0 \
    jetbrains/teamcity-agent

# Create the docker network
docker network create docker-networks

# Connect to the Docker network
 docker network connect docker-networks container-id

 apt-get update

# Install .NET SDK (if not already installed)
apt-get install -y wget apt-transport-https
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
apt-get update
apt-get install -y dotnet-sdk-8.0

# Install SonarScanner for .NET as a global tool
dotnet tool install --global dotnet-sonarscanner

# Install reportgenerator for code coverage reports
dotnet tool install -g dotnet-reportgenerator-globaltool

# Note: The coverlet.msbuild package should be added to the test project file (HelloWorld.Tests.csproj)
# This is done in the source code by adding the following PackageReference:
# <PackageReference Include="coverlet.msbuild" Version="6.0.4">
#   <PrivateAssets>all</PrivateAssets>
#   <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
# </PackageReference>

# Add dotnet tools to PATH (add this to ~/.bashrc for persistence)
export PATH="$PATH:/root/.dotnet/tools"
source ~/.bashrc


# create sonar volumes 
docker volume create --name sonarqube_data
docker volume create --name sonarqube_logs
docker volume create --name sonarqube_extensions

# start SonarQube
docker run --rm \
    -p 9000:9000 \
    -v sonarqube_extensions:/opt/sonarqube/extensions \
    sonarqube

# Token for SonarQube
sqa_55225362b671f1553da99fa0e4576a120db04877

# TeamCity Build Configuration Steps

## 1. SonarQube Begin Analysis
Command: dotnet
Parameters: >-
  sonarscanner begin
  /k:"HelloWorld"
  /d:sonar.host.url="http://sonarqube:9000"
  /d:sonar.login="sqa_55225362b671f1553da99fa0e4576a120db04877"
  /d:sonar.cs.opencover.reportsPaths="%system.teamcity.build.checkoutDir%/HelloWorld.Tests/coverage.opencover.xml"

## 2. Build Step
Command: dotnet
Parameters: build

## 3. Test Step
Command: dotnet
Parameters: >-
  test
  --logger:"trx;LogFileName=testresults.trx"
  /p:CollectCoverage=true
  /p:CoverletOutputFormat=opencover
  /p:CoverletOutput=./coverage.opencover.xml
  /p:Include="[HelloWorld]*"
  /p:Exclude="[HelloWorld.Tests]*"

## 4. Report Generator (Optional for HTML reports)
Command: ~/.dotnet/tools/reportgenerator
Parameters: >-
  "-reports:%system.teamcity.build.checkoutDir%/HelloWorld.Tests/coverage.opencover.xml"
  "-targetdir:%system.teamcity.build.checkoutDir%/CoverageReport"
  "-reporttypes:Html"

## 5. SonarQube End Analysis
Command: dotnet
Parameters: >-
  sonarscanner end
  /d:sonar.login="sqa_55225362b671f1553da99fa0e4576a120db04877"

# Unit Tests

The project includes comprehensive unit tests that cover most of the application code. The tests use:

1. **xUnit** - Testing framework
2. **Moq** - Mocking framework for creating test doubles
3. **Coverlet** - Code coverage tool

The tests are organized into several categories:

- **ProgramTests** - Tests for the main program logic
- **AgeCalculatorTests** - Tests for the age calculation functionality
- **ConsoleUserInterfaceTests** - Tests for the console UI functionality
- **IntegrationTests** - Tests that verify the complete program flow

The test coverage is reported in the OpenCover format, which can be integrated with SonarQube for code quality analysis.

## Running Tests Locally

To run the tests locally:

```bash
dotnet test --logger:"console;verbosity=detailed" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage.opencover.xml /p:Include="[HelloWorld]*" /p:Exclude="[HelloWorld.Tests]*"
```

To generate an HTML coverage report:

```bash
~/.dotnet/tools/reportgenerator -reports:"./HelloWorld.Tests/coverage.opencover.xml" -targetdir:"./CoverageReport" -reporttypes:Html
```