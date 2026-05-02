import { Node } from "@xyflow/react";
import { useCallback } from "react";
import { NodeLayout } from "./liveLayout";

const STORAGE_KEY_PREFIX = 'live-layout:';

const useLiveLayout = (pipelineId: string) => {
  const key = `${STORAGE_KEY_PREFIX}${pipelineId}`;

  const saveLayout = useCallback((nodes: Node[]) => {
    const layout: NodeLayout = {};
    nodes.forEach(n => {
      // Only store position to keep JSON small
      layout[n.id] = n.position;
    });
    localStorage.setItem(key, JSON.stringify(layout));
  }, [key]);

  const loadLayout = useCallback((): NodeLayout => {
    const raw = localStorage.getItem(key);
    return raw ? JSON.parse(raw) : {};
  }, [key]);

  return { saveLayout, loadLayout };
};

export default useLiveLayout;