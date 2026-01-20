import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { DiagnosisJournal, DiagnosisRecord } from "./diagnosis.types";

interface DiagnosisState {
  journal: DiagnosisJournal
}

const initialState: DiagnosisState = {
  journal: { records: [] }
};

const diagnosisSlice = createSlice({
  name: 'diagnosis',
  initialState,
  reducers: {
    setJournal(state, action: PayloadAction<DiagnosisJournal>) {
      state.journal = action.payload;
    },
    addRecord(state, action: PayloadAction<DiagnosisRecord>) {
      state.journal.records.push(action.payload);
    }
  }
});

const diagnosisActions = diagnosisSlice.actions;
const diagnosisReducer = diagnosisSlice.reducer;

export {
  diagnosisSlice,
  diagnosisActions,
  diagnosisReducer
}
