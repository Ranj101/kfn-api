## Kurdistan Food Network

Welcome to Kurdistan Food Network, an open source project that aims to support local producers in the Kurdistan region of Iraq by providing a digital platform to showcase and sell their goods and services.

This repository contains the source code for the api component in the Kurdistan Food Network Project.

### Technologies Used

- .NET 7.0 SDK
- PostgreSQL
- Redis

### Run Locally

To install and run the project locally, please follow these steps:

#### 1. Use the docker compose provided in the repository:

The docker compose will provide you with:
1. %%%
2. %%%

#### 2. Clone the repository: 

```sh
git clone https://github.com/Ranj101/kurdistan-food-network.git
```

#### 3. Run the following command to start the application.

```sh
dotnet restore
dotnet run --project src/KfnApi
```

The api will now be served at http://localhost:5018 by default.

### Tests

You can run all the tests locally using the command:

```sh
dotnet test
```

### Configurations

The api comes with preconfigured settings but it can be edited in the [appsettings.json](src/PilgrimageSettingsApi/appsettings.json) file.

Configurations can be overridden for different environments using _appsettings.ENV_NAME.json_.<br />
for example: [appsettings.Development.json](src/PilgrimageSettingsApi/appsettings.Development.json).

| Key  | Type                        | Description         |
|------|-----------------------------|---------------------|
| Name | [NameOptions](src/KfnApi)   | Name configuration. |

### Contributing

Contributions from the community are also welcomed! Please follow these guidelines:

- Fork the repository and create a new branch for your feature or bugfix.
- Write clean, readable, and maintainable code.
- Test your changes thoroughly.
- Submit a pull request with a clear description of your changes.

### License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


