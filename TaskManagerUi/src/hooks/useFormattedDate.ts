import { useMemo } from 'react';

export function useFormattedDate(dateString: string): string {
  return useMemo(() => {
    const eventDate = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - eventDate.getTime();
    const diffSeconds = Math.floor(diffMs / 1000);
    const diffMinutes = Math.floor(diffSeconds / 60);
    const diffHours = Math.floor(diffMinutes / 60);

    const rtf = new Intl.RelativeTimeFormat(undefined, { numeric: 'auto' });

    if (diffHours >= 24) {
      return eventDate.toLocaleString(undefined, {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      });
    } else if (diffHours >= 1) {
      return rtf.format(-diffHours, 'hour');
    } else if (diffMinutes >= 1) {
      return rtf.format(-diffMinutes, 'minute');
    } else {
      return rtf.format(-diffSeconds, 'second');
    }
  }, [dateString]);
}