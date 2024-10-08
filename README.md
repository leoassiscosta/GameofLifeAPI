# ConwayGameOfLifeAPI

This is an API project that implements **Conway's Game of Life**, a simulation based on cellular automata. The API allows creating new boards, advancing to the next state or multiple states, and detecting when the board reaches a final or stable state. Data persistence is handled using MongoDB, and the application is configured to run in a Docker container.

## Table of Contents

- [Technologies](#technologies)
- [Requirements](#requirements)
- [Installation](#installation)
- [Configuration](#configuration)
- [Execution](#execution)
- [API Routes](#api-routes)
- [Testing](#testing)
- [Contributing](#contributing)
- [License](#license)

## Technologies

- **C#**
- **.NET 6.0**
- **MongoDB** (as the database)
- **Docker** (for containerization)
- **MongoDB.Driver** (C# driver for MongoDB)
- **Repository Pattern** (for database abstraction)
- **SOLID Principles** (for clean and maintainable design)

## Requirements

Before you begin, make sure you have the following tools installed on your machine:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)
- [MongoDB](https://www.mongodb.com/try/download/community) (optional if not using Docker)

## Installation

1. Clone the repository:
   
   ```bash
   git clone https://github.com/yourusername/ConwayGameOfLifeAPI.git

3. Navigate to the project directory:

   ```bash
   cd ConwayGameOfLifeAPI

4. Restore dependencies:

   ```bash
   dotnet restore

## Configuration

1. Docker
The project is set up to run inside a Docker container. To configure MongoDB database and API with Docker, follow these steps:

   ```yml
   version: '3.8'
services:
  mongo:
    image: mongo:latest
    container_name: conway_mongo
    ports:
      - "27017:27017"
    networks:
      - conway_network
  api:
    build: .
    container_name: conway_api
    ports:
      - "5000:5000"
    networks:
      - conway_network
    depends_on:
      - mongo

networks:
  conway_network:
    driver: bridge
