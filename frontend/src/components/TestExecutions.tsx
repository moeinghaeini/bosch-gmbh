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
  Chip
} from '@mui/material';
import { DataGrid, GridColDef, GridActionsCellItem } from '@mui/x-data-grid';
import { Add, Edit, Delete, Psychology, PlayArrow } from '@mui/icons-material';
import { getTestExecutions, createTestExecution, updateTestExecution, deleteTestExecution, analyzeTestExecution } from '../services/mockApi.ts';

interface TestExecution {
  id: number;
  testName: string;
  testType: string;
  status: string;
  testSuite: string;
  testData: string;
  expectedResult: string;
  actualResult: string;
  errorMessage?: string;
  executionTime?: string;
  aiAnalysis?: string;
  confidenceScore?: string;
  testEnvironment: string;
  browser: string;
  device: string;
  createdAt: string;
  updatedAt?: string;
}

const TestExecutions: React.FC = () => {
  const [open, setOpen] = useState(false);
  const [editingTest, setEditingTest] = useState<TestExecution | null>(null);
  const [formData, setFormData] = useState({
    testName: '',
    testType: '',
    status: 'Pending',
    testSuite: '',
    testData: '',
    expectedResult: '',
    testEnvironment: '',
    browser: 'Chrome',
    device: 'Desktop'
  });

  const queryClient = useQueryClient();

  const { data: testExecutions, isLoading, error } = useQuery('testExecutions', getTestExecutions);

  const createMutation = useMutation(createTestExecution, {
    onSuccess: () => {
      queryClient.invalidateQueries('testExecutions');
      setOpen(false);
      resetForm();
    }
  });

  const updateMutation = useMutation(updateTestExecution, {
    onSuccess: () => {
      queryClient.invalidateQueries('testExecutions');
      setOpen(false);
      resetForm();
    }
  });

  const deleteMutation = useMutation(deleteTestExecution, {
    onSuccess: () => {
      queryClient.invalidateQueries('testExecutions');
    }
  });

  const analyzeMutation = useMutation(analyzeTestExecution, {
    onSuccess: () => {
      queryClient.invalidateQueries('testExecutions');
    }
  });

  const resetForm = () => {
    setFormData({
      testName: '',
      testType: '',
      status: 'Pending',
      testSuite: '',
      testData: '',
      expectedResult: '',
      testEnvironment: '',
      browser: 'Chrome',
      device: 'Desktop'
    });
    setEditingTest(null);
  };

  const handleOpen = () => {
    resetForm();
    setOpen(true);
  };

  const handleEdit = (testExecution: TestExecution) => {
    setEditingTest(testExecution);
    setFormData({
      testName: testExecution.testName,
      testType: testExecution.testType,
      status: testExecution.status,
      testSuite: testExecution.testSuite,
      testData: testExecution.testData,
      expectedResult: testExecution.expectedResult,
      testEnvironment: testExecution.testEnvironment,
      browser: testExecution.browser,
      device: testExecution.device
    });
    setOpen(true);
  };

  const handleSubmit = () => {
    if (editingTest) {
      updateMutation.mutate({ ...editingTest, ...formData });
    } else {
      createMutation.mutate(formData);
    }
  };

  const handleDelete = (id: number) => {
    if (window.confirm('Are you sure you want to delete this test execution?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleAnalyze = (id: number) => {
    analyzeMutation.mutate(id);
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Passed': return 'success';
      case 'Failed': return 'error';
      case 'Running': return 'warning';
      case 'Pending': return 'default';
      default: return 'default';
    }
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'ID', width: 70 },
    { field: 'testName', headerName: 'Test Name', width: 200 },
    { field: 'testType', headerName: 'Type', width: 120 },
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
    { field: 'testSuite', headerName: 'Test Suite', width: 150 },
    { field: 'testEnvironment', headerName: 'Environment', width: 120 },
    { field: 'browser', headerName: 'Browser', width: 100 },
    { field: 'device', headerName: 'Device', width: 100 },
    { field: 'createdAt', headerName: 'Created', width: 150 },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 150,
      getActions: (params) => [
        <GridActionsCellItem
          icon={<Psychology />}
          label="AI Analyze"
          onClick={() => handleAnalyze(params.row.id)}
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
        Error loading test executions. Please try again later.
      </Alert>
    );
  }

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">
          Quality Control
        </Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleOpen}
        >
          Add Test Execution
        </Button>
      </Box>

      <Box sx={{ height: 400, width: '100%' }}>
        <DataGrid
          rows={testExecutions || []}
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
          {editingTest ? 'Edit Test Execution' : 'Add Test Execution'}
        </DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              fullWidth
              label="Test Name"
              value={formData.testName}
              onChange={(e) => setFormData({ ...formData, testName: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Test Type</InputLabel>
              <Select
                value={formData.testType}
                onChange={(e) => setFormData({ ...formData, testType: e.target.value })}
              >
                <MenuItem value="Unit">Unit Test</MenuItem>
                <MenuItem value="Integration">Integration Test</MenuItem>
                <MenuItem value="E2E">End-to-End Test</MenuItem>
                <MenuItem value="Performance">Performance Test</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Test Suite"
              value={formData.testSuite}
              onChange={(e) => setFormData({ ...formData, testSuite: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.status}
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              >
                <MenuItem value="Pending">Pending</MenuItem>
                <MenuItem value="Running">Running</MenuItem>
                <MenuItem value="Passed">Passed</MenuItem>
                <MenuItem value="Failed">Failed</MenuItem>
                <MenuItem value="Skipped">Skipped</MenuItem>
              </Select>
            </FormControl>
            <TextField
              fullWidth
              label="Test Data (JSON)"
              value={formData.testData}
              onChange={(e) => setFormData({ ...formData, testData: e.target.value })}
              margin="normal"
              multiline
              rows={3}
            />
            <TextField
              fullWidth
              label="Expected Result"
              value={formData.expectedResult}
              onChange={(e) => setFormData({ ...formData, expectedResult: e.target.value })}
              margin="normal"
              multiline
              rows={2}
            />
            <TextField
              fullWidth
              label="Test Environment"
              value={formData.testEnvironment}
              onChange={(e) => setFormData({ ...formData, testEnvironment: e.target.value })}
              margin="normal"
            />
            <FormControl fullWidth margin="normal">
              <InputLabel>Browser</InputLabel>
              <Select
                value={formData.browser}
                onChange={(e) => setFormData({ ...formData, browser: e.target.value })}
              >
                <MenuItem value="Chrome">Chrome</MenuItem>
                <MenuItem value="Firefox">Firefox</MenuItem>
                <MenuItem value="Safari">Safari</MenuItem>
                <MenuItem value="Edge">Edge</MenuItem>
              </Select>
            </FormControl>
            <FormControl fullWidth margin="normal">
              <InputLabel>Device</InputLabel>
              <Select
                value={formData.device}
                onChange={(e) => setFormData({ ...formData, device: e.target.value })}
              >
                <MenuItem value="Desktop">Desktop</MenuItem>
                <MenuItem value="Mobile">Mobile</MenuItem>
                <MenuItem value="Tablet">Tablet</MenuItem>
              </Select>
            </FormControl>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpen(false)}>Cancel</Button>
          <Button onClick={handleSubmit} variant="contained">
            {editingTest ? 'Update' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default TestExecutions;
