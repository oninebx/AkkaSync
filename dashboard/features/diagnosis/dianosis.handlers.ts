import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { diagnosisActions } from "./diagnosis.slice";
import { ErrorEntry, ErrorJournal } from "./diagnosis.types";

envelopeHandlerMap.set('diagnosis.errors.initialized', (event, dispatch) => 
  dispatch(diagnosisActions.setErrorJournal(event.payload as ErrorJournal)));
envelopeHandlerMap.set('diagnosis.errors.added', (event, dispatch) => 
  dispatch(diagnosisActions.addError(event.payload as unknown as ErrorEntry)));