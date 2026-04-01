#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

# it is the base image for running your application 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 As base
WORKDIR /app
EXPOSE 8080


# it is the base image for restore, build, publish 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# A little extra optimization 
# it has all the dependencies so if these are not changed then that will be stored in the cache for the future build and helps maintain faster image building
COPY DemoApi.csproj ./
# restore the dependencies only 
RUN dotnet restore

# now copies all the things from src of the project to the containers src 
COPY . ./ 



# compiles the code and put the compiled artifacts in the app/build 
RUN dotnet build "DemoApi.csproj" -c Release -o /app/build

# new named stage (publish) based on the previous build stage
FROM build AS publish

# produces self-contained, runnable output (DLLs, static files, etc.) to /app/publish.
RUN dotnet publish "DemoApi.csproj" -c Release -o /app/publish

# again making the final stage 
FROM base AS final
# and this is final stage working directory 
WORKDIR /app
# now copying from the publish stage working directory to the final stage working directory denoted by (.) the working directory for the final stage is /app that is it 
COPY --from=publish /app/publish .
# When you run a container, Docker needs to know which process to start inside it the “main process” ENTRYPOINT = the program (the main executable) the container will always run. the dll that are created 
ENTRYPOINT ["dotnet", "DemoApi.dll"]

