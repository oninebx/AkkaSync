import { envelopeHandlerMap } from "@/shared/events/envelopeHandlerMap";
import { diagnosisActions } from "./diagnosis.slice";
import { DiagnosisJournal, DiagnosisRecord } from "./diagnosis.types";

envelopeHandlerMap.set('diagnosis.records.initialized', (event, dispatch) => 
  dispatch(diagnosisActions.setJournal(event.payload as DiagnosisJournal)));
envelopeHandlerMap.set('diagnosis.records.added', (event, dispatch) => 
  dispatch(diagnosisActions.addRecord(event.payload as unknown as DiagnosisRecord)));