import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { connectionActions } from "./connection.slice";


envelopeHandlerMap.set('connection.status.changed',
  (envelope, dispatch) => dispatch(connectionActions.connectionStatusChanged(envelope.payload)));