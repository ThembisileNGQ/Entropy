FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

# copy everything else and build app
COPY . ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT="Docker"
ENTRYPOINT ["dotnet", "ReadApi.dll"]

# docker build -t lutando/read-api:latest .
# docker push lutando/read-api:latest