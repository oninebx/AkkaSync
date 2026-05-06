// // app/pipelines/[id]/_components/nodes/PluginRuntimeNode.tsx
// import { memo } from "react";
// import { Handle, Position } from "@xyflow/react";
// import { Activity, AlertCircle } from "lucide-react";
// import { cn } from "@/lib/utils";

// const PluginRuntimeNode = ({ data, selected }: any) => {
//   return (
//     <div className={cn(
//       "px-4 py-3 rounded-lg border-2 bg-white min-w-[200px] shadow-sm",
//       selected ? "border-blue-500 ring-2 ring-blue-100" : "border-slate-200"
//     )}>
//       <Handle type="target" position={Position.Left} className="w-2 h-2 !bg-slate-400" />
      
//       {/* Header: 只有名称和类型 */}
//       <div className="flex items-center justify-between mb-3 pb-2 border-b border-slate-50">
//         <div>
//           <div className="text-[10px] uppercase tracking-wider text-slate-400 font-bold">
//             {data.kind}
//           </div>
//           <div className="text-sm font-bold text-slate-700">{data.name}</div>
//         </div>
//         <div className={cn(
//           "w-2 h-2 rounded-full",
//           data.isRunning ? "bg-emerald-500 animate-pulse shadow-[0_0_8px_rgba(16,185,129,0.6)]" : "bg-slate-300"
//         )} />
//       </div>

//       {/* Body: 纯粹的指标展示 */}
//       <div className="space-y-3">
//         <div className="flex items-center justify-between bg-slate-50 p-2 rounded">
//           <div className="flex items-center gap-2 text-xs text-slate-500">
//             <Activity className="w-3.5 h-3.5 text-blue-500" />
//             Processed
//           </div>
//           <span className="text-sm font-mono font-bold">{data.processed}</span>
//         </div>

//         <div className={cn(
//           "flex items-center justify-between p-2 rounded",
//           data.errors > 0 ? "bg-red-50" : "bg-slate-50"
//         )}>
//           <div className="flex items-center gap-2 text-xs text-slate-500">
//             <AlertCircle className={cn("w-3.5 h-3.5", data.errors > 0 ? "text-red-500" : "text-slate-400")} />
//             Errors
//           </div>
//           <span className={cn(
//             "text-sm font-mono font-bold",
//             data.errors > 0 ? "text-red-600" : "text-slate-500"
//           )}>{data.errors}</span>
//         </div>
//       </div>

//       {/* Footer: 实例标识 */}
//       <div className="mt-2 pt-2 border-t border-dashed border-slate-100 text-[9px] text-slate-400 font-mono italic">
//         Instance: {data.instanceId.slice(0, 8)}...
//       </div>

//       <Handle type="source" position={Position.Right} className="w-2 h-2 !bg-slate-400" />
//     </div>
//   );
// };

// export default memo(PluginRuntimeNode);

// app/pipelines/[id]/_components/nodes/PluginRuntimeNode.tsx
// app/pipelines/[id]/_components/nodes/PluginRuntimeNode.tsx
import { Handle, NodeProps, Position } from "@xyflow/react";
import { PluginRuntimeNodeData } from "../types"; // 注意路径
import { Activity, AlertCircle, PlayCircle, StopCircle, LucideIcon } from "lucide-react";
import { memo } from "react";
import { cn } from "@/lib/utils";

type PluginRuntimeNodeProps = NodeProps<PluginRuntimeNodeData>;

interface RuntimeStyle {
  color: string;
  bg: string;
  text: string;
  icon: LucideIcon;
  label: string;
}

const RUNTIME_CONFIG: Record<'running' | 'idle', RuntimeStyle> = {
  running: {
    color: 'border-emerald-500',
    bg: 'bg-emerald-50',
    text: 'text-emerald-700',
    icon: PlayCircle,
    label: 'Running',
  },
  idle: {
    color: 'border-slate-300',
    bg: 'bg-slate-50',
    text: 'text-slate-500',
    icon: StopCircle,
    label: 'Idle',
  },
};

const PluginRuntimeNode = ({ data, selected }: PluginRuntimeNodeProps) => {
  const status = data.isRunning ? RUNTIME_CONFIG.running : RUNTIME_CONFIG.idle;
  const StatusIcon = status.icon;

  return (
    <div
      className={cn(
        'min-w-[240px] rounded-xl border-2 bg-white shadow-sm transition-all duration-200 relative',
        status.color,
        selected ? 'ring-2 ring-offset-2 ring-blue-400 shadow-md' : 'hover:shadow-md'
      )}
    >
      {/* Target Handle: 放在左侧 */}
      <Handle
        type="target"
        position={Position.Left}
        className="w-3 h-3 !bg-slate-300 border-2 border-white hover:!bg-blue-400 transition-colors"
      />

      {/* Header: 保持一致的背景和样式 */}
      <div className={cn('flex items-center justify-between px-3 py-2 border-b', status.bg)}>
        <div className="flex items-center gap-2">
          <StatusIcon className={cn('w-4 h-4', status.text, data.isRunning && "animate-pulse")} />
          <span className="font-bold text-slate-800 text-sm truncate max-w-[120px]">
            {data.name}
          </span>
        </div>
        <span className="text-[10px] font-bold px-1.5 py-0.5 rounded bg-white/60 text-slate-500 border border-slate-200 uppercase tracking-tight">
          {data.kind}
        </span>
      </div>

      <div className="p-3 space-y-3">
        {/* 指标展示区：替换掉原有的 Provider 部分 */}
        <div className="grid grid-cols-2 gap-3">
          <div className="flex flex-col gap-0.5">
            <label className="text-[9px] text-slate-400 uppercase font-bold tracking-wider flex items-center gap-1">
              <Activity className="w-2.5 h-2.5" /> Processed
            </label>
            <span className="text-[13px] font-mono font-bold text-slate-700 bg-slate-50 px-2 py-1 rounded border border-slate-100 text-center">
              {data.processed.toLocaleString()}
            </span>
          </div>

          <div className="flex flex-col gap-0.5">
            <label className="text-[9px] text-slate-400 uppercase font-bold tracking-wider flex items-center gap-1">
              <AlertCircle className="w-2.5 h-2.5" /> Errors
            </label>
            <span className={cn(
              "text-[13px] font-mono font-bold px-2 py-1 rounded border text-center",
              data.errors > 0 
                ? "bg-red-50 border-red-100 text-red-600" 
                : "bg-slate-50 border-slate-100 text-slate-400"
            )}>
              {data.errors.toLocaleString()}
            </span>
          </div>
        </div>

        <div className="h-px bg-slate-100 w-full" />

        {/* Footer: 显示实例 ID 和状态标签 */}
        <div className="flex items-center justify-between">
          <div className={cn('flex items-center gap-1.5 text-xs font-semibold', status.text)}>
            <span>{status.label}</span>
          </div>
          
          <div className="flex items-center gap-1">
             <span className="text-[9px] text-slate-300 font-mono">ID:</span>
             <span className="text-[10px] text-slate-400 font-mono font-medium">
                {data.id.slice(-8)}
             </span>
          </div>
        </div>
      </div>

      {/* Source Handle: 放在右侧 */}
      <Handle
        type="source"
        position={Position.Right}
        className="w-3 h-3 !bg-slate-300 border-2 border-white hover:!bg-blue-400 transition-colors"
      />
    </div>
  );
};

export default memo(PluginRuntimeNode);