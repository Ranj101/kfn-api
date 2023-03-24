## Kurdistan Food Network

Welcome to Kurdistan Food Network, an open source project that aims to support local producers in the Kurdistan region of Iraq.

The Kurdistan Food Network project was started to help local producers in the Kurdistan region of Iraq to gain more visibility and sell their goods and services to a wider audience. The project provides a digital platform where local producers can create profiles and showcase their products, while customers can browse and place orders for available goods and services.

I believe that supporting local producers is important for building stronger, more sustainable communities. By creating a platform that connects producers and customers, I hope to encourage the growth of small businesses and provide customers with access to a wider range of high-quality, locally produced products.

The project is open source and I welcome contributions from anyone who is interested in helping to improve the platform. Whether you're a developer, designer, or just interested in supporting local producers, there are many ways to get involved. I hope that this project can serve as a model for other communities looking to build similar platforms and support local producers.

This repository contains the source code for the api component in the Kurdistan Food Network Project.

### Technologies Used

- .NET 7.0 SDK
- PostgreSQL
- Redis

### Run Locally

#### 1. Use the docker compose provided in the repository

#### 2. Clone the repository

```sh
git clone https://github.com/Ranj101/kfn-api.git
```

#### 3. Run the following command to start the application

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

| Key      | Type                                                             | Description                        |
|----------|------------------------------------------------------------------|------------------------------------|
| Postgres | [PostgresOptions](src/KfnApi/Models/Settings/PostgresOptions.cs) | PostgreSQL database configuration. |
| Redis    | [RedisOptions](src/KfnApi/Models/Settings/RedisOptions.cs)       | Redis cache configuration.         |

### Contributing

Contributions from the community are also welcomed! Please follow these guidelines:

- Fork the repository and create a new branch for your feature or bugfix.
- Write clean, readable, and maintainable code.
- Test your changes thoroughly.
- Submit a pull request with a clear description of your changes.

### License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/Ranj101/kfn-api/blob/main/LICENSE.md) file for details.
