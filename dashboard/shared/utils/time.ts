import parser from 'cron-parser';

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

function formatRelativeTime(date: string | Date): string {
  const now = Date.now();
  const target = new Date(date).getTime();
  const diffSeconds = Math.floor((target - now) / 1000);
  const abs = Math.abs(diffSeconds);

  const isFuture = diffSeconds > 0;

  if (abs < 60) {
    return isFuture ? `in ${abs}s` : `${abs}s ago`;
  }

  const minutes = Math.floor(abs / 60);
  if (minutes < 60) {
    return isFuture ? `in ${minutes}m` : `${minutes}m ago`;
  }

  const hours = Math.floor(minutes / 60);
  if (hours < 24) {
    return isFuture ? `in ${hours}h` : `${hours}h ago`;
  }

  const days = Math.floor(hours / 24);
  return isFuture ? `in ${days}d` : `${days}d ago`;
}

function formatDateTime(isoString: string | Date): string {
  const date = new Date(isoString);
  const pad = (n: number) => n.toString().padStart(2, "0");

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())} ` +
         `${pad(date.getHours())}:${pad(date.getMinutes())}:${pad(date.getSeconds())}`;
}

function formatTimeMixed(
  date: string | Date | null | undefined,
  thresholdMinutes = 60
): string {
  if (!date) return '—';

  const now = Date.now();
  const target = new Date(date).getTime();
  const diffMinutes = Math.abs(target - now) / 1000 / 60;

  if (diffMinutes <= thresholdMinutes) {
    return formatRelativeTime(date);
  }

  return formatDateTime(date);
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

function cronToNext(cronExpr: string): string | undefined {
  try {
    const interval = parser.parse(cronExpr);
    const next = interval.next().toDate(); // 得到原生 Date 对象
    // 格式化为 YYYY-MM-DD HH:mm:ss
    const pad = (n: number) => n.toString().padStart(2, '0');
    return `${next.getFullYear()}-${pad(next.getMonth() + 1)}-${pad(next.getDate())} ` +
           `${pad(next.getHours())}:${pad(next.getMinutes())}:${pad(next.getSeconds())}`;
  } catch {
    return undefined;
  }
}

export {
  formatDuration,
  formatRelativeTime,
  formatDateTime,
  formatTimeMixed,
  cronToText,
  cronToNext
}