interface PluginDefinition {
  identifier: string;
  key: string;
  type: string;
  pipeline: string;
  dependsOn: string[]
}

interface PluginLocal {
  identifier: string;
  name: string;
  version: string;
  provider: string;
  kind: string;
}

interface PluginRemote {
  identifier: string;
  qualifiedName: string;
  version: string;
}

export type {
  PluginDefinition,
  PluginLocal,
  PluginRemote
}