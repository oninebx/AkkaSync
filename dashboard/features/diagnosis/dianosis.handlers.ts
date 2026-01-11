import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { diagnosisActions } from "./diagnosis.slice";
import { DiagnosisJournal, DiagnosisRecord } from "./diagnosis.types";

envelopeHandlerMap.set('diagnosis.initialized', (event, dispatch) => 
  dispatch(diagnosisActions.setErrorJournal(event.payload as DiagnosisJournal)));
envelopeHandlerMap.set('diagnosis.errors.added', (event, dispatch) => 
  dispatch(diagnosisActions.addError(event.payload as unknown as DiagnosisRecord)));