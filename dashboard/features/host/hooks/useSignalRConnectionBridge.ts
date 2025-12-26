'use client';
import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { hostActions } from "../host.slice";
import { useHostSignalR } from "@/providers/SignalRProvider";

export const useSignalRConnectionBridge = () => {
  const { status } = useHostSignalR();
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(hostActions.connectionStatusChanged(status))
  }, [status, dispatch])
}