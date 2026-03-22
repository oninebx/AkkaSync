import { Node } from "@xyflow/react";
import { useCallback } from "react";
import { NodeLayout } from "../utils/liveLayout";

const STORAGE_KEY = 'live-layout'

const useLiveLayout = () => {
  const saveLayout = useCallback((nodes: Node[]) => {
    const layout: NodeLayout = {};
    nodes.forEach(n => { layout[n.id] = n.position });
    localStorage.setItem(STORAGE_KEY, JSON.stringify(layout));
  }, []);

  const loadLayout = useCallback((): NodeLayout | null => {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : null
  }, []);
  
  return { saveLayout, loadLayout }
}

export default useLiveLayout;