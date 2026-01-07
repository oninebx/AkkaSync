import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { schedulerActions } from "./scheduler.slice";
import { PipelineJob, PipelineSchedules } from "./scheduler.types";

envelopeHandlerMap.set('scheduler.specs.initialized', (event, dispatch) => 
  dispatch(schedulerActions.setSpecs(event.payload as Record<string, string>)));
envelopeHandlerMap.set('scheduler.none', (event, dispatch) =>
  dispatch(schedulerActions.setSchedules(event.payload as PipelineSchedules)));
envelopeHandlerMap.set('scheduler.jobs.added', (event, dispatch) =>
  dispatch(schedulerActions.addJob(event.payload as PipelineJob)));
envelopeHandlerMap.set('scheduler.jobs.removed', (event, dispatch) =>
  dispatch(schedulerActions.removeJob(event.payload as unknown as string)));