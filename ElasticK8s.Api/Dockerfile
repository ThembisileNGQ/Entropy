#FROM microsoft/aspnetcore:2.0.9 as runner
#COPY . /app
#WORKDIR app
#ENTRYPOINT ["dotnet", "RecordUnion.AdminApi.dll"]
#EXPOSE 5001





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
COPY --from=build /app/aspnetapp/src/ElasticK8s.Api/out ./
EXPOSE 5001
ENV ASPNETCORE_ENVIRONMENT="Docker"
ENTRYPOINT ["dotnet", "ElasticK8s.Api.dll"]

# docker build -t lutando/simple-api:latest .
# docker push lutando/simple-api:latest