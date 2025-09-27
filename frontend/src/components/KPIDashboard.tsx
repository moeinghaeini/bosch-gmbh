import React, { useState, useEffect } from 'react';
import {
  Grid,
  Card,
  CardContent,
  Typography,
  Box,
  LinearProgress,
  Chip,
  Avatar,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Divider,
  Paper,
  Stack,
  IconButton,
  Tooltip,
  Fade,
  Zoom
} from '@mui/material';
import {
  TrendingUp,
  TrendingDown,
  CheckCircle,
  Error,
  Schedule,
  Speed,
  Memory,
  NetworkCheck,
  Refresh,
  Analytics,
  Assessment,
  Timeline,
  Dashboard as DashboardIcon
} from '@mui/icons-material';

interface KPIData {
  testExecution: {
    totalTests: number;
    passedTests: number;
    failedTests: number;
    successRate: number;
    averageExecutionTime: number;
  };
  webAutomation: {
    totalAutomations: number;
    completedAutomations: number;
    failedAutomations: number;
    successRate: number;
    averageExecutionTime: number;
  };
  jobScheduling: {
    totalJobs: number;
    enabledJobs: number;
    completedJobs: number;
    successRate: number;
  };
  overallPerformance: {
    systemUptime: number;
    totalTasks: number;
    successfulTasks: number;
    averageResponseTime: number;
    errorRate: number;
    resourceUtilization: Record<string, number>;
  };
}

