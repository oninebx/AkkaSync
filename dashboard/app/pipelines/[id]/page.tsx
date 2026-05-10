'use client'
import { QueryResponse, useSignalRQuery } from "@/infrastructure/realtime/SignalRProvider";

import { ConnectorNode, PluginNode } from "./_components";
import PipelineTopologyBase from "./_components/PipelineTopologyBase";
import { selectPipelineTopology } from "./selectors";
import { useParams, usePathname } from "next/navigation";

const nodeTypes = { connector: ConnectorNode, plugin: PluginNode };

export default function PipelinePage() {
  const { id } = useParams<{id: string}>();
  const pathName = usePathname();
  const { data } = useSignalRQuery<QueryResponse>('CheckForUpdates');
  return (
    <PipelineTopologyBase
      id={id}
      title="Pipeline Blueprint"
      selector={selectPipelineTopology}
      nodeTypes={nodeTypes}
      path={pathName}
    />
  );
}