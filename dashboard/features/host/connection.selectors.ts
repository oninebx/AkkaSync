import { RootState } from "@/store";

export const selectConnectionStatus = (state: RootState) => state.host.connection.status;