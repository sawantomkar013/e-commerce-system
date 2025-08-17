# Deployment Plan

### 1. Introduction & Context

This document outlines the deployment strategy for our application, which uses Kafka for message queuing and Redis for caching. It's a pivot from the initial, unsuccessful attempts at a Windows-based deployment. The new plan embraces Docker, Linux, and managed cloud services to ensure a scalable, reliable, and maintainable production environment.

The initial deployment journey on a Windows EC2 instance faced significant obstacles:

* **Fragile Dependencies:** Manual installation of Kafka and Redis on Windows proved unreliable.
* **Missing Dependencies:** The Kafka startup script failed due to the deprecated `wmic` command, which is no longer shipped with modern Windows Server builds.
* **SSL Certificate Challenges:** An SSL connection issue between internal services further complicated the setup. The root cause of the SSL challenge was relying on a local machine's certificate store, which is not a scalable or portable solution for cloud deployments.

These failures highlighted that a production-ready system requires a more robust, standardized, and automated approach.

---

### 2. Production-Ready Architecture

The recommended architecture leverages AWS's managed services to reduce operational overhead and ensure high availability and scalability.

* **Application Layer:** The .NET 8 Web API will run inside Docker containers. These containers will be orchestrated by **Amazon ECS (Elastic Container Service)** on **Fargate**, a serverless compute engine that removes the need to manage EC2 instances.
* **Load Balancing:** An **Application Load Balancer (ALB)** will be used to distribute incoming traffic across the ECS tasks, providing high availability and fault tolerance.
* **Caching:** **Amazon ElastiCache for Redis** will replace the self-hosted Redis instance, offering a fully managed, highly available caching solution.
* **Event Streaming:** **Amazon MSK (Managed Streaming for Apache Kafka)** will be used instead of a self-hosted Kafka cluster. This service automates cluster management, patching, and scaling.
* **Database:** The database will be a managed service like **Amazon RDS for PostgreSQL** or **Aurora Serverless v2**. This eliminates the maintenance burden of a self-managed database and provides built-in scalability and failover.
* **Configuration & Secrets:** All secrets and configuration will be stored securely in **AWS Systems Manager Parameter Store** or **Secrets Manager**, which are accessed by the ECS tasks using IAM roles.
* **Image Management:** The container image for the API will be stored in **Amazon ECR (Elastic Container Registry)**.
* **Observability:** **Amazon CloudWatch** will be used for centralizing logs (from ECS tasks), metrics, and setting up alarms for critical events.

---

### 3. High-Level Deployment Plan

This plan focuses on a lean, "low-ops" approach suitable for a technical demonstration or a production environment where you want to minimize manual intervention.

#### Step 1: Foundational Setup

* **Secure AWS Account:** Secure your AWS root account with MFA and create a dedicated IAM admin user for day-to-day operations.
* **Define IAM Roles:** Create an IAM role for ECS tasks with permissions to pull images from ECR, write logs to CloudWatch, and read from Parameter Store/Secrets Manager.
* **VPC and Networking:** Set up a new VPC with public subnets (for the ALB) and private subnets (for ECS tasks, ElastiCache, MSK, and RDS).

#### Step 2: Service Provisioning

* **Containerize the Application:** Create a Dockerfile for the .NET API.
* **Externalize Configuration:** Update the application to read connection strings for MSK, ElastiCache, and RDS from environment variables, which will be populated from AWS Parameter Store.
* **Provision Managed Services:**
    * Deploy Amazon RDS for PostgreSQL.
    * Deploy Amazon ElastiCache for Redis.
    * Deploy Amazon MSK.
* **Push Docker Image:** Build and push the application's Docker image to Amazon ECR.

#### Step 3: Deployment with ECS Fargate

* **Create ECS Cluster:** Set up an ECS cluster.
* **Define Task Definition:** Create an ECS task definition that specifies the ECR image, environment variables pointing to your managed services, and the IAM role.
* **Create ECS Service:** Launch an ECS service that runs the task definition, configuring it to use the ALB and placing the tasks in the private subnets.
* **Configure ALB:** Set up an Application Load Balancer in the public subnets to forward traffic to the ECS service's target group.

#### Step 4: CI/CD and Operations

* **Automate Deployment:** Implement a CI/CD pipeline using GitHub Actions. This workflow will automatically build the Docker image, push it to ECR, and trigger an ECS deployment whenever code is pushed to the main branch.
* **Monitor:** Use CloudWatch to monitor application health, service metrics (like Kafka consumer lag), and set up alarms to get notified of any issues.

This plan moves the project from a fragile, manual setup to a resilient, scalable, and automated cloud-native deployment, addressing all the pain points discovered in the initial journey.