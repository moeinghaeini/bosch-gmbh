import React, { useState } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Button,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  LinearProgress,
  Alert,
  Paper,
  Stack,
  IconButton,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Divider
} from '@mui/material';
import {
  Psychology,
  AutoAwesome,
  Visibility,
  Speed,
  Analytics,
  Computer,
  BugReport,
  TrendingUp,
  Refresh,
  Download,
  Share,
  Info
} from '@mui/icons-material';

interface AIAnalysisResult {
  id: string;
  type: string;
  confidence: number;
  insights: string[];
  recommendations: string[];
  performance: {
    accuracy: number;
    precision: number;
    recall: number;
    f1Score: number;
  };
  timestamp: string;
}

const AIAnalysis: React.FC = () => {
  const [analysisType, setAnalysisType] = useState('test-execution');
  const [inputData, setInputData] = useState('');
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [results, setResults] = useState<AIAnalysisResult[]>([]);
  const [selectedResult, setSelectedResult] = useState<AIAnalysisResult | null>(null);
  const [openDialog, setOpenDialog] = useState(false);

  const analysisTypes = [
    { value: 'test-execution', label: 'Test Execution Analysis', icon: <BugReport /> },
    { value: 'web-automation', label: 'Web Automation Analysis', icon: <Computer /> },
    { value: 'performance', label: 'Performance Analysis', icon: <Speed /> },
    { value: 'computer-vision', label: 'Computer Vision Analysis', icon: <Visibility /> },
    { value: 'experimental', label: 'Experimental Analysis', icon: <Analytics /> },
    { value: 'model-comparison', label: 'AI Model Comparison', icon: <Psychology /> }
  ];

  const handleAnalyze = async () => {
    setIsAnalyzing(true);
    try {
      // Simulate AI analysis
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      const newResult: AIAnalysisResult = {
        id: Date.now().toString(),
        type: analysisType,
        confidence: Math.random() * 0.4 + 0.6, // 60-100%
        insights: [
          'Identified 3 potential performance bottlenecks',
          'Found 2 accessibility issues in web elements',
          'Detected 1 security vulnerability in test data'
        ],
        recommendations: [
          'Optimize database queries for better performance',
          'Add ARIA labels for better accessibility',
          'Implement input validation for security'
        ],
        performance: {
          accuracy: 0.94,
          precision: 0.91,
          recall: 0.89,
          f1Score: 0.90
        },
        timestamp: new Date().toISOString()
      };
      
      setResults(prev => [newResult, ...prev]);
    } catch (error) {
      console.error('Analysis failed:', error);
    } finally {
      setIsAnalyzing(false);
    }
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 0.9) return 'success';
    if (confidence >= 0.7) return 'warning';
    return 'error';
  };

  const getConfidenceLabel = (confidence: number) => {
    if (confidence >= 0.9) return 'High';
    if (confidence >= 0.7) return 'Medium';
    return 'Low';
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Psychology color="primary" />
        AI Analysis Center
      </Typography>
      
      <Typography variant="body1" color="text.secondary" paragraph>
        Advanced AI-powered analysis for test execution, web automation, and system performance.
      </Typography>

      {/* Analysis Input */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Start AI Analysis
          </Typography>
          
          <Grid container spacing={3}>
            <Grid item xs={12} md={4}>
              <FormControl fullWidth>
                <InputLabel>Analysis Type</InputLabel>
                <Select
                  value={analysisType}
                  onChange={(e) => setAnalysisType(e.target.value)}
                >
                  {analysisTypes.map((type) => (
                    <MenuItem key={type.value} value={type.value}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        {type.icon}
                        {type.label}
                      </Box>
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
            
            <Grid item xs={12} md={8}>
              <TextField
                fullWidth
                multiline
                rows={4}
                label="Input Data (JSON, text, or description)"
                value={inputData}
                onChange={(e) => setInputData(e.target.value)}
                placeholder="Enter test results, automation data, or performance metrics for AI analysis..."
              />
            </Grid>
          </Grid>
          
          <Box sx={{ mt: 2, display: 'flex', gap: 2 }}>
            <Button
              variant="contained"
              startIcon={<AutoAwesome />}
              onClick={handleAnalyze}
              disabled={!inputData.trim() || isAnalyzing}
              size="large"
            >
              {isAnalyzing ? 'Analyzing...' : 'Start AI Analysis'}
            </Button>
            
            <Button
              variant="outlined"
              startIcon={<Refresh />}
              onClick={() => {
                setInputData('');
                setResults([]);
              }}
            >
              Clear
            </Button>
          </Box>
          
          {isAnalyzing && (
            <Box sx={{ mt: 2 }}>
              <LinearProgress />
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                AI is analyzing your data... This may take a few moments.
              </Typography>
            </Box>
          )}
        </CardContent>
      </Card>

      {/* Analysis Results */}
      {results.length > 0 && (
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              <Speed /> AI Analysis Results
            </Typography>
            
            <Grid container spacing={2}>
              {results.map((result) => (
                <Grid item xs={12} md={6} key={result.id}>
                  <Paper sx={{ p: 2, border: '1px solid', borderColor: 'divider' }}>
                    <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
                      <Typography variant="subtitle1" fontWeight="bold">
                        {analysisTypes.find(t => t.value === result.type)?.label}
                      </Typography>
                      <Chip
                        label={`${getConfidenceLabel(result.confidence)} Confidence`}
                        color={getConfidenceColor(result.confidence) as any}
                        size="small"
                      />
                    </Stack>
                    
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      Confidence: {(result.confidence * 100).toFixed(1)}%
                    </Typography>
                    
                    <Box sx={{ mb: 2 }}>
                      <Typography variant="subtitle2" gutterBottom>
                        Performance Metrics:
                      </Typography>
                      <Grid container spacing={1}>
                        <Grid item xs={6}>
                          <Typography variant="body2">
                            Accuracy: {(result.performance.accuracy * 100).toFixed(1)}%
                          </Typography>
                        </Grid>
                        <Grid item xs={6}>
                          <Typography variant="body2">
                            F1 Score: {(result.performance.f1Score * 100).toFixed(1)}%
                          </Typography>
                        </Grid>
                      </Grid>
                    </Box>
                    
                    <Stack direction="row" spacing={1}>
                      <Button
                        size="small"
                        onClick={() => {
                          setSelectedResult(result);
                          setOpenDialog(true);
                        }}
                      >
                        View Details
                      </Button>
                      <Button size="small" startIcon={<Download />}>
                        Export
                      </Button>
                      <Button size="small" startIcon={<Share />}>
                        Share
                      </Button>
                    </Stack>
                  </Paper>
                </Grid>
              ))}
            </Grid>
          </CardContent>
        </Card>
      )}

      {/* Detailed Results Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Psychology color="primary" />
            Detailed AI Analysis Results
          </Box>
        </DialogTitle>
        <DialogContent>
          {selectedResult && (
            <Box>
              <Typography variant="h6" gutterBottom>
                Key Insights
              </Typography>
              <List>
                {selectedResult.insights.map((insight, index) => (
                  <ListItem key={index}>
                    <ListItemIcon>
                      <Info color="primary" />
                    </ListItemIcon>
                    <ListItemText primary={insight} />
                  </ListItem>
                ))}
              </List>
              
              <Divider sx={{ my: 2 }} />
              
              <Typography variant="h6" gutterBottom>
                Recommendations
              </Typography>
              <List>
                {selectedResult.recommendations.map((recommendation, index) => (
                  <ListItem key={index}>
                    <ListItemIcon>
                      <TrendingUp color="success" />
                    </ListItemIcon>
                    <ListItemText primary={recommendation} />
                  </ListItem>
                ))}
              </List>
              
              <Divider sx={{ my: 2 }} />
              
              <Typography variant="h6" gutterBottom>
                Performance Metrics
              </Typography>
              <Grid container spacing={2}>
                <Grid item xs={6}>
                  <Paper sx={{ p: 2, textAlign: 'center' }}>
                    <Typography variant="h4" color="primary">
                      {(selectedResult.performance.accuracy * 100).toFixed(1)}%
                    </Typography>
                    <Typography variant="body2">Accuracy</Typography>
                  </Paper>
                </Grid>
                <Grid item xs={6}>
                  <Paper sx={{ p: 2, textAlign: 'center' }}>
                    <Typography variant="h4" color="secondary">
                      {(selectedResult.performance.f1Score * 100).toFixed(1)}%
                    </Typography>
                    <Typography variant="body2">F1 Score</Typography>
                  </Paper>
                </Grid>
              </Grid>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Close</Button>
          <Button variant="contained" startIcon={<Download />}>
            Export Report
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AIAnalysis;
