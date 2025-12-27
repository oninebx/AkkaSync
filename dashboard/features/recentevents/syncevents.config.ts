import { DashboardEventType, EventEnvelope } from "./syncevents.types";
import { EventItem, EventLevel } from "./syncevents.viewmodels";

const eventDisplayMap: Record<DashboardEventType, {level: EventLevel, formatter: (source:string) => string}> = 
{
  [DashboardEventType.PipelineStarted]: { level: 'INFO', formatter: source => `Pipeline ${source} started.` },
  [DashboardEventType.PipelineCompleted]: { level: 'INFO', formatter: source =>  `Pipeline ${source} completed.`}
};


export const mapEnvelope = (envelope: EventEnvelope): EventItem => {
  const {level, formatter} = eventDisplayMap[envelope.type];
  return {
    time: envelope.occurredAt,
    level,
    message: formatter(envelope.source)
  };
};
