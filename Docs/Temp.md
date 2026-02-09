## 🚀 一、AkkaSync 高级架构图（文字版）
```pgsql        
                +----------------------------+
                |       AkkaSync Core        |
                |----------------------------|
                |  Actor System              |
                |  Supervisor / Backoff      |
                |  Message Routing           |
                |  Retry / Dead Letter Queue |
                |  Sync Pipeline Engine      |
                |  Checkpoint Store (State)  |
                |  Plugin Interfaces (ISource|
                |  & ISink & ITransformer)   |
                +-------------+--------------+
                              |
           ------------------------------------------------
           |                   |                        |
   +--------------+    +----------------+       +----------------+
   | Source Plugin|    | Transform Plug |       | Sink Plugin    |
   | (e.g. SQL,   |    | (Optional ETL) |       | (e.g. SQL,     |
   |  API, Queue) |    |                |       |  API, Queue)   |
   +--------------+    +----------------+       +----------------+
           |                   |                        |
           +-------------------+------------------------+
                              |
                            Data
                              |
                        +------------+
                        | Akka Cluster|
                        | Sharding    |
                        | Distributed |
                        +------------+

```



## 二、AkkaSync MVP 功能清单（最小可行版本）
### ✔ 核心功能（必须）

1. Actor 系统管理（启动、停止、配置）

2. SyncPipeline Actor

  * 控制同步流程：Extract → Transform → Load

3. 可插拔 Source/Sink

  * ISyncSource

  * ISyncSink

4. 断点续传（Checkpoint）

  * 本地 JSON / LiteDB / SQLite 存储

5. 失败重试机制

  * 固定重试

  * Backoff Supervisor

6. 消息可靠投递（At-least-once）

7. 吞吐量控制（每秒批次、限流）

8. 简单的本地运行管理控制台（例如 CLI 命令）

### ✔ Demo（必须）

* 本地 JSON Files → 转换 → SQLite

* 无业务逻辑，完全技术性质

* 展示管道流动、断点续传、重试机制

```scss
FileSource → Transform (uppercase / map) → SQLiteSink
```

### ✔ 文档（必须）

* 快速开始

* 如何注册 Source/Sink

* 如何创建 Pipeline

* 如何监控 Actor 日志

## 🏗 三、项目目录结构（开源友好）

```css
AkkaSync/
├─ src/
│  ├─ AkkaSync.Core/
│  │    ├─ Actors/
│  │    │    ├─ PipelineActor.cs
│  │    │    ├─ ExtractActor.cs
│  │    │    ├─ TransformActor.cs
│  │    │    ├─ LoadActor.cs
│  │    │    └─ SupervisorStrategy.cs
│  │    ├─ Pipeline/
│  │    │    ├─ SyncPipeline.cs
│  │    │    ├─ CheckpointStore.cs
│  │    │    └─ ISyncSource.cs / ISyncSink.cs / ITransformer.cs
│  │    ├─ Messaging/
│  │    ├─ Configuration/
│  │    └─ Utilities/
│  │
│  ├─ AkkaSync.Plugins/
│  │    ├─ FileSource/
│  │    ├─ SqlServerSource/
│  │    ├─ ApiSource/
│  │    ├─ SqliteSink/
│  │    ├─ KafkaSink/
│  │    └─ CustomTransformer/
│  │
│  ├─ AkkaSync.Demo/
│  │    ├─ Examples/
│  │    │    └─ FileToSqlite/
│  │    └─ DemoConsole/
│  │
│  └─ AkkaSync.Management/
│       ├─ CLI/
│       └─ WebDashboard (未来可加)
├─ tests/
│  ├─ AkkaSync.Core.Tests/
│  └─ AkkaSync.Plugins.Tests/
├─ docs/
│  ├─ Architecture.md
│  ├─ GettingStarted.md
│  └─ PipelineExamples.md
└─ README.md

```

## ⭐ 四、推荐的 Demo 场景（不耦合具体业务）

以下是可以在 README 中用来展示能力但不影响通用性的场景：

1) 文件 → 数据库（JSON/CSV → SQLite）

    最简单也最易理解的同步流程
    适合 Demo

2) API → Database

    模拟从第三方拉取数据，例如：
```css
HttpSource → Transform → SqlServerSink
```

3) Database → Message Queue

    用于事件驱动同步
```css
SqlServer CDC Source → Transform → KafkaSink
```

4) 多节点分布同步（Akka Cluster 模式）

    展示 Akka 的强项：
```css
LargeFileSource (sharded) → Transform → DistributedSink
```

5) IoT 数据汇聚（轻度示范）

    不是设备端，而是汇聚端：
```css
MQTT broker → Transform → Timeseries DB Sink
```
## 🚀 五、未来规划（打造明星开源项目的路线图）

### Phase 1 — MVP（1~2个月）

* Pipeline Actor 完整流程

* File → SQLite Demo

* 插件结构设计

* 文档、README、示例

### Phase 2 — 实战增强（2~4个月）

* 更多插件：Kafka、SQL Server、Postgres

* Dashboard（ASP.NET + Next.js）监控同步状态

