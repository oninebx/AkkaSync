interface PipelineJob {
  id: string,
  name: string,
  nextUtc: string
}

interface PipelineSchedules {
  specs: Record<string, string>,
  jobs: PipelineJob[]
}

export type {
  PipelineJob,
  PipelineSchedules
}