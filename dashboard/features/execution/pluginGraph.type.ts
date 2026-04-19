import { PluginType } from "@/contracts/plugin/types";

type PluginInstance = {
  id: string;
  name: string;
  pluginKey: string;
  pipelineId: string;
  type: PluginType;
  status: 'idle' | 'running' | 'succeeded' | 'failed';
  stats: {
    processed: number;
    errors: number;
  };
}

interface PluginEdge {
  id: string;
  pipelineId: string;
  from: string; // source plugin instance ID
  to: string;   // target plugin instance ID  
}

export type {
  PluginInstance,
  PluginEdge
}