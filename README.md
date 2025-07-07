# sonar_tc_test

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

# Add dotnet tools to PATH (add this to ~/.bashrc for persistence)
export PATH="$PATH:/root/.dotnet/tools"
source ~/.bashrc
