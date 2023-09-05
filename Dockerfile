FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

ARG AppConfig
ENV AppConfigConnectionString=$AppConfi
ENV ASPNETCORE_ENVIRONMENT=Production

RUN apt-get update

RUN echo "deb http://deb.debian.org/debian bullseye main contrib" >> /etc/apt/sources.list \
&& apt-get update \
&& apt-get install -y --allow-unauthenticated \
libc6

COPY . .
RUN dotnet restore "Application/IAcademyAPI/IAcademyAPI.csproj"
RUN dotnet restore "Tests/IAcademy.Test.Integration/IAcademy.Test.Integration.csproj"
RUN dotnet restore "Tests/IAcademy.Test.Unit/IAcademy.Test.Unit.csproj"

WORKDIR "/src/Tests"
#RUN dotnet build "IAcademy.Test.Integration/IAcademy.Test.Integration.csproj" -c Release -o /app/build
#RUN dotnet test "IAcademy.Test.Integration/IAcademy.Test.Integration.csproj" --logger "trx;LogFileName=IntegrationTests.trx"
RUN dotnet build "IAcademy.Test.Unit/IAcademy.Test.Unit.csproj" -c Release -o /app/build
RUN dotnet test "IAcademy.Test.Unit/IAcademy.Test.Unit.csproj" --logger "trx;LogFileName=UnitTests.trx"

WORKDIR "/src/Application/IAcademyAPI"
RUN dotnet build "IAcademyAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IAcademyAPI.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENTRYPOINT ["dotnet", "IAcademyAPI.dll"]