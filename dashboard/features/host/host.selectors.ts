import { RootState } from "@/store";

export const selectHostStatus = (state: RootState) => state.host.status;
export const selectHostPipelines = (state: RootState) => state.host.pipelines;