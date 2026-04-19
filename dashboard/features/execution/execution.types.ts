
interface PluginInstance {
  id: string;
  name: string;
  type: string;
  pipelineId: string;
  dependencies: string[];
}

interface PluginLive {
  id: string; // plugin instance ID
  processed?: number;
  errors?: number;
}