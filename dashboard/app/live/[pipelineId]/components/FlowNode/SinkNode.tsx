import { PluginInstance } from "@/features/execution/pluginGraph.type";
import BaseNode, { PluginNodeProps } from "./BaseNode";
import { NodeProps } from "@xyflow/react";
import { PluginNodeData } from "../../types";

export function SinkNode({ data }: PluginNodeProps) {
  return <BaseNode data={data} color="#ef4444" label="Sink" icon="sink" isTarget isSource={false} />;
}