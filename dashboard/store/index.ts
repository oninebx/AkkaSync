import { configureStore } from "@reduxjs/toolkit";
import { rootReducer } from "./rootReducer";
import { signalRMiddleware } from "@/middleware/signalRMiddleware";

export const store = configureStore({
  reducer: rootReducer,
  middleware: getDefaultMiddleware => getDefaultMiddleware().prepend(signalRMiddleware)
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;