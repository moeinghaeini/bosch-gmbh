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
import { getAutomationJobs, createAutomationJob, updateAutomationJob, deleteAutomationJob } from '../services/realApi';

interface AutomationJob {
  id: number;
  name: string;
  description: string;
  statusId: number;
  jobTypeId: number;
  scheduledAt?: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  configuration?: string;
  createdAt: string;
  updatedAt?: string;
}

const AutomationJobs: React.FC = () => {
  const [open, setOpen] = useState(false);
  const [editingJob, setEditingJob] = useState<AutomationJob | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    jobTypeId: 1,
    statusId: 1,
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
      jobTypeId: 1,
      statusId: 1,
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
      jobTypeId: job.jobTypeId,
      statusId: job.statusId,
      configuration: job.configuration || ''
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
    { field: 'jobTypeId', headerName: 'Type ID', width: 100 },
    { field: 'statusId', headerName: 'Status ID', width: 100 },
    { field: 'createdAt', headerName: 'Created', width: 150 },
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
                value={formData.jobTypeId}
                onChange={(e) => setFormData({ ...formData, jobTypeId: Number(e.target.value) })}
              >
                <MenuItem value={1}>Web Automation</MenuItem>
                <MenuItem value={2}>Test Execution</MenuItem>
                <MenuItem value={3}>Data Processing</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.statusId}
                onChange={(e) => setFormData({ ...formData, statusId: Number(e.target.value) })}
              >
                <MenuItem value={1}>Pending</MenuItem>
                <MenuItem value={2}>Running</MenuItem>
                <MenuItem value={3}>Completed</MenuItem>
                <MenuItem value={4}>Failed</MenuItem>
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
