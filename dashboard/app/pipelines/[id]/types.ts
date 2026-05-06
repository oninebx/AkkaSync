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

type PluginRuntimeNodePayload = {
  id: string;
  name: string;
  kind: string;
  processed: number;
  errors: number;
  isRunning: boolean;
};

type ConnectorNodeData = Node<ConnectorNodePayload, 'connector'>;
type PluginNodeData = Node<PluginNodePayload, 'plugin'>;
type PluginRuntimeNodeData = Node<PluginRuntimeNodePayload, 'runtimePlugin'>;
type FlowNodeData = ConnectorNodeData | PluginNodeData | PluginRuntimeNodeData;

type FlowEdge = Edge;

export type {
  ConnectorNodeData,
  PluginNodeData,
  PluginRuntimeNodeData,
  FlowNodeData,
  FlowEdge
}