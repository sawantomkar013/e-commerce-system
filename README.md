# Order and Notification Microservices

This project is a technical assignment that implements a core part of a microservices-based e-commerce system. It features an **OrderService** that manages order lifecycle, integrates with a **NotificationService**, publishes events to Kafka, and uses Redis for caching.

The application is built on **.NET 8+** and adheres to modern architectural principles such as Clean Architecture and CQRS, emphasizing code quality and testability.

---

### 📖 Documentation

* [**Architectural Diagram**](doc/architectural-diagram.md) - High-level overview of the system's architecture.
* [**Code Structure & Architectural Decisions**](doc/code-structure.md) - A detailed breakdown of the project layout and the design patterns used.
* [**Tech Stack**](doc/tech-stack.md) - A complete list of all technologies and libraries.
* [**Integration Points**](doc/integration-points.md) - Details on inter-service communication and external system integrations.
* [**Local Setup**](doc/local-setup.md) - Instructions on how to set up and run the application locally.
* [**Database Setup**](doc/database-setup.md) - Guidance on configuring the SQLite database.
* [**Deployment Plan**](doc/deployment-plan.md) - Deployment plan for the application on AWS.

### 💻 API Endpoints

The APIs for these services can be accessed at the following URLs when the application is running locally:

* **Order API**: `https://localhost:5001`

* **Notification API**: `https://localhost:6001`