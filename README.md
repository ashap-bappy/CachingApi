# CachingApi

## Overview

`CachingApi` is a .NET Core Web API application that demonstrates the usage of multiple caching mechanisms such as:

- Custom In-Memory Cache
- Redis Cache
- Memcached

The caching mechanism is configurable via the `appsettings.json` file. The API abstracts the cache through a higher-level interface (`ICacheService`), making it easy to swap between different cache implementations.

## Features

- **Custom In-Memory Cache**: A lightweight, local cache for storing frequently accessed data.
- **Redis Cache**: Distributed cache using Redis for enhanced performance and scalability.
- **Memcached**: Another distributed caching option using Memcached.

## Prerequisites

- [.NET 6 or later](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Redis Server](https://redis.io/download)
- [Memcached Server](https://memcached.org/)
  
## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/ashap-bappy/CachingApi.git
cd CachingApi
```

### 2. Configure the Cache in `appsettings.json`

```json
{
  "CacheSettings": {
    "CacheType": "Redis",  // Options: "Memory", "Redis", "Memcached"
    "Redis": {
      "ConnectionString": "localhost:6379",
      "DefaultExpirationTime": 5 // Default expiration time in minutes
    },
    "Memcached": {
      "Server": {
        "Address": "localhost",
        "Port": 11211
      },
      "DefaultExpirationTime": 5 // Default expiration time in minutes
    }
  },
}
```

- **`DefaultCacheType`**: Choose the cache implementation to use (`Memory`, `Redis`, or `Memcached`).
- **`Redis`**: Set the connection string for your Redis instance.
- **`Memcached`**: Configure Memcached server.

### 3. Install Redis and Memcached

- Install Redis: [Redis installation guide](https://redis.io/docs/getting-started/installation/)
- Install Memcached: [Memcached installation guide](https://memcached.org/)

### 4. Build and Run the Application

1. **Build the project:**

```bash
dotnet build
```

2. **Run the API:**

```bash
dotnet run http
```

The API will be available at `https://localhost:5095`.

## Usage

Once the API is running, you can interact with the cache by making HTTP requests.

### Endpoints

- **GET** `/api/cache/{key}`: Get the cached value for a given key.
- **POST** `/api/cache`: Add a value to the cache. Send the key and value in the request body.
- **DELETE** `/api/cache/{key}`: Remove a value from the cache for the given key.

### Example

#### Storing a value in the cache

```bash
curl -X POST https://localhost:5095/api/cache \
-H "Content-Type: application/json" \
-d '{"key": "sampleKey", "value": "sampleValue"}'
```

#### Retrieving a value from the cache

```bash
curl https://localhost:5095/api/cache/sampleKey
```

#### Removing a value from the cache

```bash
curl -X DELETE https://localhost:5095/api/cache/sampleKey
```

## Caching Implementations

### 1. Custom In-Memory Cache

This cache stores data in memory for the duration of the application’s lifecycle. It's a fast and efficient option for local development but not suitable for distributed environments.

### 2. Redis Cache

Redis is a popular distributed in-memory database used as a cache. The Redis cache can be used to store data across multiple instances of the API for a distributed architecture.

### 3. Memcached

Memcached is another distributed memory caching system used to speed up dynamic web applications by alleviating database load.

## Error Handling

The API logs errors when the application fails to connect to Redis or Memcached. These errors can be viewed in the console output or a configured logging provider.

## Contributing

Contributions are welcome! If you encounter any issues, feel free to open an issue or submit a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
