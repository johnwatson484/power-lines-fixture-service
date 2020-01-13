[![Build Status](https://johnwatson484.visualstudio.com/John%20D%20Watson/_apis/build/status/Power%20Lines%20Fixture%20Service?branchName=master)](https://johnwatson484.visualstudio.com/John%20D%20Watson/_build/latest?definitionId=33&branchName=master)

# Power Lines Fixture Service
Fixture repository service

# Prerequisites
- Docker
- Docker Compose

# Running the application
The application is intended to be run and developed within a container.  A set of docker-compose files exist to support this.

## Run application in container
Note the application is dependent on an external PostgreSQL database.  A connection string in the environment variable `ConnectionStrings__DefaultConnection` is required.

The application will be accessible on port `8000`.

```
docker-compose build
docker-compose up
```

## Develop application in container
This will create a PostgreSQL database in a separate container exposed on port `5432`.  The application will be accessible on port `5000`.

The application is dependent on an existing Docker network named `power-lines`.

```
docker network create power-lines
docker-compose -f docker-compose.yaml -f docker-compose.development.yaml build
docker-compose -f docker-compose.yaml -f docker-compose.development.yaml up
```

The service is dependent on an AMQP 1.0 message broker. For development an ActiveMQ Artemis container is provided.
```
docker-compose -f docker-compose.yaml -f docker-compose.development.yaml -f docker-compose.external.yaml build
docker-compose -f docker-compose.yaml -f docker-compose.development.yaml -f docker-compose.external.yaml up
```

## Debug application in container
Running the above development container configuration will include a remote debugger that can be connected to using the example VS Code debug configuration within `./vscode`.

The `${command:pickRemoteProcess}` will prompt for which process to connect to within the container.  

Note that if this will not work if there is a space in the filepath to the VS Code extensions.  In that scenario the process Id should be manually added to the debug config.  The process Id can be found by running the below command.

`docker exec -i power-lines-fixture-service sh -s < "C:\Users\<USERNAME>\.vscode\extensions\ms-vscode.csharp-1.21.9\scripts\remoteProcessPickerScript"`

Local changes to code files will automatically trigger a rebuild and restart of the application within the container.

## Run tests
Unit tests are written in NUnit.

```
docker-compose -f docker-compose.test.yaml build
docker-compose -f docker-compose.test.yaml up
```

The test container will automatically close when all tests have been completed.  There is also the option to run the test container using a file watch to aide local development.

```
docker-compose -f docker-compose.test.yaml -f docker-compose.development.test.yaml build
docker-compose -f docker-compose.test.yaml -f docker-compose.development.test.yaml up
```
