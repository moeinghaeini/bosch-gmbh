import React, { useState, useRef } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Button,
  Paper,
  Chip,
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
  Divider,
  Alert,
  LinearProgress,
  Fab
} from '@mui/material';
import {
  Visibility,
  CameraAlt,
  Upload,
  Download,
  Share,
  AutoAwesome,
  BugReport,
  CheckCircle,
  Error,
  Info,
  Refresh,
  ZoomIn,
  ZoomOut
} from '@mui/icons-material';

interface DetectedElement {
  id: string;
  type: string;
  confidence: number;
  boundingBox: { x: number; y: number; width: number; height: number };
  selector: string;
  attributes: Record<string, string>;
  text: string;
  color: string;
  accessibility: {
    score: number;
    issues: string[];
  };
}

const ComputerVision: React.FC = () => {
  const [uploadedImage, setUploadedImage] = useState<string | null>(null);
  const [detectedElements, setDetectedElements] = useState<DetectedElement[]>([]);
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [selectedElement, setSelectedElement] = useState<DetectedElement | null>(null);
  const [openDialog, setOpenDialog] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleImageUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        setUploadedImage(e.target?.result as string);
        setDetectedElements([]);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleAnalyzeImage = async () => {
    if (!uploadedImage) return;
    
    setIsAnalyzing(true);
    try {
      // Simulate computer vision analysis
      await new Promise(resolve => setTimeout(resolve, 3000));
      
      const mockElements: DetectedElement[] = [
        {
          id: '1',
          type: 'Button',
          confidence: 0.95,
          boundingBox: { x: 100, y: 200, width: 120, height: 40 },
          selector: 'button[data-testid="submit-btn"]',
          attributes: { 'data-testid': 'submit-btn', 'class': 'btn-primary' },
          text: 'Submit',
          color: '#1976d2',
          accessibility: {
            score: 85,
            issues: ['Missing aria-label']
          }
        },
        {
          id: '2',
          type: 'Input',
          confidence: 0.88,
          boundingBox: { x: 100, y: 150, width: 200, height: 35 },
          selector: 'input[name="email"]',
          attributes: { 'name': 'email', 'type': 'email', 'placeholder': 'Enter email' },
          text: '',
          color: '#ffffff',
          accessibility: {
            score: 92,
            issues: []
          }
        },
        {
          id: '3',
          type: 'Link',
          confidence: 0.91,
          boundingBox: { x: 100, y: 250, width: 80, height: 20 },
          selector: 'a[href="/login"]',
          attributes: { 'href': '/login', 'class': 'nav-link' },
          text: 'Login',
          color: '#1976d2',
          accessibility: {
            score: 78,
            issues: ['Low color contrast']
          }
        }
      ];
      
      setDetectedElements(mockElements);
    } catch (error) {
      console.error('Analysis failed:', error);
    } finally {
      setIsAnalyzing(false);
    }
  };

  const getElementTypeColor = (type: string) => {
    const colors: Record<string, string> = {
      'Button': '#4caf50',
      'Input': '#ff9800',
      'Link': '#2196f3',
      'Image': '#9c27b0',
      'Text': '#607d8b'
    };
    return colors[type] || '#757575';
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 0.9) return 'success';
    if (confidence >= 0.7) return 'warning';
    return 'error';
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <Visibility color="primary" />
        Computer Vision Analysis
      </Typography>
      
      <Typography variant="body1" color="text.secondary" paragraph>
        AI-powered computer vision for web element detection, analysis, and automation.
      </Typography>

      {/* Image Upload */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Upload Screenshot
          </Typography>
          
          <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', mb: 2 }}>
            <Button
              variant="contained"
              startIcon={<CameraAlt />}
              onClick={() => fileInputRef.current?.click()}
            >
              Take Screenshot
            </Button>
            
            <Button
              variant="outlined"
              startIcon={<Upload />}
              onClick={() => fileInputRef.current?.click()}
            >
              Upload Image
            </Button>
            
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              onChange={handleImageUpload}
              style={{ display: 'none' }}
            />
          </Box>
          
          {uploadedImage && (
            <Box sx={{ mb: 2 }}>
              <img
                src={uploadedImage}
                alt="Uploaded screenshot"
                style={{ maxWidth: '100%', maxHeight: '400px', border: '1px solid #ddd', borderRadius: '4px' }}
              />
            </Box>
          )}
          
          <Button
            variant="contained"
            startIcon={<AutoAwesome />}
            onClick={handleAnalyzeImage}
            disabled={!uploadedImage || isAnalyzing}
            size="large"
            fullWidth
          >
            {isAnalyzing ? 'Analyzing Image...' : 'Analyze with AI'}
          </Button>
          
          {isAnalyzing && (
            <Box sx={{ mt: 2 }}>
              <LinearProgress />
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                AI is analyzing the image and detecting web elements...
              </Typography>
            </Box>
          )}
        </CardContent>
      </Card>

      {/* Detected Elements */}
      {detectedElements.length > 0 && (
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              <Visibility /> Detected Elements ({detectedElements.length})
            </Typography>
            
            <Grid container spacing={2}>
              {detectedElements.map((element) => (
                <Grid item xs={12} md={6} lg={4} key={element.id}>
                  <Paper 
                    sx={{ 
                      p: 2, 
                      border: '1px solid', 
                      borderColor: 'divider',
                      cursor: 'pointer',
                      '&:hover': { borderColor: 'primary.main' }
                    }}
                    onClick={() => {
                      setSelectedElement(element);
                      setOpenDialog(true);
                    }}
                  >
                    <Stack direction="row" justifyContent="space-between" alignItems="center" mb={1}>
                      <Chip
                        label={element.type}
                        sx={{ backgroundColor: getElementTypeColor(element.type), color: 'white' }}
                        size="small"
                      />
                      <Chip
                        label={`${(element.confidence * 100).toFixed(0)}%`}
                        color={getConfidenceColor(element.confidence) as any}
                        size="small"
                      />
                    </Stack>
                    
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      {element.selector}
                    </Typography>
                    
                    {element.text && (
                      <Typography variant="body2" sx={{ fontStyle: 'italic' }}>
                        "{element.text}"
                      </Typography>
                    )}
                    
                    <Box sx={{ mt: 1 }}>
                      <Typography variant="caption" color="text.secondary">
                        Accessibility: {element.accessibility.score}/100
                      </Typography>
                      {element.accessibility.issues.length > 0 && (
                        <Chip
                          label={`${element.accessibility.issues.length} issues`}
                          color="warning"
                          size="small"
                          sx={{ ml: 1 }}
                        />
                      )}
                    </Box>
                  </Paper>
                </Grid>
              ))}
            </Grid>
          </CardContent>
        </Card>
      )}

      {/* Element Details Dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Visibility color="primary" />
            Element Details
          </Box>
        </DialogTitle>
        <DialogContent>
          {selectedElement && (
            <Box>
              <Grid container spacing={3}>
                <Grid item xs={12} md={6}>
                  <Typography variant="h6" gutterBottom>
                    Element Information
                  </Typography>
                  
                  <List dense>
                    <ListItem>
                      <ListItemIcon><Info /></ListItemIcon>
                      <ListItemText 
                        primary="Type" 
                        secondary={selectedElement.type}
                      />
                    </ListItem>
                    <ListItem>
                      <ListItemIcon><BugReport /></ListItemIcon>
                      <ListItemText 
                        primary="Selector" 
                        secondary={selectedElement.selector}
                      />
                    </ListItem>
                    <ListItem>
                      <ListItemIcon><CheckCircle /></ListItemIcon>
                      <ListItemText 
                        primary="Confidence" 
                        secondary={`${(selectedElement.confidence * 100).toFixed(1)}%`}
                      />
                    </ListItem>
                    {selectedElement.text && (
                      <ListItem>
                        <ListItemIcon><Info /></ListItemIcon>
                        <ListItemText 
                          primary="Text Content" 
                          secondary={selectedElement.text}
                        />
                      </ListItem>
                    )}
                  </List>
                </Grid>
                
                <Grid item xs={12} md={6}>
                  <Typography variant="h6" gutterBottom>
                    Accessibility Analysis
                  </Typography>
                  
                  <Paper sx={{ p: 2, mb: 2 }}>
                    <Typography variant="h4" color="primary" gutterBottom>
                      {selectedElement.accessibility.score}/100
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Accessibility Score
                    </Typography>
                  </Paper>
                  
                  {selectedElement.accessibility.issues.length > 0 ? (
                    <Alert severity="warning">
                      <Typography variant="subtitle2" gutterBottom>
                        Issues Found:
                      </Typography>
                      <List dense>
                        {selectedElement.accessibility.issues.map((issue, index) => (
                          <ListItem key={index}>
                            <ListItemIcon><Error color="warning" /></ListItemIcon>
                            <ListItemText primary={issue} />
                          </ListItem>
                        ))}
                      </List>
                    </Alert>
                  ) : (
                    <Alert severity="success">
                      No accessibility issues detected!
                    </Alert>
                  )}
                </Grid>
              </Grid>
              
              <Divider sx={{ my: 3 }} />
              
              <Typography variant="h6" gutterBottom>
                Element Attributes
              </Typography>
              
              <Paper sx={{ p: 2, backgroundColor: 'grey.50' }}>
                <pre style={{ margin: 0, fontSize: '0.875rem' }}>
                  {JSON.stringify(selectedElement.attributes, null, 2)}
                </pre>
              </Paper>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Close</Button>
          <Button variant="contained" startIcon={<Download />}>
            Export Element Data
          </Button>
        </DialogActions>
      </Dialog>

      {/* Floating Action Button */}
      <Fab
        color="primary"
        sx={{ position: 'fixed', bottom: 16, right: 16 }}
        onClick={() => fileInputRef.current?.click()}
      >
        <CameraAlt />
      </Fab>
    </Box>
  );
};

export default ComputerVision;
