import { RootState } from "@/store";

const selectErrors = (state: RootState) => state.diagnosis.journal.records.filter(record => record.level === 'Error');
const selectJournal = (state: RootState) => state.diagnosis.journal.records;
export {
  selectErrors,
  selectJournal
}