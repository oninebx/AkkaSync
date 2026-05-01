import { ChangeOperation } from "@/infrastructure/realtime/types";
import { IBusinessState } from "@/types";
import { Comparer, createEntityAdapter, createSlice, EntityId, IdSelector, PayloadAction } from "@reduxjs/toolkit";

interface SliceOptions<T, ID extends EntityId> {
  name: string;
  selectId: IdSelector<T, ID>;
  sortComparer?: Comparer<T>;
  additionalInitialState?: Record<string, any>;
}

function createEntitySlice<T, ID extends EntityId = string>({
  name,
  selectId,
  sortComparer,
  additionalInitialState = {}
}: SliceOptions<T, ID>){

  const adapter = createEntityAdapter<T, ID>({
    selectId,
    sortComparer
  });

  const initialState: IBusinessState<T, ID> = adapter.getInitialState({
    isInitialized: false,
    error: null
  });

  const slice = createSlice({
    name,
    initialState,
    reducers: {
      applyChanges: (state, action: PayloadAction<{
        operation: ChangeOperation;
        data: T[];
      }>) => {
        const {operation, data} = action.payload;
        switch(operation){
          case ChangeOperation.Upsert:
            adapter.upsertMany(state, data);
            break;
        }
      }
    }
  });

  return {
    slice,
    adapter,
    actions: slice.actions,
    selectors: adapter.getSelectors(state => (state as Record<string, any>)[name])
  }
}

export {
  createEntitySlice
}