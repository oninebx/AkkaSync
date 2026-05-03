import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { SignalRStatus } from "../types";
import NoticeBar from "@/components/NoticeBar";
import { useSignalR } from "./useSignalR";
import { connectionSelectors } from "../store";

const SignalRStatusContainer = () => {
  const {status, error} = useSelector(connectionSelectors.selectConnectionState);
  const { reconnect } = useSignalR();
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    setIsVisible(true);
  }, [status]);

 const getNoticeProps = () => {
  switch (status) {
    case SignalRStatus.Connected:
      return {
        type: 'info' as const,
        message: 'Real-time service connected successfully',
      };
    case SignalRStatus.Disconnected:
      return {
        type: 'error' as const,
        message: `Connection lost: ${error || 'Unknown error'}`,
        actionText: 'Reconnect', // Container determines the text
        onActionClick: reconnect, // Container determines the logic
      };
    case SignalRStatus.Reconnecting:
      return {
        type: 'warning' as const,
        message: 'Network instability detected. Attempting to recover...',
      };
    default:
      return {
        type: 'info' as const,
        message: 'Initializing communication service...',
      };
  }
};

  return (
    <NoticeBar
      {...getNoticeProps()}
      isVisible={isVisible}
      onClose={() => setIsVisible(false)}
    />
  );
};

export { 
  SignalRStatusContainer
}