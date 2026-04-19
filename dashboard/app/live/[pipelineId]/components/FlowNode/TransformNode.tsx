import { PluginInstance } from "@/features/execution/pluginGraph.type";
import BaseNode, { PluginNodeProps } from "./BaseNode";
import { PluginNodeData } from "../../types";
import { NodeProps } from "@xyflow/react";

export function TransformNode({ data }: PluginNodeProps) {
  return <BaseNode data={data} color="#3b82f6" label="Transform" icon="transform" isSource isTarget />;
}