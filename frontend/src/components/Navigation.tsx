import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Tabs, Tab, Box, Paper, Chip, Stack, Typography } from '@mui/material';
import { 
  Dashboard as DashboardIcon,
  Assignment,
  People,
  Psychology,
  Web,
  Schedule,
  Analytics
} from '@mui/icons-material';

const Navigation: React.FC = () => {
  const location = useLocation();

  const getTabValue = () => {
    switch (location.pathname) {
      case '/':
        return 0;
      case '/kpis':
        return 1;
      case '/automation-jobs':
        return 2;
      case '/users':
        return 3;
      case '/test-executions':
        return 4;
      case '/web-automations':
        return 5;
      case '/job-schedules':
        return 6;
      case '/performance':
        return 7;
      default:
        return 0;
    }
  };

  const navItems = [
    { label: 'Control Center', path: '/', icon: <DashboardIcon />, color: '#1976d2' },
    { label: 'Performance KPIs', path: '/kpis', icon: <Analytics />, color: '#4caf50' },
    { label: 'Production Lines', path: '/automation-jobs', icon: <Assignment />, color: '#2e7d32' },
    { label: 'Operators', path: '/users', icon: <People />, color: '#ed6c02' },
    { label: 'Quality Control', path: '/test-executions', icon: <Psychology />, color: '#9c27b0' },
    { label: 'System Integration', path: '/web-automations', icon: <Web />, color: '#ff9800' },
    { label: 'Maintenance', path: '/job-schedules', icon: <Schedule />, color: '#607d8b' },
    { label: 'Performance Monitor', path: '/performance', icon: <Analytics />, color: '#03a9f4' }
  ];

  return (
    <Paper 
      elevation={2} 
      sx={{ 
        borderRadius: 3, 
        mb: 3, 
        background: 'linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%)',
        border: '1px solid rgba(0,0,0,0.05)'
      }}
    >
      <Box sx={{ 
        borderBottom: 1, 
        borderColor: 'divider',
        background: 'linear-gradient(90deg, #f5f5f5 0%, #ffffff 100%)',
        borderRadius: '12px 12px 0 0'
      }}>
        <Tabs 
          value={getTabValue()} 
          aria-label="navigation tabs"
          variant="scrollable"
          scrollButtons="auto"
          sx={{
            '& .MuiTab-root': {
              minHeight: 64,
              textTransform: 'none',
              fontWeight: 600,
              fontSize: '0.95rem',
              transition: 'all 0.3s ease',
              '&:hover': {
                backgroundColor: 'rgba(0,0,0,0.04)',
                transform: 'translateY(-1px)'
              }
            },
            '& .Mui-selected': {
              color: 'primary.main',
              fontWeight: 700
            },
            '& .MuiTabs-indicator': {
              height: 3,
              borderRadius: '2px 2px 0 0',
              background: 'linear-gradient(90deg, #1976d2 0%, #1565c0 100%)'
            }
          }}
        >
          {navItems.map((item, index) => (
            <Tab
              key={item.path}
              label={
                <Stack direction="row" alignItems="center" spacing={1}>
                  <Box sx={{ color: item.color, display: 'flex', alignItems: 'center' }}>
                    {item.icon}
                  </Box>
                  <span>{item.label}</span>
                </Stack>
              }
              component={Link}
              to={item.path}
              sx={{
                '&.Mui-selected': {
                  color: item.color,
                  '& .MuiSvgIcon-root': {
                    color: item.color
                  }
                }
              }}
            />
          ))}
        </Tabs>
      </Box>
      
      {/* Status Bar */}
      <Box sx={{ 
        p: 2, 
        background: 'linear-gradient(90deg, #e3f2fd 0%, #f3e5f5 100%)',
        borderRadius: '0 0 12px 12px'
      }}>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
          <Stack direction="row" spacing={1}>
            <Chip 
              label="Industry 4.0" 
              size="small" 
              color="primary" 
              variant="outlined"
              sx={{ fontWeight: 600 }}
            />
            <Chip 
              label="Real-time" 
              size="small" 
              color="success" 
              variant="outlined"
              sx={{ fontWeight: 600 }}
            />
          </Stack>
          <Typography variant="caption" color="text.secondary" sx={{ fontWeight: 500 }}>
            Bosch Industrial Systems • All Systems Operational
          </Typography>
        </Stack>
      </Box>
    </Paper>
  );
};

export default Navigation;