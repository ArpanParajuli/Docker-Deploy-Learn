



######### Build stage ###########

# import and build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# make  /src 
WORKDIR /src

#
COPY *.csproj ./

# Restore 
RUN dotnet restore

# Copy Local Code to Container
COPY . ./


# Build 
RUN dotnet publish -c Release -o /app/publish



########### Runtime Stage ##################
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet" , "TodoApi.dll"]