* Distributed Pipeline (Akka Cluster)

* Checkpoint 的多种实现：Redis、Postgres、LiteDB

### Phase 3 — 商业级能力（4~8个月）

* At-least-once & Exactly-once 流水线逻辑

* Dead Letter 队列管理

* Backpressure 全链路控制

* 数据转换 DSL

* SDK 跨语言支持（Java/Kotlin 后续可能支持）

### Phase 4 — 开源影响力（长期）

* 写 Medium/知乎/Blog 系列文章

* 与 Dapr、MassTransit 对比

* 提供 Benchmark

* GitHub 开源视频介绍

* 录制 YouTube 教程



| purpose        | 中文说明       | scheduleId 示例                 | cron 表达式            | 说明                        |
| -------------- | ---------- | ----------------------------- | ------------------- | ------------------------- |
| every-5-min    | 每 5 分钟运行   | `sync-orders::every-5-min`    | `*/5 * * * *`       | 每 5 分钟                    |
| every-15-min   | 每 15 分钟运行  | `sync-orders::every-15-min`   | `*/15 * * * *`      | 每 15 分钟                   |
| hourly         | 每小时运行      | `sync-orders::hourly`         | `0 * * * *`         | 每小时整点                     |
| daily          | 每天固定时间运行   | `sync-orders::daily`          | `0 3 * * *`         | 每天 03:00                  |
| nightly        | 每夜固定时间（凌晨） | `sync-orders::nightly`        | `0 1 * * *`         | 每天 01:00                  |
| weekly         | 每周运行       | `sync-orders::weekly`         | `0 3 * * 1`         | 每周一 03:00                 |
| monthly        | 每月运行       | `sync-orders::monthly`        | `0 3 1 * *`         | 每月 1 日 03:00              |
| business-hours | 仅工作时间内运行   | `sync-orders::business-hours` | `*/15 9-17 * * 1-5` | 周一至周五 09:00–17:59 每 15 分钟 |
| weekend        | 周末运行       | `sync-orders::weekend`        | `0 10 * * 6,0`      | 周六、周日 10:00               |






# AkkaSync

**AkkaSync** is a lightweight, extensible data-synchronization framework built on the **Akka.NET actor model**.
It offers a configuration-driven **Extract → Transform → Load (ETL)**  pipeline featuring **high concurrency**, **failure isolation**, and **scalable parallel processing**.

AkkaSync was initially designed to support **reliable**, **concurrent**, and **parallel** data transformation and synchronization across heterogeneous storage mediums—such as files, databases, and message queues—commonly found in distributed application environments.

The framework adopts a **plugin-oriented architecture**, defining four extensible module types:

- **Source**  – Responsible for extracting data from external systems or storage mediums.
- **Transformer**  – Applies business logic to convert, filter, or reshape the extracted data.
- **Sink**  – Persists the processed data into the target system or storage layer.
- **HistoryStore** – Maintains synchronization metadata to support incremental and reliable processing.

This modular design allows the system to evolve organically, making it easier to introduce new capabilities and adapt to broader integration scenarios over time.
The project structure is intentionally kept flexible: the core runtime and plugin modules are isolated from each other, and each plugin can be developed, versioned, and published independently. This allows users to depend only on the components they need while keeping integrations clean and maintainable.


## 🧩 Architecture Overview

### Concurrent and Parallel Pipelines in AkkaSync
![AkkaSync Diagram](./assets/akkasync-outside.png)

### Actors & Plugins in AkkaSync
![AkkaSync Actor & Plugin](./assets/akkasync-actor-plugin.png)

## 📘 Architecture Components

### **1. PipelineManagerActor**
Manages global orchestration:

- Loads pipeline definitions from configuration  
- Builds and validates dependency DAG  
- Starts pipeline execution  
- Supervises PipelineActor lifecycle  

➡️ *See: [PipelineManagerActor](./docs/pipeline-manager.md)*

### **2. PipelineActor**
Owns execution of a single pipeline:

- Starts sync steps in correct order  
- Spawns and supervises SyncWorkerActor  
- Handles backoff, retries, and failures  
- Reports progress to the manager  

➡️ *See: [PipelineActor](./docs/pipeline.md)*

### **3. SyncWorkerActor**
Handles actual business execution:

- Invokes data source and sink plugins  
- Performs sync logic  
- Reports cursor & progress  
- Isolated, restartable, testable  

➡️ *See: [SyncWorkerActor](./docs/worker.md)*

---

### **4. Plugins**
Plugins enable extensibility:

- **Source plugins**: CSV, SQL, API...  
- **Sink plugins**: Sqlite, SqlServer, ElasticSearch  
- **Transform plugins**: Clean, map, enrich  

Each plugin runs inside a worker, making the system highly modular.

➡️ *See: [Plugins](./docs/plugins.md)*

## Development Log

Curious about what’s been built, what’s in progress, or what’s coming next? Check out our [Development Log](./docs/DEVELOPMENT_LOG.md) to see the current roadmap, planned features, and ongoing work. This helps contributors and users stay up-to-date with AkkaSync’s progress.

