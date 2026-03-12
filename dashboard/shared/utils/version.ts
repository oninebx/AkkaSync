export function compareVersion(a: string, b: string): number {
  const pa = a.split('.').map(Number)
  const pb = b.split('.').map(Number)

  const len = Math.max(pa.length, pb.length)

  for (let i = 0; i < len; i++) {
    const diff = (pa[i] || 0) - (pb[i] || 0)
    if (diff !== 0) return diff
  }

  return 0
}

export function hasNewVersion(
  installed?: string,
  latest?: string
) {
  if (!installed || !latest) return false
  return compareVersion(latest, installed) > 0
}