const KPIDashboard: React.FC = () => {
  const [kpiData, setKpiData] = useState<KPIData | null>(null);
  const [loading, setLoading] = useState(true);
  const [lastUpdated, setLastUpdated] = useState<Date>(new Date());

  useEffect(() => {
    fetchKPIData();
    const interval = setInterval(fetchKPIData, 30000); // Update every 30 seconds
    return () => clearInterval(interval);
  }, []);

  const fetchKPIData = async () => {
    try {
      // Mock data for demonstration
      const mockData: KPIData = {
        testExecution: {
          totalTests: 1247,
          passedTests: 1156,
          failedTests: 91,
          successRate: 92.7,
          averageExecutionTime: 2.3
        },
        webAutomation: {
          totalAutomations: 342,
          completedAutomations: 318,
          failedAutomations: 24,
          successRate: 92.9,
          averageExecutionTime: 1.8
        },
        jobScheduling: {
          totalJobs: 89,
          enabledJobs: 76,
          completedJobs: 67,
          successRate: 88.2
        },
        overallPerformance: {
          systemUptime: 99.8,
          totalTasks: 1678,
          successfulTasks: 1541,
          averageResponseTime: 145,
          errorRate: 2.1,
          resourceUtilization: {
            CPU: 68.5,
            Memory: 72.3,
            Disk: 45.8,
            Network: 38.2
          }
        }
      };

      setKpiData(mockData);
      setLastUpdated(new Date());
    } catch (error) {
      console.error('Error fetching KPI data:', error);
    } finally {
      setLoading(false);
    }
  };

  const KPICard = ({ title, value, icon, color, trend, trendValue, description }: any) => (
    <Zoom in={true} style={{ transitionDelay: '100ms' }}>
      <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
        <CardContent>
          <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={2}>
            <Typography variant="h6" sx={{ fontWeight: 600, color: 'text.primary' }}>
              {title}
            </Typography>
            <Avatar sx={{ bgcolor: color, width: 48, height: 48 }}>
              {icon}
            </Avatar>
          </Box>
          
          <Typography variant="h3" sx={{ fontWeight: 700, mb: 1, color: 'text.primary' }}>
            {value}
          </Typography>
          
          {trend && (
            <Box display="flex" alignItems="center" mb={1}>
              {trend === 'up' ? (
                <TrendingUp color="success" sx={{ mr: 0.5, fontSize: 20 }} />
              ) : (
                <TrendingDown color="error" sx={{ mr: 0.5, fontSize: 20 }} />
              )}
              <Typography variant="body2" color={trend === 'up' ? 'success.main' : 'error.main'} sx={{ fontWeight: 600 }}>
                {trendValue}
              </Typography>
              <Typography variant="body2" color="text.secondary" ml={0.5}>
                vs last period
              </Typography>
            </Box>
          )}
          
          <Typography variant="body2" color="text.secondary">
            {description}
          </Typography>
        </CardContent>
      </Card>
    </Zoom>
  );

  const ProgressCard = ({ title, value, maxValue, color, icon }: any) => (
    <Fade in={true} style={{ transitionDelay: '200ms' }}>
      <Card elevation={2} sx={{ borderRadius: 2, height: '100%' }}>
        <CardContent>
          <Box display="flex" alignItems="center" mb={2}>
            <Avatar sx={{ bgcolor: color, mr: 2, width: 40, height: 40 }}>
              {icon}
            </Avatar>
            <Typography variant="h6" sx={{ fontWeight: 600 }}>
              {title}
            </Typography>
          </Box>
          
          <Box mb={1}>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={0.5}>
              <Typography variant="body2" color="text.secondary">
                {value} / {maxValue}
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ fontWeight: 600 }}>
                {Math.round((value / maxValue) * 100)}%
              </Typography>
            </Box>
            <LinearProgress 
              variant="determinate" 
              value={(value / maxValue) * 100} 
              color={color}
              sx={{ height: 8, borderRadius: 4 }}
            />
          </Box>
        </CardContent>
      </Card>
    </Fade>
  );

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <Typography>Loading KPI data...</Typography>
      </Box>
    );
  }

  if (!kpiData) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <Typography color="error">Failed to load KPI data</Typography>
      </Box>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box mb={4}>
        <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
          <Typography variant="h4" sx={{ fontWeight: 700 }}>
            📊 Performance KPIs
          </Typography>
          <Box display="flex" alignItems="center" gap={2}>
            <Chip 
              icon={<CheckCircle />} 
              label="Live Data" 
              color="success" 
              variant="outlined" 
              sx={{ fontWeight: 600 }} 
            />
            <IconButton onClick={fetchKPIData} color="primary">
              <Refresh />
            </IconButton>
          </Box>
        </Box>
        
        <Typography variant="body2" color="text.secondary">
          Last updated: {lastUpdated.toLocaleString()}
        </Typography>
      </Box>

      {/* Main KPI Cards */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} md={3}>
          <KPICard
            title="Test Success Rate"
            value={`${kpiData.testExecution.successRate}%`}
            icon={<CheckCircle />}
            color="#2e7d32"
            trend="up"
            trendValue="+2.3%"
            description="Quality Control Tests"
          />
        </Grid>
        
        <Grid item xs={12} md={3}>
          <KPICard
            title="Automation Success"
            value={`${kpiData.webAutomation.successRate}%`}
            icon={<Speed />}
            color="#1976d2"
            trend="up"
            trendValue="+1.8%"
            description="System Integration"
          />
        </Grid>
        
        <Grid item xs={12} md={3}>
          <KPICard
            title="Job Success Rate"
            value={`${kpiData.jobScheduling.successRate}%`}
            icon={<Schedule />}
            color="#ed6c02"
            trend="up"
            trendValue="+3.1%"
            description="Maintenance Tasks"
          />
        </Grid>
        
        <Grid item xs={12} md={3}>
          <KPICard
            title="System Uptime"
            value={`${kpiData.overallPerformance.systemUptime}%`}
            icon={<NetworkCheck />}
            color="#9c27b0"
            trend="up"
            trendValue="+0.2%"
            description="Overall Availability"
          />
        </Grid>
      </Grid>

      {/* Performance Metrics */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} md={6}>
          <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                📈 Task Performance Overview
              </Typography>
              
              <Stack spacing={2}>
                <ProgressCard
                  title="Test Executions"
                  value={kpiData.testExecution.passedTests}
                  maxValue={kpiData.testExecution.totalTests}
                  color="success"
                  icon={<CheckCircle />}
                />
                
                <ProgressCard
                  title="Web Automations"
                  value={kpiData.webAutomation.completedAutomations}
                  maxValue={kpiData.webAutomation.totalAutomations}
                  color="primary"
                  icon={<Speed />}
                />
                
                <ProgressCard
                  title="Scheduled Jobs"
                  value={kpiData.jobScheduling.completedJobs}
                  maxValue={kpiData.jobScheduling.totalJobs}
                  color="warning"
                  icon={<Schedule />}
                />
              </Stack>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={6}>
          <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                🖥️ System Resources
              </Typography>
              
              <Stack spacing={2}>
                {Object.entries(kpiData.overallPerformance.resourceUtilization).map(([resource, usage]) => (
                  <Box key={resource}>
                    <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
                      <Typography variant="body2" sx={{ fontWeight: 600, textTransform: 'capitalize' }}>
                        {resource}
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        {usage}%
                      </Typography>
                    </Box>
                    <LinearProgress 
                      variant="determinate" 
                      value={usage} 
                      color={usage > 80 ? 'error' : usage > 60 ? 'warning' : 'success'}
                      sx={{ height: 8, borderRadius: 4 }}
                    />
                  </Box>
                ))}
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Detailed Metrics */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={4}>
          <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                ⚡ Performance Metrics
              </Typography>
              
              <List dense>
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'success.light' }}>
                      <Speed />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Average Response Time" 
                    secondary={`${kpiData.overallPerformance.averageResponseTime}ms`}
                  />
                </ListItem>
                
                <Divider />
                
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'info.light' }}>
                      <Analytics />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Total Tasks" 
                    secondary={kpiData.overallPerformance.totalTasks.toLocaleString()}
                  />
                </ListItem>
                
                <Divider />
                
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'warning.light' }}>
                      <Error />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Error Rate" 
                    secondary={`${kpiData.overallPerformance.errorRate}%`}
                  />
                </ListItem>
              </List>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={4}>
          <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                🎯 Success Metrics
              </Typography>
              
              <List dense>
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'success.light' }}>
                      <CheckCircle />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Successful Tasks" 
                    secondary={kpiData.overallPerformance.successfulTasks.toLocaleString()}
                  />
                </ListItem>
                
                <Divider />
                
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'primary.light' }}>
                      <Timeline />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Test Execution Time" 
                    secondary={`${kpiData.testExecution.averageExecutionTime}min avg`}
                  />
                </ListItem>
                
                <Divider />
                
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'secondary.light' }}>
                      <Speed />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Automation Time" 
                    secondary={`${kpiData.webAutomation.averageExecutionTime}min avg`}
                  />
                </ListItem>
              </List>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={4}>
          <Card elevation={3} sx={{ borderRadius: 3, height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 3 }}>
                📊 System Health
              </Typography>
              
              <Box textAlign="center" mb={3}>
                <Typography variant="h2" sx={{ fontWeight: 700, color: 'success.main' }}>
                  {kpiData.overallPerformance.systemUptime}%
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  System Uptime
                </Typography>
              </Box>
              
              <Stack direction="row" spacing={1} justifyContent="center" flexWrap="wrap" gap={1}>
                <Chip label="Operational" color="success" size="small" />
                <Chip label="Stable" color="info" size="small" />
                <Chip label="Optimized" color="primary" size="small" />
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

export default KPIDashboard;
