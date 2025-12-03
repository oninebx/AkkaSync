# AkkaSync

**AkkaSync** is a lightweight and extensible **data synchronization and migration framework** built on top of the **Akka.NET Actor Model**.  
It is designed for enterprise data integration scenarios such as system-to-system synchronization, ETL workflows, and structured/unstructured data migration.

AkkaSync simplifies the traditional ETL complexity in .NET and provides:

- Actor-based asynchronous data pipelines  
- Extract → Transform → Load workflow  
- Checkpointing, resume, and offset tracking  
- Idempotent writes & deduplication  
- Automatic retries and supervision  
- Multi-pipeline parallelism and tenant isolation  
- Plugin-driven connectors for multiple source & target systems  

---

## 🚀 Getting Started

### 1. Configure a Pipeline

Example pipeline configuration (`samples/pipeline.yaml`):

```yaml
pipelines:
  - name: CustomerSync
    source:
      type: SQL
      connection: "Server=.;Database=Demo;User Id=sa;Password=yourpassword;"
      query: "SELECT * FROM Customer WHERE UpdatedAt > @LastSync"
    transform:
      - type: MapFields
        mappings:
          CustomerID: ID
          Name: FullName
      - type: Filter
        condition: "Status = 'Active'"
    sink:
      type: REST
      endpoint: "https://api.demo.com/customers"
      method: POST
    schedule:
      interval: 30s
```

### 2. Run the Example

```bash
dotnet run --project src/AkkaSync.Examples/DemoPipeline.cs
```

AkkaSync will:

* Initialize an Extract → Transform → Load actor pipeline

* Pull data from SQL

* Apply mapping and filtering transforms

* POST the transformed records to a REST API

* Record offsets to support checkpointing and resume

* Automatically retry failed operations

## 🔧 Architecture Overview

Core Modules

* AkkaSync.Core – Core actor framework

* AkkaSync.Connectors – Pluggable Source/Sink connectors

* AkkaSync.Transforms – Transformation rules (MapFields, Filter, Custom Functions)

* Persistence – Offset tracking, pipeline state storage

* Examples – Runnable examples demonstrating YAML-based pipeline configuration

## 🏗️ Development Guide
Extend the Framework

* Add a new Source or Sink connector by implementing the ISourceConnector or ISinkConnector interfaces

* Add custom transformations by inheriting from BaseTransform

* Create new pipelines by providing YAML configuration—no need to write orchestration code

## 🔭 Roadmap

Planned features for future releases:

* Additional source/sink connectors (CSV, Kafka, EventHub, Azure Table, etc.)

* Akka.Cluster distributed execution

* Web-based dashboard for pipeline monitoring

* Rich transformation DSL

* CLI tools for managing pipelines and inspecting offsets

## 📝 License

MIT