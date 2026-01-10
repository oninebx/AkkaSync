import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ErrorEntry, ErrorJournal } from "./diagnosis.types";

interface DiagnosisState {
  errorJournal: ErrorJournal
}

const initialState: DiagnosisState = {
  errorJournal: { errors: [] }
};

const diagnosisSlice = createSlice({
  name: 'diagnosis',
  initialState,
  reducers: {
    setErrorJournal(state, action: PayloadAction<ErrorJournal>) {
      state.errorJournal = action.payload;
    },
    addError(state, action: PayloadAction<ErrorEntry>) {
      state.errorJournal.errors.push(action.payload);
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
