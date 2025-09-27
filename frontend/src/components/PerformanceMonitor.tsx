import React, { useEffect, useState } from 'react';
import {
  Card,
  CardContent,
  Typography,
  Box,
  LinearProgress,
  Chip,
  Grid,
  IconButton,
  Tooltip
} from '@mui/material';
import {
  Speed,
  Memory,
  NetworkCheck,
  Refresh,
  TrendingUp,
  TrendingDown
} from '@mui/icons-material';

interface PerformanceMetrics {
  loadTime: number;
  renderTime: number;
  memoryUsage: number;
  networkRequests: number;
  fps: number;
  cpuUsage: number;
}

const PerformanceMonitor: React.FC = () => {
  const [metrics, setMetrics] = useState<PerformanceMetrics>({
    loadTime: 0,
    renderTime: 0,
    memoryUsage: 0,
    networkRequests: 0,
    fps: 60,
    cpuUsage: 0
  });

  const [isMonitoring, setIsMonitoring] = useState(true);

  useEffect(() => {
    if (!isMonitoring) return;

    const updateMetrics = () => {
      // Get performance metrics from browser APIs
      const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming;
      const memory = (performance as any).memory;
      
      setMetrics(prev => ({
        ...prev,
        loadTime: navigation ? navigation.loadEventEnd - navigation.loadEventStart : 0,
        renderTime: navigation ? navigation.domContentLoadedEventEnd - navigation.domContentLoadedEventStart : 0,
        memoryUsage: memory ? Math.round(memory.usedJSHeapSize / 1024 / 1024) : 0,
        networkRequests: performance.getEntriesByType('resource').length,
        fps: Math.round(1000 / (performance.now() - (prev as any).lastFrameTime || 16.67)),
        cpuUsage: Math.random() * 100 // Simulated CPU usage
      }));
    };

    const interval = setInterval(updateMetrics, 1000);
    return () => clearInterval(interval);
  }, [isMonitoring]);

  const getPerformanceColor = (value: number, thresholds: { good: number; warning: number }) => {
    if (value <= thresholds.good) return 'success';
    if (value <= thresholds.warning) return 'warning';
    return 'error';
  };

  const formatTime = (ms: number) => `${ms.toFixed(2)}ms`;
  const formatMemory = (mb: number) => `${mb.toFixed(1)}MB`;

  return (
    <Card sx={{ height: '100%' }}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            🚀 Performance Monitor
          </Typography>
          <Tooltip title={isMonitoring ? "Stop monitoring" : "Start monitoring"}>
            <IconButton 
              size="small" 
              onClick={() => setIsMonitoring(!isMonitoring)}
              color={isMonitoring ? "error" : "primary"}
            >
              <Refresh />
            </IconButton>
          </Tooltip>
        </Box>

        <Grid container spacing={2}>
          <Grid item xs={6}>
            <Box mb={2}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <Typography variant="body2" color="text.secondary">
                  Load Time
                </Typography>
                <Chip 
                  size="small" 
                  color={getPerformanceColor(metrics.loadTime, { good: 1000, warning: 3000 })}
                  label={formatTime(metrics.loadTime)}
                />
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={Math.min((metrics.loadTime / 5000) * 100, 100)}
                color={getPerformanceColor(metrics.loadTime, { good: 1000, warning: 3000 })}
              />
            </Box>

            <Box mb={2}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <Typography variant="body2" color="text.secondary">
                  Memory Usage
                </Typography>
                <Chip 
                  size="small" 
                  color={getPerformanceColor(metrics.memoryUsage, { good: 50, warning: 100 })}
                  label={formatMemory(metrics.memoryUsage)}
                />
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={Math.min((metrics.memoryUsage / 200) * 100, 100)}
                color={getPerformanceColor(metrics.memoryUsage, { good: 50, warning: 100 })}
              />
            </Box>
          </Grid>

          <Grid item xs={6}>
            <Box mb={2}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <Typography variant="body2" color="text.secondary">
                  FPS
                </Typography>
                <Chip 
                  size="small" 
                  color={getPerformanceColor(60 - metrics.fps, { good: 10, warning: 20 })}
                  label={`${metrics.fps} FPS`}
                />
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={(metrics.fps / 60) * 100}
                color={getPerformanceColor(60 - metrics.fps, { good: 10, warning: 20 })}
              />
            </Box>

            <Box mb={2}>
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                <Typography variant="body2" color="text.secondary">
                  Network Requests
                </Typography>
                <Chip 
                  size="small" 
                  color={getPerformanceColor(metrics.networkRequests, { good: 20, warning: 50 })}
                  label={metrics.networkRequests}
                />
              </Box>
              <LinearProgress 
                variant="determinate" 
                value={Math.min((metrics.networkRequests / 100) * 100, 100)}
                color={getPerformanceColor(metrics.networkRequests, { good: 20, warning: 50 })}
              />
            </Box>
          </Grid>
        </Grid>

        <Box mt={2} p={2} bgcolor="grey.50" borderRadius={1}>
          <Typography variant="body2" color="text.secondary" align="center">
            {isMonitoring ? (
              <>
                <Speed sx={{ fontSize: 16, mr: 1, verticalAlign: 'middle' }} />
                Real-time monitoring active
              </>
            ) : (
              <>
                <NetworkCheck sx={{ fontSize: 16, mr: 1, verticalAlign: 'middle' }} />
                Monitoring paused
              </>
            )}
          </Typography>
        </Box>
      </CardContent>
    </Card>
  );
};

export default PerformanceMonitor;
