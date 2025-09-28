import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { 
  Box, 
  AppBar, 
  Toolbar, 
  Typography, 
  Container, 
  CssBaseline,
  ThemeProvider,
  Fade,
  Stack,
  Chip,
  IconButton,
  Tooltip
} from '@mui/material';
import { 
  AutoAwesome, 
  Security, 
  Speed, 
  Notifications,
  Settings,
  Refresh
} from '@mui/icons-material';

// Import components directly (not lazy loaded for now to fix the errors)
import Dashboard from './components/Dashboard';
import KPIDashboard from './components/KPIDashboard';
import AutomationJobs from './components/AutomationJobs';
import Users from './components/Users';
import TestExecutions from './components/TestExecutions';
import WebAutomations from './components/WebAutomations';
import JobSchedules from './components/JobSchedules';
import PerformanceMonitor from './components/PerformanceMonitor';
import Navigation from './components/Navigation';
import Loading from './components/Loading';
import ErrorBoundary from './components/ErrorBoundary';
import theme from './theme/theme';

function App() {
  const [systemStatus, setSystemStatus] = React.useState({
    online: true,
    lastUpdate: new Date(),
    notifications: 3
  });

  const handleRefresh = () => {
    setSystemStatus(prev => ({
      ...prev,
      lastUpdate: new Date()
    }));
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <ErrorBoundary>
        <Router>
          <Box sx={{ 
            flexGrow: 1, 
            minHeight: '100vh', 
            background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
            position: 'relative',
            overflow: 'hidden'
          }}>
            {/* Background Pattern */}
            <Box
              sx={{
                position: 'absolute',
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                background: `
                  radial-gradient(circle at 20% 80%, rgba(25, 118, 210, 0.1) 0%, transparent 50%),
                  radial-gradient(circle at 80% 20%, rgba(156, 39, 176, 0.1) 0%, transparent 50%),
                  radial-gradient(circle at 40% 40%, rgba(76, 175, 80, 0.1) 0%, transparent 50%)
                `,
                pointerEvents: 'none'
              }}
            />

            {/* Enhanced App Bar */}
            <AppBar 
              position="static" 
              elevation={0} 
              sx={{ 
                background: 'linear-gradient(135deg, #1976d2 0%, #1565c0 100%)',
                borderBottom: '1px solid rgba(255,255,255,0.1)',
                backdropFilter: 'blur(10px)',
                position: 'relative',
                zIndex: 1000
              }}
            >
              <Toolbar sx={{ py: 1 }}>
                <Box sx={{ flexGrow: 1, display: 'flex', alignItems: 'center' }}>
                  <Typography 
                    variant="h5" 
                    component="div" 
                    sx={{ 
                      fontWeight: 700,
                      background: 'linear-gradient(45deg, #ffffff 30%, #e3f2fd 90%)',
                      backgroundClip: 'text',
                      WebkitBackgroundClip: 'text',
                      WebkitTextFillColor: 'transparent',
                      mr: 3
                    }}
                  >
                    🏭 Industrial Automation Platform
                  </Typography>
                  
                  <Stack direction="row" spacing={1} sx={{ ml: 2 }}>
                    <Chip 
                      icon={<AutoAwesome />} 
                      label="AI-Enhanced" 
                      size="small" 
                      sx={{ 
                        backgroundColor: 'rgba(255,255,255,0.2)',
                        color: 'white',
                        fontWeight: 600
                      }} 
                    />
                    <Chip 
                      icon={<Security />} 
                      label="Secure" 
                      size="small" 
                      sx={{ 
                        backgroundColor: 'rgba(255,255,255,0.2)',
                        color: 'white',
                        fontWeight: 600
                      }} 
                    />
                    <Chip 
                      icon={<Speed />} 
                      label="Real-time" 
                      size="small" 
                      sx={{ 
                        backgroundColor: 'rgba(255,255,255,0.2)',
                        color: 'white',
                        fontWeight: 600
                      }} 
                    />
                  </Stack>
                </Box>

                <Stack direction="row" spacing={1} alignItems="center">
                  <Tooltip title="System Status">
                    <Chip 
                      label={systemStatus.online ? "Online" : "Offline"} 
                      color={systemStatus.online ? "success" : "error"}
                      size="small"
                      sx={{ color: 'white' }}
                    />
                  </Tooltip>
                  
                  <Tooltip title="Refresh Data">
                    <IconButton 
                      onClick={handleRefresh}
                      sx={{ color: 'white' }}
                    >
                      <Refresh />
                    </IconButton>
                  </Tooltip>
                  
                  <Tooltip title="Notifications">
                    <IconButton sx={{ color: 'white', position: 'relative' }}>
                      <Notifications />
                      {systemStatus.notifications > 0 && (
                        <Box
                          sx={{
                            position: 'absolute',
                            top: 4,
                            right: 4,
                            width: 16,
                            height: 16,
                            borderRadius: '50%',
                            backgroundColor: 'error.main',
                            color: 'white',
                            fontSize: '0.75rem',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            fontWeight: 600
                          }}
                        >
                          {systemStatus.notifications}
                        </Box>
                      )}
                    </IconButton>
                  </Tooltip>
                  
                  <Tooltip title="Settings">
                    <IconButton sx={{ color: 'white' }}>
                      <Settings />
                    </IconButton>
                  </Tooltip>
                </Stack>
              </Toolbar>
            </AppBar>
            
            <Container maxWidth="xl" sx={{ mt: 3, mb: 4, position: 'relative', zIndex: 1 }}>
              <Fade in timeout={500}>
                <Box>
                  <Navigation />
                  
                  <Routes>
                    <Route path="/" element={<Dashboard />} />
                    <Route path="/kpis" element={<KPIDashboard />} />
                    <Route path="/automation-jobs" element={<AutomationJobs />} />
                    <Route path="/users" element={<Users />} />
                    <Route path="/test-executions" element={<TestExecutions />} />
                    <Route path="/web-automations" element={<WebAutomations />} />
                    <Route path="/job-schedules" element={<JobSchedules />} />
                    <Route path="/performance" element={<PerformanceMonitor />} />
                  </Routes>
                </Box>
              </Fade>
            </Container>
          </Box>
        </Router>
      </ErrorBoundary>
    </ThemeProvider>
  );
}

export default App;