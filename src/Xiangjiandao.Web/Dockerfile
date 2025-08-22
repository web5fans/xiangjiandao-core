#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine-amd64 AS base
RUN apk --no-cache add tzdata
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine-amd64 AS build
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Build.targets", "."]
COPY ["src/Xiangjiandao.Web/Xiangjiandao.Web.csproj", "src/Xiangjiandao.Web/"]
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet restore "src/Xiangjiandao.Web/Xiangjiandao.Web.csproj"
COPY . .
WORKDIR "/src/src/Xiangjiandao.Web"
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet build "Xiangjiandao.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages dotnet publish "Xiangjiandao.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime && echo "Asia/Shanghai" > /etc/timezone
ENTRYPOINT ["dotnet", "Xiangjiandao.Web.dll"]