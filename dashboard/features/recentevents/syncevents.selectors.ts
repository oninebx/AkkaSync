import { RootState } from "@/store";
import { createSelector } from "@reduxjs/toolkit";

export const selectEventsOrdered = createSelector(
  (state: RootState) => state.events.envelopes,
  envelopes => Object.values(envelopes).sort(
    (a, b) => a.sequence - b.sequence
  ));