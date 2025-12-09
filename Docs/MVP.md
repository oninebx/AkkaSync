```pgsql               
               +----------------------+
               |    PipelineActor     |
               |  (Orchestrator)      |
               +-----------+----------+
                           |
                           |
          +----------------+----------------+
          |                                 |
    +-----v------+                    +------v------+
    | ExtractActor|   Raw Batch       |TransformActor|
    |   (Source)  +------------------>+ (Transformer)|
    +------+------/                   +------+-------+
           |                                 |
           | Transformed Batch               |
           v                                 v
      +----+-----------+        Checkpoint + Batch
      |   LoadActor    +----------------------------+
      |     (Sink)     |                            |
      +----------------+                            |
               |                                     |
               +-----------> PipelineActor <----------+
                         checkpoint update
```


AkkaSync/ ├─ src/ │ ├─ AkkaSync.Core/ │ │ ├─ Actors/ │ │ │ ├─ PipelineActor.cs │ │ │ ├─ ExtractActor.cs │ │ │ ├─ TransformActor.cs │ │ │ ├─ LoadActor.cs │ │ │ └─ SupervisorStrategy.cs │ │ ├─ Pipeline/ │ │ │ ├─ SyncPipeline.cs │ │ │ ├─ CheckpointStore.cs │ │ │ └─ ISyncSource.cs / ISyncSink.cs / ITransformer.cs │ │ ├─ Messaging/ │ │ ├─ Configuration/ │ │ └─ Utilities/ │ │ │ ├─ AkkaSync.Plugins/ │ │ ├─ FileSource/ │ │ ├─ SqlServerSource/ │ │ ├─ ApiSource/ │ │ ├─ SqliteSink/ │ │ ├─ KafkaSink/ │ │ └─ CustomTransformer/ │ │ │ ├─ AkkaSync.Demo/ │ │ ├─ Examples/ │ │ │ └─ FileToSqlite/ │ │ └─ DemoConsole/ │ │ │ └─ AkkaSync.Management/ │ ├─ CLI/ │ └─ WebDashboard (未来可加) ├─ tests/ │ ├─ AkkaSync.Core.Tests/ │ └─ AkkaSync.Plugins.Tests/ ├─ docs/ │ ├─ Architecture.md │ ├─ GettingStarted.md │ └─ PipelineExamples.md └─ README.md


```mermaid
flowchart TD
    A[用户点击 Run Pipeline] --> B[PipelineManagerActor 收到 StartPipeline 消息]
    B --> C{依赖检查: 所有前置 Pipeline 是否 Completed?}
    C -- 否 --> D[返回提示：依赖未完成，阻止启动]
    C -- 是 --> E[Pipeline 状态设为 Running]
    E --> F[启动 Pipeline Actor 或 Task 执行 Pipeline]
    F --> G{Pipeline 执行完成?}
    G -- 成功 --> H[状态设为 Completed]
    G -- 失败 --> I[状态设为 Failed, 返回错误信息]
    H --> J{自动触发下一层? AutoRunDownstream = true}
    J -- 是 --> B[检查并启动下一层 Pipeline]
    J -- 否 --> K[等待用户手动启动下一层 Pipeline]
```