import { MiddlewareAPI } from "@reduxjs/toolkit";
import { ChangeSet, PatchReceivedPayload, SignalRStatus, SignalRStatusPayload } from "../types";

const handlePatchMessage = (store: MiddlewareAPI, envelope: { sequence: number, payload: ChangeSet[] }) => {
  const { sequence, payload } = envelope;

  payload.forEach((change) => {
    console.log(`dispatching action to ${change.slice}`);
    store.dispatch({
      type: `${change.slice}/applyChanges`,
      payload: {
        operation: change.operation,
        data: change.payload,
        // sequence: sequence
      }
    });
  });
};

const handleStatusChange = (store: MiddlewareAPI, payload: SignalRStatusPayload) => {
  const { status, error } = payload;

  switch (status) {
    case SignalRStatus.Connected:
      console.log('%c[SignalR] 🟢 is connected', 'color: green; font-weight: bold');
      // 副作用：例如连接成功后清除可能存在的“连接超时”弹窗
      break;

    case SignalRStatus.Reconnecting:
      console.warn(`[SignalR] 🟡 is reconnecting... ${error || ''}`);
      break;

    case SignalRStatus.Disconnected:
      console.error(`[SignalR] 🔴 is disconnected: ${error || '未知原因'}`);
      // 副作用：例如派发一个全局通知
      // store.dispatch(addNotification({ type: 'error', message: '实时数据连接已断开' }));
      break;

    case SignalRStatus.Unavailable:
      console.error(`[SignalR] ❌ is unavailable: ${error}`);
      break;
  }
};

export {
  handlePatchMessage,
  handleStatusChange
}