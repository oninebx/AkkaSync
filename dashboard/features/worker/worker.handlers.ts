import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { setWorkers } from "./worker.slice";
import { WorkerRecord } from "./worker.types";

envelopeHandlerMap.set('worker.state.initialized', (event, dispatch) => {
  console.log(event.payload);
  dispatch(setWorkers(event.payload as Record<string, WorkerRecord>));
}
  );

envelopeHandlerMap.set('worker.record.added', (event, dispatch) => {});
envelopeHandlerMap.set('worker.record.updated', (event, dispatch) => {});
envelopeHandlerMap.set('worker.record.removed', (event, dispatch) => {});