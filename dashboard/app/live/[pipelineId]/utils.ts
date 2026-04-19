import { PluginDefinition } from "@/features/pipeline/pipeline.types";
import { PluginAggregate, PluginNodeData, PluginNodeEdge } from "./types";

export function buildPluginGraph(
  defs: PluginAggregate[]
): { nodes: PluginNodeData[]; edges: PluginNodeEdge[] } {


  // ----------------------------
  // 2. 构建 nodes（纯定义，不含 layout）
  // ----------------------------
  const nodes: PluginNodeData[] = defs.map((d, i) => ({
    id: d.id,
    type: d.type,
    position: { x: 0, y: 0 }, // layout 后面再做
    data: {
      id: d.id,
      name: d.name,
      type: d.type,
      status: 'idle',
      stats: {
        processed: 0,
        errors: 0,
      }
    }
  }));

  // ----------------------------
  // 3. 构建 edges（DAG 核心）
  // ----------------------------
  const edges: PluginNodeEdge[] = [];

  defs.forEach(def => {
    const targetId = def.id;

    (def.dependsOn ?? []).forEach(dep => {
      const sourceId = defs.find(d => d.id === dep)?.id;
      console.log(defs);
      if (!sourceId) return;
      
      edges.push({
        id: `${sourceId}->${targetId}`,
        source: sourceId,
        target: targetId,
      });
      console.log(edges);
    });
  });

  return { nodes, edges };
}

export function buildLayers(
  nodes: PluginNodeData[],
  edges: PluginNodeEdge[]
): PluginNodeData[][] {

  const inDegree = new Map<string, number>();
  const graph = new Map<string, string[]>();

  nodes.forEach(n => {
    inDegree.set(n.id, 0);
    graph.set(n.id, []);
  });

  edges.forEach(e => {
    graph.get(e.source)!.push(e.target);
    inDegree.set(e.target, (inDegree.get(e.target) || 0) + 1);
  });

  const layers: PluginNodeData[][] = [];

  let current = nodes.filter(n => (inDegree.get(n.id) ?? 0) === 0);

  while (current.length > 0) {

    layers.push(current);

    const next: PluginNodeData[] = [];

    for (const n of current) {
      for (const child of graph.get(n.id) ?? []) {

        const deg = (inDegree.get(child) ?? 0) - 1;
        inDegree.set(child, deg);

        if (deg === 0) {
          const node = nodes.find(x => x.id === child);
          if (node) next.push(node);
        }
      }
    }

    current = next;
  }

  if (layers.flat().length !== nodes.length) {
    throw new Error('Circular dependency detected');
  }

  return layers;
}

export function layoutNodes(layers: PluginNodeData[][]): PluginNodeData[] {

  const layerGap = 200;
  const nodeGap = 180;

  const result: PluginNodeData[] = [];

  layers.forEach((layer, yIndex) => {

    const y = yIndex * layerGap;

    const totalWidth = (layer.length - 1) * nodeGap;
    const startX = -totalWidth / 2;

    layer.forEach((node, xIndex) => {

      result.push({
        ...node,
        position: {
          x: startX + xIndex * nodeGap,
          y
        }
      });
    });
  });

  return result;
}