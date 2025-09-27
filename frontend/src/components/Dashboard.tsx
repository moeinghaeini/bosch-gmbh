import React, { Suspense, lazy, memo } from 'react';
import { 
  Grid, 
  Paper, 
  Typography, 
  Box, 
  Card, 
  CardContent,
  Chip,
  LinearProgress,
  Avatar,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Divider,
  Button,
  IconButton,
  Tooltip,
  Stack,
  Fade,
  Zoom,
  Skeleton
} from '@mui/material';
import { 
  Assignment, 
  People, 
  CheckCircle, 
  Error,
  Psychology,
  Web,
  Schedule,
  PlayArrow,
  TrendingUp,
  TrendingDown,
  Refresh,
  Settings,
  Notifications,
  Security,
  Speed,
  Analytics,
  AutoAwesome,
  Rocket,
  Timeline,
  Dashboard as DashboardIcon
} from '@mui/icons-material';

const Dashboard: React.FC = () => {
  // Mock data for demonstration
  const totalJobs = 24;
  const completedJobs = 18;
  const failedJobs = 3;
  const totalUsers = 5;
  const totalTests = 156;
  const passedTests = 142;
  const failedTests = 8;
  const totalWebAutomations = 32;
  const completedWebAutomations = 28;
  const totalJobSchedules = 12;
  const enabledJobSchedules = 10;

  const successRate = Math.round((passedTests / totalTests) * 100);
  const automationRate = Math.round((completedWebAutomations / totalWebAutomations) * 100);
  const jobSuccessRate = Math.round((completedJobs / totalJobs) * 100);

  const StatCard = ({ title, value, icon, color, trend, subtitle }: any) => (
    <Zoom in={true} timeout={500}>
      <Card sx={{ 
        height: '100%', 
        background: `linear-gradient(135deg, ${color}15 0%, ${color}05 100%)`,
        border: `1px solid ${color}30`,
        borderRadius: 3,
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: `0 8px 25px ${color}25`
        }
      }}>
        <CardContent>
          <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
            <Avatar sx={{ bgcolor: color, width: 48, height: 48 }}>
              {icon}
            </Avatar>
            <Box display="flex" alignItems="center" gap={1}>
              {trend > 0 ? <TrendingUp color="success" /> : trend < 0 ? <TrendingDown color="error" /> : null}
              <Typography variant="body2" color="text.secondary">
                {trend > 0 ? `+${trend}%` : trend < 0 ? `${trend}%` : '0%'}
              </Typography>
            </Box>
          </Box>
          <Typography variant="h4" component="div" sx={{ fontWeight: 700, color: color }}>
            {value}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
            {title}
          </Typography>
          {subtitle && (
            <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, display: 'block' }}>
              {subtitle}
            </Typography>
          )}
        </CardContent>
      </Card>
    </Zoom>
  );

  const QuickActionCard = ({ title, description, icon, color, action }: any) => (
    <Fade in={true} timeout={700}>
      <Card sx={{ 
        height: '100%', 
        cursor: 'pointer',
        transition: 'all 0.3s ease',
        '&:hover': {
          transform: 'translateY(-2px)',
          boxShadow: 4
        }
      }}>
        <CardContent sx={{ textAlign: 'center', p: 3 }}>
          <Avatar sx={{ bgcolor: color, width: 56, height: 56, mx: 'auto', mb: 2 }}>
            {icon}
          </Avatar>
          <Typography variant="h6" component="div" sx={{ fontWeight: 600, mb: 1 }}>
            {title}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            {description}
          </Typography>
          <Button 
            variant="contained" 
            size="small" 
            sx={{ 
              bgcolor: color,
              '&:hover': { bgcolor: color, opacity: 0.9 }
            }}
            onClick={action}
          >
            Get Started
          </Button>
        </CardContent>
      </Card>
    </Fade>
  );

  return (
    <Box>
      {/* Header Section */}
      <Box mb={4}>
        <Box display="flex" alignItems="center" mb={2}>
          <img 
            src="/logo.png" 
            alt="Bosch Logo" 
            style={{ 
              height: '60px', 
              width: 'auto', 
              marginRight: '16px',
              filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.1))'
            }} 
          />
          <Box>
            <Typography variant="h4" component="h1" sx={{ fontWeight: 700, mb: 0 }}>
              🏭 Industrial Control Center
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Monitor and manage all industrial automation systems from this central control hub. Real-time production oversight and system management.
            </Typography>
          </Box>
        </Box>
        
        <Stack direction="row" spacing={2} alignItems="center">
          <Chip 
            icon={<CheckCircle />} 
            label="System Online" 
            color="success" 
            variant="outlined"
          />
          <Chip 
            icon={<Security />} 
            label="Secure Environment" 
            color="info" 
            variant="outlined"
          />
          <Chip 
            icon={<AutoAwesome />} 
            label="AI-Enhanced" 
            color="secondary" 
            variant="outlined"
          />
        </Stack>
      </Box>

      {/* Key Metrics */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Jobs"
            value={totalJobs}
            icon={<Assignment />}
            color="#1976d2"
            trend={jobSuccessRate > 80 ? 5 : -2}
            subtitle={`${jobSuccessRate}% success rate`}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Test Executions"
            value={totalTests}
            icon={<Psychology />}
            color="#9c27b0"
            trend={successRate > 90 ? 8 : -3}
            subtitle={`${successRate}% pass rate`}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Web Automations"
            value={totalWebAutomations}
            icon={<Web />}
            color="#ff9800"
            trend={automationRate > 85 ? 6 : -1}
            subtitle={`${automationRate}% completion rate`}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Active Users"
            value={totalUsers}
            icon={<People />}
            color="#4caf50"
            trend={2}
            subtitle="System administrators"
          />
        </Grid>
      </Grid>

      {/* Performance Overview */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12} md={8}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Box display="flex" alignItems="center" justifyContent="space-between" mb={3}>
                <Typography variant="h6" sx={{ fontWeight: 600 }}>
                  📊 System Performance Overview
                </Typography>
                <IconButton size="small">
                  <Refresh />
                </IconButton>
              </Box>
              
              <Grid container spacing={3}>
                <Grid item xs={12} sm={4}>
                  <Box textAlign="center">
                    <Typography variant="h3" sx={{ fontWeight: 700, color: 'success.main' }}>
                      {successRate}%
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Test Success Rate
                    </Typography>
                    <LinearProgress 
                      variant="determinate" 
                      value={successRate} 
                      sx={{ mt: 1, height: 8, borderRadius: 4 }}
                      color="success"
                    />
                  </Box>
                </Grid>
                <Grid item xs={12} sm={4}>
                  <Box textAlign="center">
                    <Typography variant="h3" sx={{ fontWeight: 700, color: 'primary.main' }}>
                      {automationRate}%
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Automation Success
                    </Typography>
                    <LinearProgress 
                      variant="determinate" 
                      value={automationRate} 
                      sx={{ mt: 1, height: 8, borderRadius: 4 }}
                      color="primary"
                    />
                  </Box>
                </Grid>
                <Grid item xs={12} sm={4}>
                  <Box textAlign="center">
                    <Typography variant="h3" sx={{ fontWeight: 700, color: 'warning.main' }}>
                      {enabledJobSchedules}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Active Schedules
                    </Typography>
                    <LinearProgress 
                      variant="determinate" 
                      value={(enabledJobSchedules / Math.max(totalJobSchedules, 1)) * 100} 
                      sx={{ mt: 1, height: 8, borderRadius: 4 }}
                      color="warning"
                    />
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={4}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
                🎯 Quick Actions
              </Typography>
              <List dense>
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'primary.main', width: 32, height: 32 }}>
                      <PlayArrow />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Run Test Suite" 
                    secondary="Execute automated tests"
                  />
                </ListItem>
                <Divider />
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'secondary.main', width: 32, height: 32 }}>
                      <Web />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Start Web Automation" 
                    secondary="Launch AI web tasks"
                  />
                </ListItem>
                <Divider />
                <ListItem>
                  <ListItemAvatar>
                    <Avatar sx={{ bgcolor: 'success.main', width: 32, height: 32 }}>
                      <Schedule />
                    </Avatar>
                  </ListItemAvatar>
                  <ListItemText 
                    primary="Schedule Job" 
                    secondary="Create new automation job"
                  />
                </ListItem>
              </List>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Thesis Topics Overview */}
      <Grid container spacing={3} mb={4}>
        <Grid item xs={12}>
          <Typography variant="h5" sx={{ fontWeight: 600, mb: 3 }}>
            🎓 Thesis Topics Implementation
          </Typography>
        </Grid>
        
        <Grid item xs={12} md={4}>
          <QuickActionCard
            title="AI Test Execution"
            description="Advanced AI methods for automated test execution with intelligent analysis and optimization."
            icon={<Psychology />}
            color="#9c27b0"
            action={() => window.location.href = '/test-executions'}
          />
        </Grid>
        
        <Grid item xs={12} md={4}>
          <QuickActionCard
            title="Web Automation"
            description="AI-based web automation solution for intelligent interaction with third-party websites."
            icon={<Web />}
            color="#ff9800"
            action={() => window.location.href = '/web-automations'}
          />
        </Grid>
        
        <Grid item xs={12} md={4}>
          <QuickActionCard
            title="Back-Office Interface"
            description="Comprehensive supervision interface for automation task orchestration and management."
            icon={<Settings />}
            color="#4caf50"
            action={() => window.location.href = '/job-schedules'}
          />
        </Grid>
      </Grid>

      {/* System Status */}
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
                🔧 System Status
              </Typography>
              <Stack spacing={2}>
                <Box display="flex" alignItems="center" justifyContent="space-between">
                  <Box display="flex" alignItems="center" gap={1}>
                    <CheckCircle color="success" />
                    <Typography variant="body2">Frontend Application</Typography>
                  </Box>
                  <Chip label="Online" color="success" size="small" />
                </Box>
                <Box display="flex" alignItems="center" justifyContent="space-between">
                  <Box display="flex" alignItems="center" gap={1}>
                    <CheckCircle color="success" />
                    <Typography variant="body2">Backend API</Typography>
                  </Box>
                  <Chip label="Running" color="success" size="small" />
                </Box>
                <Box display="flex" alignItems="center" justifyContent="space-between">
                  <Box display="flex" alignItems="center" gap={1}>
                    <CheckCircle color="success" />
                    <Typography variant="body2">Database</Typography>
                  </Box>
                  <Chip label="Connected" color="success" size="small" />
                </Box>
                <Box display="flex" alignItems="center" justifyContent="space-between">
                  <Box display="flex" alignItems="center" gap={1}>
                    <AutoAwesome color="primary" />
                    <Typography variant="body2">AI Services</Typography>
                  </Box>
                  <Chip label="Ready" color="primary" size="small" />
                </Box>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" sx={{ fontWeight: 600, mb: 2 }}>
                🚀 Ready for Development
              </Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              </Typography>
              <Stack direction="row" spacing={1} flexWrap="wrap" gap={1}>
                <Chip label="Clean Architecture" color="primary" variant="outlined" size="small" />
                <Chip label="AI Integration" color="secondary" variant="outlined" size="small" />
                <Chip label="Docker Ready" color="info" variant="outlined" size="small" />
                <Chip label="Production Ready" color="success" variant="outlined" size="small" />
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Personal Signature */}
      <Box sx={{ 
        mt: 6, 
        pt: 4, 
        borderTop: '1px solid rgba(0,0,0,0.1)',
        textAlign: 'center'
      }}>
        <Typography 
          variant="body2" 
          sx={{ 
            color: 'text.secondary',
            fontStyle: 'italic',
            fontSize: '0.9rem',
            opacity: 0.8
          }}
        >
          Made with ❤️ from Moein Ghaeini for Bosch
        </Typography>
      </Box>
    </Box>
  );
};

export default Dashboard;