import { PluginInstance } from "@/features/plugin-graph/pluginGraph.type";
import BaseNode from "./BaseNode";

export function SourceNode({ data }: { data: PluginInstance }) {
  return <BaseNode data={data} color="#16a34a" label="Source" icon="source" isSource isTarget={false} />;
}