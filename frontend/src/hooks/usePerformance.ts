import { useEffect, useState } from 'react';

export const usePerformance = () => {
  const [metrics, setMetrics] = useState({
    loadTime: 0,
    renderTime: 0,
    memoryUsage: 0,
    networkRequests: 0
  });

  useEffect(() => {
    const startTime = performance.now();
    
    // Monitor performance
    const observer = new PerformanceObserver((list) => {
      const entries = list.getEntries();
      const navigationEntry = entries.find(entry => entry.entryType === 'navigation') as PerformanceNavigationTiming;
      
      if (navigationEntry) {
        setMetrics(prev => ({
          ...prev,
          loadTime: navigationEntry.loadEventEnd - navigationEntry.loadEventStart,
          renderTime: navigationEntry.domContentLoadedEventEnd - navigationEntry.domContentLoadedEventStart
        }));
      }
    });

    observer.observe({ entryTypes: ['navigation', 'measure'] });

    // Monitor memory usage
    const checkMemory = () => {
      if ('memory' in performance) {
        const memory = (performance as any).memory;
        setMetrics(prev => ({
          ...prev,
          memoryUsage: memory.usedJSHeapSize / 1024 / 1024 // Convert to MB
        }));
      }
    };

    const interval = setInterval(checkMemory, 5000);

    return () => {
      observer.disconnect();
      clearInterval(interval);
    };
  }, []);

  return metrics;
};

export const useLazyLoading = (threshold = 0.1) => {
  const [isVisible, setIsVisible] = useState(false);
  const [ref, setRef] = useState<HTMLElement | null>(null);

  useEffect(() => {
    if (!ref) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setIsVisible(true);
          observer.disconnect();
        }
      },
      { threshold }
    );

    observer.observe(ref);

    return () => observer.disconnect();
  }, [ref, threshold]);

  return [setRef, isVisible] as const;
};

export const useDebounce = <T>(value: T, delay: number): T => {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
};

export const useThrottle = <T extends (...args: any[]) => any>(
  func: T,
  delay: number
): T => {
  const [lastCall, setLastCall] = useState(0);

  return ((...args: Parameters<T>) => {
    const now = Date.now();
    if (now - lastCall >= delay) {
      setLastCall(now);
      return func(...args);
    }
  }) as T;
};
