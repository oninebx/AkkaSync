import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { setDefinitions, upsertRun } from "./pipeline.slice";
import { PipelineDefinition, PipelineRun } from "./pipeline.types";


envelopeHandlerMap.set('pipeline.specs.initialized', (event, dispatch) => 
  dispatch(setDefinitions(event.payload as PipelineDefinition[])));

envelopeHandlerMap.set('pipeline.run.created', (event, dispatch) => 
  dispatch(upsertRun(event.payload as PipelineRun)));

// envelopeHandlerMap.set('pipeline.run.started', (event, dispatch) => 
//   dispatch(pipelineUpdated(event.payload as Pipeline)));

// envelopeHandlerMap.set('pipeline.run.completed', (event, dispatch) => 
//   dispatch(pipelineUpdated(event.payload as Pipeline)));