import { EntityState } from "@reduxjs/toolkit";

interface IBusinessState<T, ID extends string | number = string> extends EntityState<T, ID> {
  isInitialized: boolean;
  error: string | null;
}

export type {
  IBusinessState
}