import { ChangeOperation } from "@/infrastructure/realtime/types";
import { IBusinessState } from "@/types";
import { Comparer, createEntityAdapter, createSlice, EntityId, IdSelector, PayloadAction } from "@reduxjs/toolkit"; 

type EntityChangeHandler<T, A> = (
  state: A, 
  data: T[], 
  operation: ChangeOperation
) => void;

interface SliceOptions<T, ID extends EntityId, A> {
  name: string;
  selectId: IdSelector<T, ID>;
  sortComparer?: Comparer<T>;
  additionalInitialState?: A;
  onChanges?: EntityChangeHandler<T, A>;
}

function createEntitySlice<T, ID extends EntityId = string, A = Record<string, any>>({
  name,
  selectId,
  sortComparer,
  additionalInitialState = {} as A,
  onChanges
}: SliceOptions<T, ID, A>){

  const adapter = createEntityAdapter<T, ID>({
    selectId,
    sortComparer
  });

  const initialState: IBusinessState<T, ID> = adapter.getInitialState({
    isInitialized: false,
    error: null,
    ...additionalInitialState
  }) as IBusinessState<T, ID> & A;

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
          case ChangeOperation.Replace:
            adapter.setAll(state, data);
            break;
          // case ChangeOperation.Remove:
          //   console.log(data);
          //   const idsToRemove = data.map(selectId);
          //   console.log(`Removing entities with IDs: ${idsToRemove.join(', ')}`);
          //   adapter.removeMany(state, idsToRemove);
          //   break;
        }
        if(onChanges) {
          onChanges(state as A, data, operation);
        }
      }
    }
  });

  const selectSliceState = (state: any) => state[name] as IBusinessState<T, ID> & A;

  return {
    slice,
    adapter,
    actions: slice.actions,
    selectors: {
      ...adapter.getSelectors(selectSliceState),
      getExtraField: <K extends keyof A>(key: K) => (state: any): A[K] => selectSliceState(state)[key]
      // getExtraField: <K extends keyof A>(state: any, key: K): A[K] => selectSliceState(state)[key] 
    }
  }
}

export {
  createEntitySlice
}

export type {
  EntityChangeHandler
}
