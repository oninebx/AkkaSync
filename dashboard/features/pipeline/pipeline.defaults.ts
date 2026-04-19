export const DEFAULT_OVERVIEW = {
  runCount: 0,
  totalProcessed: 0,
  totalErrors: 0,
  schedule: '-'
};

// Live 默认值
export const DEFAULT_LIVE = {
  status: 'idle' as const,
  processed: 0,
  errors: 0,
  currentRunTime: 0,
  nextRunTime: 0
};