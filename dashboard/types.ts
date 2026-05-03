import { EntityState } from "@reduxjs/toolkit";

interface IBusinessState<T, ID extends string | number = string> extends EntityState<T, ID> {
  isInitialized: boolean;
  error: string | null;
}
type AsyncFunction<TArgs extends any[] = any[], TReturn = any> = (...args: TArgs) => Promise<TReturn>;

export type {
  IBusinessState,
  AsyncFunction
}