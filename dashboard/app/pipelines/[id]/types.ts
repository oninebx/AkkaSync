import { PluginHealthStatus, PluginKind } from '@/contracts/plugin/types';
import { Node, Edge } from '@xyflow/react';

type ConnectorNodePayload = {
  id: string;
  name: string;
  role: 'SOURCE' | 'SINK'
}

type PluginNodePayload = {
  id: string;
  name: string;
  provider: string;
  kind: string;
  health: PluginHealthStatus;
  version?: string;
  latestVersion?: string;
};

type ConnectorNodeData = Node<ConnectorNodePayload, 'connector'>;
type PluginNodeData = Node<PluginNodePayload, 'plugin'>;
type FlowNodeData = ConnectorNodeData | PluginNodeData;

type FlowEdge = Edge;

export type {
  ConnectorNodeData,
  PluginNodeData,
  FlowNodeData,
  FlowEdge
}