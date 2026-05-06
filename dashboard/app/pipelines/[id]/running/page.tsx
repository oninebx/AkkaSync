'use client'
import { useParams, usePathname } from "next/navigation";
import { ConnectorNode, PluginRuntimeNode } from "../_components";
import PipelineTopologyBase from "../_components/PipelineTopologyBase";
import { selectRuntimeTopology } from "./selectors";

const nodeTypes = { connector: ConnectorNode, runtimePlugin: PluginRuntimeNode };

export default function RuntimePage() {
   const { id } = useParams<{id: string}>();
   const pathName = usePathname();
  return (
    <PipelineTopologyBase
      id={id}
      title="Runtime Monitor"
      selector={selectRuntimeTopology}
      nodeTypes={nodeTypes}
      path={pathName}
    />
  );
}