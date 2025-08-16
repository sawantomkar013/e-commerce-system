# Local Setup

Follow these steps to set up and run the application and its dependencies locally.

## ⚙️ Prerequisites

* **[.NET 8+ SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** installed.
* **[Java 11+](https://www.java.com/en/download/)** installed. Java is required to run the Kafka server and its related services.

## 🚀 Steps

1.  **Clone the Repository**

    Navigate to the directory where you want to store the project and clone the repository.

    ```bash
    git clone https://github.com/sawantomkar013/e-commerce-system.git
    cd e-commerce-system
    ```

2.  **Install and Extract Dependencies**

    Your dependencies (Kafka and Redis) are provided as compressed files. The `install-dependencies.sh` script will extract these files for you.

    **Note:** This script must be run with Administrator access.

    ```bash
    # Open Command Prompt with Administrator privileges
    install-dependencies.sh
    ```

3.  **Start Dependencies Manually (via Scripts)**

    With the dependencies extracted, you can now start the Redis and Kafka servers using the provided batch files.

    * **Start Redis Server**
        Open a new terminal and run the following script to start the Redis server:

        ```bash
        start-redis.bat
        ```

    * **Start Kafka Services & Consumers**
        Open a separate terminal and run this batch file. It will launch the Zookeeper server, Kafka broker, and three Kafka Consumer instances for the following topics:
        * `orders.created.pending`
        * `orders.created.confirmed`
        * `orders.created.shipped`

        ```bash
        start-kafka.bat
        ```
        (This script runs both `StartKafkaServices.ps1` and `KafkaConsumer.ps1`).

4.  **Configure Connection Strings**

    Update the `appsettings.json` files in your `OrderService` and `NotificationService` projects to point to your local Kafka and Redis instances.

5.  **Run the .NET Services**

    With all dependencies running, you can now start your microservices. Open two separate terminals for each service:

    * **NotificationService.API**:
        ```bash
        cd src/NotificationService/NotificationService.API
        dotnet run
        ```
    * **OrderService**:
        ```bash
        cd src/OrderService/Interface/OrderService.Interface.API
        dotnet run
        ```

6.  **Stop Dependencies**

    When you are finished, you can stop the services using the corresponding scripts.

    * **Stop Redis**:
        ```bash
        stop-redis.bat
        ```

    * **Stop Kafka**:
        ```bash
        stop-kafka.bat
        ```
        (This will task kill all Java instances, effectively stopping Zookeeper and the Kafka broker/consumers).