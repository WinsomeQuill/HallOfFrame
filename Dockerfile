FROM ubuntu:latest AS stage-build

WORKDIR /src

EXPOSE 5112

RUN apt-get update && apt-get upgrade -y && apt-get install -y curl
RUN apt-get install -y dotnet-sdk-7.0 && apt-get install -y dotnet-runtime-7.0 && apt-get install -y aspnetcore-runtime-7.0
RUN apt-get install mc -y

COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

ENV POSTGRES_HOST="postgres-db"
ENV POSTGRES_PORT="5432"
ENV POSTGRES_DB="HOF_Test"
ENV POSTGRES_USER="artem"
ENV POSTGRES_PASSWORD="password"

RUN dotnet build

CMD ["dotnet", "ef", "migrations", "add", "MyMigration"]
CMD ["dotnet", "ef", "database", "update"]
CMD ["dotnet", "run"]