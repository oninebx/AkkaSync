import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { pipelineUpdated, setPipelines } from "./pipeline.slice";
import { Pipeline } from "./pipeline.types";

envelopeHandlerMap.set('pipeline.specs.initialized', (event, dispatch) => 
  dispatch(setPipelines(event.payload as Pipeline[])));

envelopeHandlerMap.set('pipeline.run.started', (event, dispatch) => 
  dispatch(pipelineUpdated(event.payload as Pipeline)));

envelopeHandlerMap.set('pipeline.run.completed', (event, dispatch) => 
  dispatch(pipelineUpdated(event.payload as Pipeline)));