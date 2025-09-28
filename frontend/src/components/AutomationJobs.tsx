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
  CircularProgress
} from '@mui/material';
import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Add, Edit, Delete } from '@mui/icons-material';
import { getAutomationJobs, createAutomationJob, updateAutomationJob, deleteAutomationJob } from '../services/mockApi';

interface AutomationJob {
  id: number;
  name: string;
  description: string;
  status: string;
  jobType: string;
  scheduledAt?: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  configuration: string;
  createdAt: string;
  updatedAt?: string;
}

const AutomationJobs: React.FC = () => {
  const [open, setOpen] = useState(false);
  const [editingJob, setEditingJob] = useState<AutomationJob | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    jobType: '',
    status: 'Pending',
    configuration: ''
  });

  const queryClient = useQueryClient();

  const { data: jobs, isLoading, error } = useQuery('automationJobs', getAutomationJobs);

  const createMutation = useMutation(createAutomationJob, {
    onSuccess: () => {
      queryClient.invalidateQueries('automationJobs');
      setOpen(false);
      resetForm();
    }
  });

  const updateMutation = useMutation(updateAutomationJob, {
    onSuccess: () => {
      queryClient.invalidateQueries('automationJobs');
      setOpen(false);
      resetForm();
    }
  });

  const deleteMutation = useMutation(deleteAutomationJob, {
    onSuccess: () => {
      queryClient.invalidateQueries('automationJobs');
    }
  });

  const resetForm = () => {
    setFormData({
      name: '',
      description: '',
      jobType: '',
      status: 'Pending',
      configuration: ''
    });
    setEditingJob(null);
  };

  const handleOpen = () => {
    resetForm();
    setOpen(true);
  };

  const handleEdit = (job: AutomationJob) => {
    setEditingJob(job);
    setFormData({
      name: job.name,
      description: job.description,
      jobType: job.jobType,
      status: job.status,
      configuration: job.configuration
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
    if (window.confirm('Are you sure you want to delete this job?')) {
      deleteMutation.mutate(id);
    }
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'name', headerName: 'Name', width: 200 },
    { field: 'description', headerName: 'Description', width: 300 },
    { field: 'jobType', headerName: 'Type', width: 150 },
    { field: 'status', headerName: 'Status', width: 120 },
    { field: 'scheduledAt', headerName: 'Scheduled', width: 150 },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 100,
      getActions: (params) => [
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
        Error loading automation jobs. Please try again later.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          Production Lines
        </Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleOpen}
        >
          Add Job
        </Button>
      </Box>

      <Box sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={jobs || []}
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
          {editingJob ? 'Edit Automation Job' : 'Add Automation Job'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              fullWidth
              label="Name"
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              margin="normal"
            />
            <TextField
              fullWidth
              label="Description"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              margin="normal"
              multiline
              rows={3}
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Job Type</InputLabel>
              <Select
                value={formData.jobType}
                onChange={(e) => setFormData({ ...formData, jobType: e.target.value })}
              >
                <MenuItem value="WebAutomation">Web Automation</MenuItem>
                <MenuItem value="TestExecution">Test Execution</MenuItem>
                <MenuItem value="DataProcessing">Data Processing</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.status}
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              >
                <MenuItem value="Pending">Pending</MenuItem>
                <MenuItem value="Running">Running</MenuItem>
                <MenuItem value="Completed">Completed</MenuItem>
                <MenuItem value="Failed">Failed</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Configuration (JSON)"
              value={formData.configuration}
              onChange={(e) => setFormData({ ...formData, configuration: e.target.value })}
              margin="normal"
              multiline
              rows={4}
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

export default AutomationJobs;
