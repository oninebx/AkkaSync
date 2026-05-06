import { Handle, NodeProps, Position } from "@xyflow/react";
import { PluginNodeData } from "../types";
import { PluginHealthStatus } from "@/contracts/plugin/types";
import { AlertCircle, ArrowUpCircle, CheckCircle2, DownloadCloud, FileWarning, LucideIcon } from "lucide-react";
import { memo } from "react";
import { cn } from "@/lib/utils";

type PluginNodeProps = NodeProps<PluginNodeData>;

interface StatusStyle {
  color: string;
  bg: string;
  text: string;
  icon: LucideIcon;
  label: string;
}

const HEALTH_CONFIG: Record<PluginHealthStatus, StatusStyle> = {
  loaded: {
    color: 'border-emerald-500',
    bg: 'bg-emerald-50',
    text: 'text-emerald-700',
    icon: CheckCircle2,
    label: 'Ready',
  },
  loadFailed: {
    color: 'border-red-500',
    bg: 'bg-red-50',
    text: 'text-red-700',
    icon: AlertCircle,
    label: 'Load Failed',
  },
  notFound: {
    color: 'border-slate-400',
    bg: 'bg-slate-50',
    text: 'text-slate-600',
    icon: FileWarning,
    label: 'Not Found',
  },
  notDownloaded: {
    color: 'border-amber-500',
    bg: 'bg-amber-50',
    text: 'text-amber-700',
    icon: DownloadCloud,
    label: 'Pending Sync',
  },
  updateAvailable: {
    color: 'border-blue-500',
    bg: 'bg-blue-50',
    text: 'text-blue-700',
    icon: ArrowUpCircle,
    label: 'Update Available',
  },
};

const PluginNode = ({data, selected, id}: PluginNodeProps) => {
  const status = HEALTH_CONFIG[data.health] || HEALTH_CONFIG.notFound;
  const StatusIcon = status.icon;

  return (
    <div
      className={cn(
        'min-w-[240px] rounded-xl border-2 bg-white shadow-sm transition-all duration-200',
        status.color,
        selected ? 'ring-2 ring-offset-2 ring-blue-400 shadow-md' : 'hover:shadow-md'
      )}
    >
      <div className={cn('flex items-center justify-between px-3 py-2 border-b', status.bg)}>
        <div className="flex items-center gap-2">
          <StatusIcon className={cn('w-4 h-4', status.text)} />
          <span className="font-bold text-slate-800 text-sm truncate max-w-[120px]">
            {data.name}
          </span>
        </div>
        <span className="text-[10px] font-bold px-1.5 py-0.5 rounded bg-white/60 text-slate-500 border border-slate-200 uppercase tracking-tight">
          {data.kind}
        </span>
      </div>

      <div className="p-3 space-y-3">
        <div className="flex flex-col gap-0.5">
          <label className="text-[9px] text-slate-400 uppercase font-bold tracking-wider">Provider</label>
          <code className="text-[11px] text-slate-600 font-mono truncate bg-slate-50 px-1 py-0.5 rounded">
            {data.provider}
          </code>
        </div>

        <div className="h-px bg-slate-100 w-full" />

        <div className="flex items-center justify-between">
          <div className={cn('flex items-center gap-1.5 text-xs font-semibold', status.text)}>
            <span>{status.label}</span>
          </div>
          
          {data.version && (
            <span className="text-[10px] text-slate-400 font-medium">
              v{data.version}
              {data.health === 'updateAvailable' && (
                <span className="text-blue-500 ml-1">→ v{data.latestVersion}</span>
              )}
            </span>
          )}
        </div>
      </div>
      <Handle
        type="target"
        position={Position.Left}
        className="w-3 h-3 !bg-slate-300 border-2 border-white hover:!bg-blue-400 transition-colors"
      />
      <Handle
        type="source"
        position={Position.Right}
        className="w-3 h-3 !bg-slate-300 border-2 border-white hover:!bg-blue-400 transition-colors"
      />
    </div>
  );
};

export default memo(PluginNode);