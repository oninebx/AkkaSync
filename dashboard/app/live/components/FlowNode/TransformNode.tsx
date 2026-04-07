import { PluginInstance } from "@/features/plugin-graph/pluginGraph.type";
import BaseNode from "./BaseNode";

export function TransformNode({ data }: { data: PluginInstance }) {
  return <BaseNode data={data} color="#3b82f6" label="Transform" icon="transform" isSource isTarget />;
}