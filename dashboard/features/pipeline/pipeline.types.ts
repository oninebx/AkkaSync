interface Pipeline {
  id: string;
  name: string;
  schedule: string;
  sourcePluginId: string;
  transformerPluginid: string;
  sinkPluginId: string;
  startAt: string;
  finishAt: string;
}

export type {
  Pipeline
}