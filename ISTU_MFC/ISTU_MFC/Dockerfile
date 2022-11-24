FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ISTU_MFC.csproj", "ISTU_MFC/"]
RUN dotnet restore "ISTU_MFC/ISTU_MFC.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "ISTU_MFC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ISTU_MFC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ISTU_MFC.dll"]
