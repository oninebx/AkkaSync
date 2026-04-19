import { PluginInstance } from "@/features/execution/pluginGraph.type";
import BaseNode, { PluginNodeProps } from "./BaseNode";
import { NodeProps } from "@xyflow/react";
import { PluginNodeData } from "../../types";

export function SourceNode({ data }: PluginNodeProps) {
  return <BaseNode data={data} color="#16a34a" label="Source" icon="source" isSource isTarget={false} />;
}