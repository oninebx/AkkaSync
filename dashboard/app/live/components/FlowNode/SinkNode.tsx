import { PluginInstance } from "@/features/plugin-graph/pluginGraph.type";
import BaseNode from "./BaseNode";

export function SinkNode({ data }: { data: PluginInstance }) {
  return <BaseNode data={data} color="#ef4444" label="Sink" icon="sink" isTarget isSource={false} />;
}