import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { hostActions } from "./host.slice";
import { HostSnapshot } from "./host.types";

envelopeHandlerMap.set('host.snapshot.updated', 
  (envelope, dispatch) => dispatch(hostActions.snapshotUpdated(envelope as HostSnapshot))
)