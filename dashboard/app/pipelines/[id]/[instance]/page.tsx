'use client'
import { useParams, usePathname } from "next/navigation";
import { ConnectorNode, PluginRuntimeNode } from "../_components";
import PipelineTopologyBase from "../_components/PipelineTopologyBase";
import { selectRuntimeTopology } from "./selectors";

const nodeTypes = { connector: ConnectorNode, runtimePlugin: PluginRuntimeNode };

export default function RuntimePage() {
   const { id, instance } = useParams<{id: string; instance: string}>();
   const pathName = usePathname();
  return (
    <PipelineTopologyBase
      id={id}
      instanceId={instance}
      title="Runtime Monitor"
      selector={selectRuntimeTopology}
      nodeTypes={nodeTypes}
      path={pathName}
    />
  );
}