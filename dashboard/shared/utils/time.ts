function calcDuration(startedAt: string | Date, finishedAt?: string | Date): number | null {
  if (!startedAt) return null;

  const start = new Date(startedAt).getTime();
  const end = finishedAt ? new Date(finishedAt).getTime() : Date.now();

  return end - start;
}

function formatDuration(startedAt: string | Date, finishedAt?: string | Date): string | null {
  const durationMs = calcDuration(startedAt, finishedAt);
  if (durationMs === null) return null;

  const totalSeconds = Math.floor(durationMs / 1000);
  const hours = Math.floor(totalSeconds / 3600);
  const minutes = Math.floor((totalSeconds % 3600) / 60);
  const seconds = totalSeconds % 60;

  return `${hours.toString().padStart(2, '0')}:` +
         `${minutes.toString().padStart(2, '0')}:` +
         `${seconds.toString().padStart(2, '0')}`;
}

function timeAgo(isoString: string | Date): string {
  const now = Date.now();
  const dt = new Date(isoString).getTime();
  const diffSeconds = Math.floor((now - dt) / 1000);

  if (diffSeconds < 60) return `${diffSeconds}s ago`;
  const diffMinutes = Math.floor(diffSeconds / 60);
  if (diffMinutes < 60) return `${diffMinutes}m ago`;
  const diffHours = Math.floor(diffMinutes / 60);
  if (diffHours < 24) return `${diffHours}h ago`;
  const diffDays = Math.floor(diffHours / 24);
  return `${diffDays}d ago`;
}
function formatDateTime(isoString: string | Date): string {
  const date = new Date(isoString);
  const pad = (n: number) => n.toString().padStart(2, "0");

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ` +
         `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
}

function formatTimeMixed(
  isoString: string | Date,
  thresholdMinutes = 60
): string {
  const now = Date.now();
  const dt = new Date(isoString).getTime();
  const diffMinutes = (now - dt) / 1000 / 60;

  if (diffMinutes < thresholdMinutes) {
    return timeAgo(isoString);
  } else {
    return formatDateTime(isoString);
  }
}

function dayOfWeekName(dow: string): string {
  const map = ['Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday'];
  if (dow.includes(',')) {
    return dow.split(',').map(n => map[+n]).join(', ');
  }
  return map[+dow] || dow;
}

function cronToText(cronExpr: string): string {
  try {
    const fields = cronExpr.trim().split(/\s+/);
    if (fields.length < 5) {
      return 'Invalid cron';
    }
    const [min, hour, dayOfMonth, month, dayOfWeek] = fields;
    if (min === '*' && hour === '*' && dayOfMonth === '*' && month === '*' && dayOfWeek === '*') {
      return 'Every minute';
    }
    const everyNMin = min.match(/\*\/(\d+)/);
    if (everyNMin && hour === '*' && dayOfMonth === '*' && month === '*' && dayOfWeek === '*') {
      return `Every ${everyNMin[1]} minutes`;
    }
    if (hour === '*' && dayOfMonth === '*' && month === '*' && dayOfWeek === '*' && min !== '*') {
      return `Every hour at minute ${min}`;
    }
    if (dayOfMonth === '*' && month === '*' && dayOfWeek === '*' && hour !== '*' && min !== '*') {
      return `Every day at ${hour.padStart(2,'0')}:${min.padStart(2,'0')}`;
    }
    if (dayOfWeek !== '*' && hour !== '*' && min !== '*') {
      return `Every ${dayOfWeekName(dayOfWeek)} at ${hour.padStart(2,'0')}:${min.padStart(2,'0')}`;
    }

    
    return 'Custom schedule';
  } catch(e){
    console.error(e);
    return 'Invalid cron';
  }
}

export {
  formatDuration,
  timeAgo,
  formatDateTime,
  formatTimeMixed,
  cronToText
}