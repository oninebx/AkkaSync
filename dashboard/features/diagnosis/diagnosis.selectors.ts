import { RootState } from "@/store";

const selectErrors = (state: RootState) => state.diagnosis.errorJournal.errors;

export {
  selectErrors
}