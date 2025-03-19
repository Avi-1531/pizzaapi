# Use the official Microsoft .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official Microsoft .NET Runtime image to create the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage to the runtime stage
COPY --from=build-env /app/out .

#Expose port

EXPOSE 7048

# Specify the command to run the application
ENTRYPOINT ["dotnet", "DemoApi.dll"]
