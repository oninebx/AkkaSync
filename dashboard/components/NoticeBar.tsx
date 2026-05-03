import React, { useEffect, useRef } from 'react';

export type NoticeType = 'info' | 'warning' | 'error';

interface NoticeBarProps {
  isVisible: boolean;
  type: NoticeType;
  message: string;
  actionText?: string;
  onActionClick?: () => void;
  onClose: () => void;
}

const NoticeBar: React.FC<NoticeBarProps> = ({
  isVisible,
  type,
  message,
  actionText,
  onActionClick,
  onClose,
}) => {
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  const CONFIG = {
    info: {
      barStyle: 'bg-blue-600 text-white',
      btnStyle: 'bg-white text-blue-600 hover:bg-blue-50',
      duration: 3000,
      icon: (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-5 h-5">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      ),
    },
    warning: {
      barStyle: 'bg-amber-500 text-white',
      btnStyle: 'bg-white text-amber-600 hover:bg-amber-50',
      duration: 6000,
      icon: (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-5 h-5">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
        </svg>
      ),
    },
    error: {
      barStyle: 'bg-red-600 text-white',
      btnStyle: 'bg-white text-red-600 hover:bg-red-50',
      duration: null,
      icon: (
        <svg fill="none" stroke="currentColor" viewBox="0 0 24 24" className="w-5 h-5">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      ),
    },
  };

  useEffect(() => {
    if (timerRef.current) clearTimeout(timerRef.current);
    if (isVisible && CONFIG[type].duration) {
      timerRef.current = setTimeout(onClose, CONFIG[type].duration!);
    }
    return () => { if (timerRef.current) clearTimeout(timerRef.current); };
  }, [isVisible, type, onClose]);

  const current = CONFIG[type];

  return (
    <div
      className={`fixed top-0 left-0 right-0 z-[10000] transform transition-all duration-500 ease-in-out shadow-lg ${
        isVisible ? 'translate-y-0' : '-translate-y-full'
      } ${current.barStyle}`}
    >
      <div className="max-w-screen-2xl mx-auto px-6 py-3 flex items-center justify-between min-h-[56px]">
        <div className="flex items-center gap-3 pr-4 overflow-hidden">
          <span className="flex-shrink-0">{current.icon}</span>
          <span className="text-sm font-bold truncate">{message}</span>
        </div>

        <div className="flex items-center gap-4 flex-shrink-0">
          {/* 统一风格的按钮：只有当传入了点击事件和文本时才显示 */}
          {onActionClick && actionText && (
            <button
              onClick={onActionClick}
              className={`px-4 py-1.5 rounded text-xs font-black uppercase tracking-wider transition-all active:scale-95 shadow-sm ${current.btnStyle}`}
            >
              {actionText}
            </button>
          )}
          
          <button onClick={onClose} className="p-1.5 hover:bg-black/10 rounded-full transition-colors">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
};

export default NoticeBar;