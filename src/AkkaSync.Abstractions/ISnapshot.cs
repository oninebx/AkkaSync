namespace AkkaSync.Abstractions;

public interface ISnapshot
{
  string Identifier { get; }
}

///// <summary>
///// Pipeline 领域状态快照
///// 管理 ETL 管道的定义和运行实例
///// 
///// 职责:
/////   - 存储管道配置 (Source, Transforms, Sinks, Plugins)
/////   - 跟踪管道执行实例的生命周期
/////   - 汇总指标和运行状态
/////   
///// 实现: PipelineState (src/AkkaSync.Host/Application/Pipeline)
///// 事件: PipelineCreatedTransition, PipelineStartReported, PipelineCompleteReported
///// Reducer: PipelineReducer
///// Mapping: PipelineStateMapping
///// </summary>
//public interface IPipelineStateSnapshot : ISnapshot { }

///// <summary>
///// Worker 领域状态快照
///// 管理数据同步执行单位的生命周期和性能指标
///// 
///// 职责:
/////   - 跟踪 Worker 实例的启动、运行和完成
/////   - 汇总每个 Worker 的性能指标 (处理数、错误数)
/////   - 管理数据处理进度 (cursor)
/////   
///// 实现: WorkerState (src/AkkaSync.Host/Application/RuntimeTopology)
///// 事件: WorkerStartReported, WorkerCompleteReported, WorkerMetricsReported
///// Reducer: WorkerReducer
///// Mapping: WorkerStateMapping
///// </summary>
//public interface IWorkerStateSnapshot : ISnapshot { }

///// <summary>
///// Scheduling 领域状态快照
///// 管理 Pipeline 的定时执行和任务队列
///// 
///// 职责:
/////   - 存储调度规则 (Cron 表达式等)
/////   - 维护待执行任务队列
/////   - 跟踪下次执行时间
/////   
///// 实现: ScheduleState (src/AkkaSync.Host/Application/Scheduling)
///// 事件: PipelineScheduled, PipelineTriggered, SchedulerInitialized
///// Reducer: ScheduleStateReducer
///// Mapping: ScheduleStateMapping
///// </summary>
//public interface IScheduleStateSnapshot : ISnapshot { }

///// <summary>
///// Plugin 领域状态快照
///// 管理 Source、Transform、Sink 等插件的注册和缓存
///// 
///// 职责:
/////   - 维护插件注册表
/////   - 管理本地缓存
/////   - 跟踪插件版本和依赖关系
/////   
///// 实现: PluginState (src/AkkaSync.Host/Application/SyncPlugin)
///// 事件: PluginLoaded, PluginUpdated, PluginVersionConflict (可扩展)
///// Reducer: PluginStateReducer
///// Mapping: PluginStateMapping
///// </summary>
//public interface IPluginStateSnapshot : ISnapshot { }

///// <summary>
///// Syncing 领域状态快照
///// 管理当前活跃数据同步会话中的插件实例
///// 
///// 职责:
/////   - 跟踪活跃的同步会话
/////   - 维护会话中的插件实例映射
/////   - 管理会话级别的资源
/////   
///// 实现: SyncingState (src/AkkaSync.Host/Application/Syncing)
///// 事件: SyncSessionStarted, SyncSessionEnded, SyncInstanceRegistered
///// Reducer: SyncingStateReducer
///// Mapping: SyncingStateMapping
///// </summary>
//public interface ISyncingStateSnapshot : ISnapshot { }

///// <summary>
///// Host/Connection 领域状态快照
///// 管理整个 SyncEngine 的生命周期和全局同步状态
///// 
///// 职责:
/////   - 跟踪主机整体运行状态 (Idle, Running, Stopping)
/////   - 记录启动和停止时间
/////   - 管理全局健康检查和就绪状态
/////   
///// 实现: SyncState (src/AkkaSync.Host/Application/Connecting)
///// 事件: SyncEngineReady, SyncEngineStopped, DashboardInitialized
///// Reducer: SyncStateReducer (可为 SyncState)
///// Mapping: SyncingStateMapping
///// </summary>
//public interface IHostStateSnapshot : ISnapshot { }
