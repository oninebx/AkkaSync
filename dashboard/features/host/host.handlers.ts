import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { hostActions } from "./host.slice";
import { HostSnapshot } from "./host.types";

envelopeHandlerMap.set('host.snapshot.updated', 
  (event, dispatch) => dispatch(hostActions.snapshotUpdated(event.payload as HostSnapshot))
)