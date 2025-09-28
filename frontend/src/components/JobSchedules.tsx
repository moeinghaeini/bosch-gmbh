import React, { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Alert,
  CircularProgress,
  Chip,
  Grid,
  Card,
  CardContent,
  Switch,
  FormControlLabel
} from '@mui/material';
import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Add, Edit, Delete, PlayArrow, Pause, Schedule } from '@mui/icons-material';
import { getJobSchedules, createJobSchedule, updateJobSchedule, deleteJobSchedule } from '../services/mockApi';

interface JobSchedule {
  id: number;
  jobName: string;
  jobType: string;
  status: string;
  cronExpression: string;
  nextRunTime?: string;
  lastRunTime?: string;
  configuration: string;
  priority: string;
  isEnabled: boolean;
  timeZone: string;
  maxRetries: number;
  currentRetries: number;
  errorMessage?: string;
  executionHistory: string;
  notifications: string;
  dependencies: string;
  createdAt: string;
  updatedAt?: string;
}

const JobSchedules: React.FC = () => {
  const [open, setOpen] = useState(false);
  const [editingJob, setEditingJob] = useState<JobSchedule | null>(null);
  const [formData, setFormData] = useState({
    jobName: '',
    jobType: '',
    cronExpression: '',
    configuration: '',
    priority: 'Normal',
    isEnabled: true,
    timeZone: 'UTC',
    maxRetries: 3,
    notifications: '',
    dependencies: ''
  });

  const queryClient = useQueryClient();

  const { data: jobSchedules, isLoading, error } = useQuery('jobSchedules', getJobSchedules);

  const createMutation = useMutation(createJobSchedule, {
    onSuccess: () => {
      queryClient.invalidateQueries('jobSchedules');
      setOpen(false);
      resetForm();
    }
  });

  const updateMutation = useMutation(updateJobSchedule, {
    onSuccess: () => {
      queryClient.invalidateQueries('jobSchedules');
      setOpen(false);
      resetForm();
    }
  });

  const deleteMutation = useMutation(deleteJobSchedule, {
    onSuccess: () => {
      queryClient.invalidateQueries('jobSchedules');
    }
  });

  const resetForm = () => {
    setFormData({
      jobName: '',
      jobType: '',
      cronExpression: '',
      configuration: '',
      priority: 'Normal',
      isEnabled: true,
      timeZone: 'UTC',
      maxRetries: 3,
      notifications: '',
      dependencies: ''
    });
    setEditingJob(null);
  };

  const handleOpen = () => {
    resetForm();
    setOpen(true);
  };

  const handleEdit = (jobSchedule: JobSchedule) => {
    setEditingJob(jobSchedule);
    setFormData({
      jobName: jobSchedule.jobName,
      jobType: jobSchedule.jobType,
      cronExpression: jobSchedule.cronExpression,
      configuration: jobSchedule.configuration,
      priority: jobSchedule.priority,
      isEnabled: jobSchedule.isEnabled,
      timeZone: jobSchedule.timeZone,
      maxRetries: jobSchedule.maxRetries,
      notifications: jobSchedule.notifications,
      dependencies: jobSchedule.dependencies
    });
    setOpen(true);
  };

  const handleSubmit = () => {
    if (editingJob) {
      updateMutation.mutate({ ...editingJob, ...formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleDelete = (id: number) => {
    if (window.confirm('Are you sure you want to delete this job schedule?')) {
      deleteMutation.mutate(id);
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'success';
      case 'Failed': return 'error';
      case 'Running': return 'warning';
      case 'Scheduled': return 'info';
      case 'Cancelled': return 'default';
      default: return 'default';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'Critical': return 'error';
      case 'High': return 'warning';
      case 'Normal': return 'info';
      case 'Low': return 'default';
      default: return 'default';
    }
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'jobName', headerName: 'Job Name', width: 200 },
    { field: 'jobType', headerName: 'Type', width: 150 },
    { 
      field: 'status', 
      headerName: 'Status', 
      width: 120,
      renderCell: (params) => (
        <Chip 
          label={params.value} 
          color={getStatusColor(params.value) as any}
          size="small"
        />
      )
    },
    { 
      field: 'priority', 
      headerName: 'Priority', 
      width: 100,
      renderCell: (params) => (
        <Chip 
          label={params.value} 
          color={getPriorityColor(params.value) as any}
          size="small"
        />
      )
    },
    { field: 'cronExpression', headerName: 'Schedule', width: 150 },
    { field: 'nextRunTime', headerName: 'Next Run', width: 150 },
    { field: 'lastRunTime', headerName: 'Last Run', width: 150 },
    { 
      field: 'isEnabled', 
      headerName: 'Enabled', 
      width: 100,
      renderCell: (params) => (
        <Chip 
          label={params.value ? 'Yes' : 'No'} 
          color={params.value ? 'success' : 'default'}
          size="small"
        />
      )
    },
    { field: 'createdAt', headerName: 'Created', width: 150 },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 200,
      getActions: (params) => [
        <GridActionsCellItem
          icon={<PlayArrow />}
          label="Run Now"
          onClick={() => {/* Handle run now */}}
        />,
        <GridActionsCellItem
          icon={params.row.isEnabled ? <Pause /> : <PlayArrow />}
          label={params.row.isEnabled ? 'Disable' : 'Enable'}
          onClick={() => {/* Handle enable/disable */}}
        />,
        <GridActionsCellItem
          icon={<Edit />}
          label="Edit"
          onClick={() => handleEdit(params.row)}
        />,
        <GridActionsCellItem
          icon={<Delete />}
          label="Delete"
          onClick={() => handleDelete(params.row.id)}
        />
      ]
    }
  ];

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        Error loading job schedules. Please try again later.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          Maintenance
        </Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleOpen}
        >
          Add Job Schedule
        </Button>
      </Box>

      <Grid container spacing={3} mb={3}>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Total Jobs
              </Typography>
              <Typography variant="h4">
                {jobSchedules?.length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Enabled
              </Typography>
              <Typography variant="h4" color="success.main">
                {jobSchedules?.filter(j => j.isEnabled).length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Running
              </Typography>
              <Typography variant="h4" color="warning.main">
                {jobSchedules?.filter(j => j.status === 'Running').length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={3}>
          <Card>
            <CardContent>
              <Typography color="textSecondary" gutterBottom>
                Failed
              </Typography>
              <Typography variant="h4" color="error.main">
                {jobSchedules?.filter(j => j.status === 'Failed').length || 0}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <Box sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={jobSchedules || []}
          columns={columns}
          pageSizeOptions={[5, 10, 25]}
          initialState={{
            pagination: { paginationModel: { pageSize: 10 } }
          }}
          disableRowSelectionOnClick
        />
      </Box>

      <Dialog open={open} onClose={() => setOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {editingJob ? 'Edit Job Schedule' : 'Add Job Schedule'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              fullWidth
              label="Job Name"
              value={formData.jobName}
              onChange={(e) => setFormData({ ...formData, jobName: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Job Type</InputLabel>
              <Select
                value={formData.jobType}
                onChange={(e) => setFormData({ ...formData, jobType: e.target.value })}
              >
                <MenuItem value="TestExecution">Test Execution</MenuItem>
                <MenuItem value="WebAutomation">Web Automation</MenuItem>
                <MenuItem value="DataProcessing">Data Processing</MenuItem>
                <MenuItem value="ReportGeneration">Report Generation</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Cron Expression"
              value={formData.cronExpression}
              onChange={(e) => setFormData({ ...formData, cronExpression: e.target.value })}
              margin="normal"
              placeholder="0 0 * * * (daily at midnight)"
              helperText="Use standard cron format: minute hour day month weekday"
            />
            <TextField
              fullWidth
              label="Configuration (JSON)"
              value={formData.configuration}
              onChange={(e) => setFormData({ ...formData, configuration: e.target.value })}
              margin="normal"
              multiline
              rows={4}
              placeholder='{"param1": "value1", "param2": "value2"}'
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Priority</InputLabel>
              <Select
                value={formData.priority}
                onChange={(e) => setFormData({ ...formData, priority: e.target.value })}
              >
                <MenuItem value="Low">Low</MenuItem>
                <MenuItem value="Normal">Normal</MenuItem>
                <MenuItem value="High">High</MenuItem>
                <MenuItem value="Critical">Critical</MenuItem>
              </Select>
            </FormControl>
            <FormControlLabel
              control={
                <Switch
                  checked={formData.isEnabled}
                  onChange={(e) => setFormData({ ...formData, isEnabled: e.target.checked })}
                />
              }
              label="Enabled"
            />
            <TextField
              fullWidth
              label="Time Zone"
              value={formData.timeZone}
              onChange={(e) => setFormData({ ...formData, timeZone: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Max Retries"
              type="number"
              value={formData.maxRetries}
              onChange={(e) => setFormData({ ...formData, maxRetries: parseInt(e.target.value) })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Notifications (JSON)"
              value={formData.notifications}
              onChange={(e) => setFormData({ ...formData, notifications: e.target.value })}
              margin="normal"
              multiline
              rows={2}
              placeholder='{"email": "admin@example.com", "slack": "#alerts"}'
            />
            <TextField
              fullWidth
              label="Dependencies (JSON)"
              value={formData.dependencies}
              onChange={(e) => setFormData({ ...formData, dependencies: e.target.value })}
              margin="normal"
              multiline
              rows={2}
              placeholder='{"dependsOn": [1, 2, 3]}'
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingJob ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default JobSchedules;
