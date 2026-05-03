import { useContext } from "react";
import { SignalRContext } from "./SignalRProvider";

export const useSignalR = () => {
  const context = useContext(SignalRContext);

  if (!context) {
    throw new Error('useSignalR can only be used inside a SignalRProvider.');
  }

  return context;
};