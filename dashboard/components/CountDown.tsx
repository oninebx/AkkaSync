// components/ui/Countdown.tsx
import { useState, useEffect, useCallback } from 'react';

interface CountdownProps {
  /** 目标截止时间 */
  target: string | number | Date;
  /** 倒计时结束后的占位文字，默认 "Expired" */
  expiredLabel?: string;
  /** 是否显示天数，默认自动判断 */
  showDays?: boolean;
  /** 完成后的回调 */
  onEnd?: () => void;
  /** 自定义渲染逻辑 */
  children?: (time: { d: number, h: number, m: number, s: number, isEnd: boolean }) => React.ReactNode;
  /** 容器样式 */
  className?: string;
}

export const Countdown = ({
  target,
  expiredLabel = '00:00:00',
  showDays = true,
  onEnd,
  children,
  className
}: CountdownProps) => {
  const [timeLeft, setTimeLeft] = useState({ d: 0, h: 0, m: 0, s: 0, isEnd: false });

  const calculate = useCallback(() => {
    const diff = new Date(target).getTime() - Date.now();
    
    if (diff <= 0) {
      return { d: 0, h: 0, m: 0, s: 0, isEnd: true };
    }

    return {
      d: Math.floor(diff / 86400000),
      h: Math.floor((diff % 86400000) / 3600000),
      m: Math.floor((diff % 3600000) / 60000),
      s: Math.floor((diff % 60000) / 1000),
      isEnd: false
    };
  }, [target]);

  useEffect(() => {
    const tick = () => {
      const result = calculate();
      setTimeLeft(result);
      if (result.isEnd) {
        onEnd?.();
      }
    };

    tick();
    const timer = setInterval(tick, 1000);
    return () => clearInterval(timer);
  }, [calculate, onEnd]);

  // 如果有自定义渲染
  if (children) return <div className={className}>{children(timeLeft)}</div>;

  // 默认渲染逻辑
  if (timeLeft.isEnd) return <span className={className}>{expiredLabel}</span>;

  const { d, h, m, s } = timeLeft;
  const pad = (n: number) => n.toString().padStart(2, '0');

  return (
    <span className={`${className} tabular-nums`}>
      {showDays && d > 0 && `${d}d `}
      {`${pad(h)}:${pad(m)}:${pad(s)}`}
    </span>
  );
};