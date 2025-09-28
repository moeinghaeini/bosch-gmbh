import React, { Component, ErrorInfo, ReactNode } from 'react';
import {
  Box,
  Typography,
  Button,
  Paper,
  Stack,
  Alert,
  AlertTitle,
  IconButton,
  Collapse,
  Chip,
  Divider
} from '@mui/material';
import {
  Error as ErrorIcon,
  Refresh as RefreshIcon,
  BugReport as BugReportIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon,
  Home as HomeIcon,
  Report as ReportIcon
} from '@mui/icons-material';

interface Props {
  children: ReactNode;
  fallback?: ReactNode;
  onError?: (error: Error, errorInfo: ErrorInfo) => void;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
  expanded: boolean;
  retryCount: number;
}

class ErrorBoundary extends Component<Props, State> {
  private maxRetries = 3;

  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
      expanded: false,
      retryCount: 0
    };
  }

  static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
      errorInfo: null,
      expanded: false,
      retryCount: 0
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    this.setState({
      error,
      errorInfo,
      hasError: true
    });

    // Log error to console in development
    if (process.env.NODE_ENV === 'development') {
      console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    // Call custom error handler if provided
    if (this.props.onError) {
      this.props.onError(error, errorInfo);
    }

    // In production, you might want to send this to an error reporting service
    // Example: Sentry.captureException(error, { extra: errorInfo });
  }

  handleRetry = () => {
    if (this.state.retryCount < this.maxRetries) {
      this.setState(prevState => ({
        hasError: false,
        error: null,
        errorInfo: null,
        retryCount: prevState.retryCount + 1
      }));
    }
  };

  handleReset = () => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
      retryCount: 0
    });
  };

  handleGoHome = () => {
    window.location.href = '/';
  };

  toggleExpanded = () => {
    this.setState(prevState => ({
      expanded: !prevState.expanded
    }));
  };

  render() {
    if (this.state.hasError) {
      // Custom fallback UI
      if (this.props.fallback) {
        return this.props.fallback;
      }

      const { error, errorInfo, retryCount, expanded } = this.state;
      const canRetry = retryCount < this.maxRetries;

      return (
        <Box
          sx={{
            minHeight: '100vh',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            p: 3,
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
          }}
        >
          <Paper
            elevation={24}
            sx={{
              maxWidth: 600,
              width: '100%',
              p: 4,
              borderRadius: 3,
              background: 'rgba(255, 255, 255, 0.95)',
              backdropFilter: 'blur(10px)',
              border: '1px solid rgba(255, 255, 255, 0.2)'
            }}
          >
            <Stack spacing={3}>
              {/* Error Header */}
              <Box textAlign="center">
                <Box
                  sx={{
                    display: 'inline-flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    width: 80,
                    height: 80,
                    borderRadius: '50%',
                    background: 'linear-gradient(135deg, #ff6b6b, #ee5a24)',
                    mb: 2,
                    animation: 'pulse 2s ease-in-out infinite'
                  }}
                >
                  <ErrorIcon sx={{ fontSize: 40, color: 'white' }} />
                </Box>
                <Typography variant="h4" component="h1" sx={{ fontWeight: 700, mb: 1 }}>
                  Oops! Something went wrong
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  The Industrial Automation Platform encountered an unexpected error.
                </Typography>
              </Box>

              {/* Error Details */}
              <Alert 
                severity="error" 
                sx={{ 
                  borderRadius: 2,
                  '& .MuiAlert-message': { width: '100%' }
                }}
              >
                <AlertTitle sx={{ fontWeight: 600 }}>
                  System Error Detected
                </AlertTitle>
                <Typography variant="body2" sx={{ mt: 1 }}>
                  {error?.message || 'An unknown error occurred'}
                </Typography>
                
                {/* Error stack trace (expandable) */}
                {error?.stack && (
                  <Box sx={{ mt: 2 }}>
                    <Button
                      size="small"
                      startIcon={expanded ? <ExpandLessIcon /> : <ExpandMoreIcon />}
                      onClick={this.toggleExpanded}
                      sx={{ mb: 1 }}
                    >
                      {expanded ? 'Hide' : 'Show'} Technical Details
                    </Button>
                    <Collapse in={expanded}>
                      <Paper
                        variant="outlined"
                        sx={{ 
                          p: 2, 
                          mt: 1, 
                          backgroundColor: 'rgba(0,0,0,0.05)',
                          fontFamily: 'monospace',
                          fontSize: '0.75rem',
                          overflow: 'auto',
                          maxHeight: 200
                        }}
                      >
                        <Typography variant="caption" component="pre" sx={{ whiteSpace: 'pre-wrap' }}>
                          {error.stack}
                        </Typography>
                        {errorInfo?.componentStack && (
                          <>
                            <Divider sx={{ my: 1 }} />
                            <Typography variant="caption" component="pre" sx={{ whiteSpace: 'pre-wrap' }}>
                              {errorInfo.componentStack}
                            </Typography>
                          </>
                        )}
                      </Paper>
                    </Collapse>
                  </Box>
                )}
              </Alert>

              {/* Status Information */}
              <Box>
                <Stack direction="row" spacing={1} flexWrap="wrap" gap={1}>
                  <Chip 
                    icon={<BugReportIcon />} 
                    label={`Retry ${retryCount}/${this.maxRetries}`} 
                    color={canRetry ? 'primary' : 'default'}
                    variant="outlined"
                  />
                  <Chip 
                    label={process.env.NODE_ENV === 'development' ? 'Development Mode' : 'Production Mode'} 
                    color={process.env.NODE_ENV === 'development' ? 'warning' : 'success'}
                    variant="outlined"
                  />
                  <Chip 
                    label={`Error ID: ${Date.now()}`} 
                    color="info" 
                    variant="outlined"
                  />
                </Stack>
              </Box>

              {/* Action Buttons */}
              <Stack direction="row" spacing={2} justifyContent="center" flexWrap="wrap">
                {canRetry && (
                  <Button
                    variant="contained"
                    startIcon={<RefreshIcon />}
                    onClick={this.handleRetry}
                    sx={{
                      background: 'linear-gradient(45deg, #1976d2, #42a5f5)',
                      '&:hover': {
                        background: 'linear-gradient(45deg, #1565c0, #1976d2)',
                        transform: 'translateY(-1px)'
                      }
                    }}
                  >
                    Try Again
                  </Button>
                )}
                
                <Button
                  variant="outlined"
                  startIcon={<HomeIcon />}
                  onClick={this.handleGoHome}
                  sx={{
                    borderColor: 'primary.main',
                    color: 'primary.main',
                    '&:hover': {
                      borderColor: 'primary.dark',
                      backgroundColor: 'primary.light',
                      color: 'white'
                    }
                  }}
                >
                  Go to Dashboard
                </Button>

                <Button
                  variant="text"
                  startIcon={<ReportIcon />}
                  onClick={() => {
                    // In a real application, you might want to open a support ticket
                    // or send an email to support
                    const subject = encodeURIComponent('Industrial Automation Platform - Error Report');
                    const body = encodeURIComponent(`
Error Details:
- Message: ${error?.message}
- Stack: ${error?.stack}
- Component Stack: ${errorInfo?.componentStack}
- Retry Count: ${retryCount}
- Timestamp: ${new Date().toISOString()}
                    `);
                    window.open(`mailto:support@bosch.com?subject=${subject}&body=${body}`);
                  }}
                  sx={{ color: 'text.secondary' }}
                >
                  Report Issue
                </Button>
              </Stack>

              {/* Help Text */}
              <Box textAlign="center">
                <Typography variant="caption" color="text.secondary">
                  If this problem persists, please contact the system administrator or check the system logs.
                </Typography>
              </Box>
            </Stack>
          </Paper>
        </Box>
      );
    }

    return this.props.children;
  }
}

// Higher-order component for easier usage
export const withErrorBoundary = <P extends object>(
  Component: React.ComponentType<P>,
  errorBoundaryProps?: Omit<Props, 'children'>
) => {
  const WrappedComponent = (props: P) => (
    <ErrorBoundary {...errorBoundaryProps}>
      <Component {...props} />
    </ErrorBoundary>
  );

  WrappedComponent.displayName = `withErrorBoundary(${Component.displayName || Component.name})`;
  return WrappedComponent;
};

// Hook for error boundary functionality
export const useErrorHandler = () => {
  const handleError = (error: Error, errorInfo?: ErrorInfo) => {
    console.error('Error caught by useErrorHandler:', error, errorInfo);
    
    // You can add custom error handling logic here
    // For example, sending to an error reporting service
  };

  return { handleError };
};

export default ErrorBoundary;
