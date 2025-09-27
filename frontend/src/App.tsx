import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { Box, AppBar, Toolbar, Typography, Container, CssBaseline } from '@mui/material';
import Dashboard from './components/Dashboard.tsx';
import KPIDashboard from './components/KPIDashboard.tsx';
import AutomationJobs from './components/AutomationJobs.tsx';
import Users from './components/Users.tsx';
import TestExecutions from './components/TestExecutions.tsx';
import WebAutomations from './components/WebAutomations.tsx';
import JobSchedules from './components/JobSchedules.tsx';
import Navigation from './components/Navigation.tsx';

function App() {
  return (
    <Router>
      <Box sx={{ flexGrow: 1, minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
        <AppBar position="static" elevation={0} sx={{ 
          background: 'linear-gradient(135deg, #1976d2 0%, #1565c0 100%)',
          borderBottom: '1px solid rgba(255,255,255,0.1)'
        }}>
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ 
              flexGrow: 1, 
              fontWeight: 600,
              background: 'linear-gradient(45deg, #ffffff 30%, #e3f2fd 90%)',
              backgroundClip: 'text',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent'
            }}>
              🏭 Industrial Automation Platform
            </Typography>
          </Toolbar>
        </AppBar>
        
        <Container maxWidth="xl" sx={{ mt: 3, mb: 4 }}>
          <Navigation />
          
                <Routes>
                  <Route path="/" element={<Dashboard />} />
                  <Route path="/kpis" element={<KPIDashboard />} />
                  <Route path="/automation-jobs" element={<AutomationJobs />} />
                  <Route path="/users" element={<Users />} />
                  <Route path="/test-executions" element={<TestExecutions />} />
                  <Route path="/web-automations" element={<WebAutomations />} />
                  <Route path="/job-schedules" element={<JobSchedules />} />
                </Routes>
        </Container>
      </Box>
    </Router>
  );
}

export default App;