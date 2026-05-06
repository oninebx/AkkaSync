'use client'
import { TableCard } from "@/components/TableCard";
import { useSelector } from "react-redux";
import { selectPipelineRows } from "./selectors";
import { Column } from "@/components/DisplayTable";
import { PipelineRow } from "./types";
import { cn } from "@/lib/utils";
import Link from "next/link";
import { Countdown } from "@/components/CountDown";
import { PipelineStatus } from "@/features/pipelines/types";

const statusStyles: Record<keyof typeof PipelineStatus, string> = {
  RUNNING: 'bg-blue-50 text-blue-600 border-blue-100',
  SUCCESS: 'bg-emerald-50 text-emerald-600 border-emerald-100',
  FAILED: 'bg-red-50 text-red-600 border-red-100',
  RETRYING: 'bg-amber-50 text-amber-600 border-amber-100',
  SKIPPED: 'bg-slate-50 text-slate-400 border-slate-200',
  UNKNOWN: 'bg-gray-50 text-gray-500 border-gray-100',
};

const columns: Column<PipelineRow>[] = [
    {
      header: 'Pipeline Info',
      key: 'name',
      render: (item) => (
        <Link
          href={`/pipelines/${item.id}`}
         className="flex flex-col group cursor-pointer">
          <span className="font-bold text-slate-800">{item.name}</span>
          <span className="text-[10px] font-mono text-slate-400">{item.id}</span>
        </Link>
      ),
    },
    {
      header: 'Schedule',
      key: 'scheduleText',
      render: (item) => (
        <div className="flex flex-col group">
          <span className="text-slate-600 text-sm">{item.scheduleText}</span>
          <span className="text-[10px] text-slate-300 font-mono hidden group-hover:block italic">
            {item.scheduleText}
          </span>
        </div>
      ),
    },
    {
      header: 'Status',
      key: 'status',
      render: (item) => {
        
        
        return (
          // <span className={cn(
          //   'px-2 py-1 rounded border text-[11px] font-bold uppercase tracking-wider',
          //   statusStyles[item.status]
          // )}>
          //   {item.status}
          // </span>
          <Link 
            href={`/pipelines/${item.id}/running`}
            // 阻止冒泡：防止点击状态标签时触发整行的点击事件
            onClick={(e) => e.stopPropagation()}
            className={cn(
              'px-2 py-1 rounded border text-[11px] font-bold uppercase tracking-wider transition-all',
              'hover:brightness-90 active:scale-95 cursor-pointer', // 增加点击反馈
              statusStyles[item.status]
            )}
          >
            {item.status}
          </Link>
        );
      },
    },
    {
      header: 'Metrics',
      key: 'lastRun',
      render: (item) => (
        <div className="flex flex-col">
          <div className="text-slate-600 flex items-center gap-1">
            <span className="text-xs text-slate-400 font-medium">Last:</span>
            <span>{item.lastRun}</span>
          </div>
          {item.errors > 0 && (
            <span className="text-[10px] text-red-500 font-bold">
              {item.errors} Errors Detected
            </span>
          )}
        </div>
      ),
    },
    {
      header: 'Next Run',
      key: 'nextRun',
      render: (item) => {
        if (!item.nextRun) {
          return <span className="text-slate-400 text-xs">—</span>;
        }

        return (
          <Countdown 
            target={item.nextRun} 
            expiredLabel="Pending..." 
            className="text-xs font-bold text-slate-600"
          />
        );
      }
    }
  ];

const PipelineHome = () => {
  const data = useSelector(selectPipelineRows);
  
  return (
    <div className="p-8 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Page Header */}
        <div className="flex justify-between items-end pb-2 border-b border-slate-200">
          <div>
            <h1 className="text-3xl font-black text-slate-900 tracking-tight">Pipeline Monitor</h1>
            <p className="text-sm text-slate-500 mt-1 font-medium">
              Oversee and manage data synchronization workflows
            </p>
          </div>
          <div className="text-sm font-semibold text-slate-400 uppercase tracking-widest">
            Total: <span className="text-slate-900">{data.length}</span>
          </div>
        </div>

        {/* Utilizing your TableCard component */}
        <TableCard
          title="Active Pipelines"
          data={data}
          columns={columns}
        />
      </div>
    </div>
  );
}

export default PipelineHome;