import { EnvelopeHandler } from "@/shared/events/types";
import { hostActions } from "./host.slice";
import { HostSnapshot } from "./host.types";


export const hostEventHandlers: Record<string, EnvelopeHandler> = {
  'host.snapshot.updated': (e, dispatch) => {
    dispatch(hostActions.snapshotUpdated(e.payload as HostSnapshot));
  }
}