FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

# copy everything else and build app
COPY . ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/src/WriteApi/out ./
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT="Docker"
ENTRYPOINT ["dotnet", "WriteApi.dll"]

# docker build -t lutando/write-api:latest .
# docker push lutando/write-api:latest