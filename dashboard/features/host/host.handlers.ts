import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { hostActions } from "./host.slice";
import { HostSnapshot, PipelineSnapshot } from "./host.types";

envelopeHandlerMap.set('syncing.state.initialized', 
  (event, dispatch) => dispatch(hostActions.snapshotInitialized(event.payload as HostSnapshot))
);

envelopeHandlerMap.set('syncing.pipeline.started', 
  (event, dispatch) => dispatch(hostActions.pipelineStarted(event.payload as PipelineSnapshot))
);

envelopeHandlerMap.set('syncing.pipeline.completed',
  (event, dispatch) => dispatch(hostActions.pipelineCompleted(event.payload as PipelineSnapshot))
);
