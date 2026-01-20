import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { connectionActions } from "./connection.slice";
import { ConnectionStatusEvent } from "./signalr.types";


envelopeHandlerMap.set('connection.status.changed',
  (event, dispatch) => dispatch(connectionActions.connectionStatusChanged(event as unknown as ConnectionStatusEvent)));