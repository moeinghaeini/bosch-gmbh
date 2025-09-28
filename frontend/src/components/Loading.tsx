import React from 'react';
import { 
  Box, 
  CircularProgress, 
  Typography, 
  Fade, 
  Zoom,
  Stack,
  Skeleton,
  LinearProgress
} from '@mui/material';
import { 
  AutoAwesome, 
  Psychology, 
  Web, 
  Settings,
  TrendingUp,
  Security
} from '@mui/icons-material';

interface LoadingProps {
  message?: string;
  type?: 'default' | 'ai' | 'automation' | 'system';
  progress?: number;
  showTips?: boolean;
}

const Loading: React.FC<LoadingProps> = ({ 
  message = "Loading...", 
  type = 'default',
  progress,
  showTips = true 
}) => {
  const getLoadingIcon = () => {
    switch (type) {
      case 'ai':
        return <Psychology sx={{ fontSize: 40, color: 'secondary.main' }} />;
      case 'automation':
        return <Web sx={{ fontSize: 40, color: 'warning.main' }} />;
      case 'system':
        return <Settings sx={{ fontSize: 40, color: 'info.main' }} />;
      default:
        return <AutoAwesome sx={{ fontSize: 40, color: 'primary.main' }} />;
    }
  };

  const getLoadingMessage = () => {
    switch (type) {
      case 'ai':
        return "AI is analyzing data and generating insights...";
      case 'automation':
        return "Automation engine is processing your request...";
      case 'system':
        return "System is initializing components...";
      default:
        return message;
    }
  };

  const tips = [
    "💡 AI can analyze test results and predict failures",
    "🔧 Web automation works with natural language commands",
    "📊 Real-time monitoring provides instant system insights",
    "🚀 All systems are optimized for industrial performance",
    "🔒 Enterprise-grade security protects your data",
    "⚡ Advanced caching ensures lightning-fast responses"
  ];

  const randomTip = tips[Math.floor(Math.random() * tips.length)];

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '400px',
        p: 4,
        background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
        borderRadius: 3,
        position: 'relative',
        overflow: 'hidden'
      }}
    >
      {/* Animated background elements */}
      <Box
        sx={{
          position: 'absolute',
          top: -50,
          right: -50,
          width: 100,
          height: 100,
          borderRadius: '50%',
          background: 'linear-gradient(45deg, rgba(25, 118, 210, 0.1), rgba(156, 39, 176, 0.1))',
          animation: 'float 6s ease-in-out infinite',
          '@keyframes float': {
            '0%, 100%': { transform: 'translateY(0px) rotate(0deg)' },
            '50%': { transform: 'translateY(-20px) rotate(180deg)' }
          }
        }}
      />
      <Box
        sx={{
          position: 'absolute',
          bottom: -30,
          left: -30,
          width: 60,
          height: 60,
          borderRadius: '50%',
          background: 'linear-gradient(45deg, rgba(76, 175, 80, 0.1), rgba(255, 152, 0, 0.1))',
          animation: 'float 4s ease-in-out infinite reverse',
        }}
      />

      <Fade in timeout={500}>
        <Stack spacing={3} alignItems="center">
          {/* Main loading icon */}
          <Zoom in timeout={800}>
            <Box
              sx={{
                position: 'relative',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                width: 80,
                height: 80,
                borderRadius: '50%',
                background: 'linear-gradient(135deg, rgba(255,255,255,0.9), rgba(255,255,255,0.7))',
                boxShadow: '0 8px 32px rgba(0,0,0,0.1)',
                backdropFilter: 'blur(10px)',
                border: '1px solid rgba(255,255,255,0.2)'
              }}
            >
              {getLoadingIcon()}
              <CircularProgress
                size={100}
                thickness={2}
                sx={{
                  position: 'absolute',
                  color: type === 'ai' ? 'secondary.main' : 
                        type === 'automation' ? 'warning.main' : 
                        type === 'system' ? 'info.main' : 'primary.main',
                  animation: 'spin 2s linear infinite',
                  '@keyframes spin': {
                    '0%': { transform: 'rotate(0deg)' },
                    '100%': { transform: 'rotate(360deg)' }
                  }
                }}
              />
            </Box>
          </Zoom>

          {/* Loading message */}
          <Fade in timeout={1000}>
            <Typography 
              variant="h6" 
              sx={{ 
                fontWeight: 600, 
                textAlign: 'center',
                color: 'text.primary',
                maxWidth: 400
              }}
            >
              {getLoadingMessage()}
            </Typography>
          </Fade>

          {/* Progress bar if progress is provided */}
          {progress !== undefined && (
            <Fade in timeout={1200}>
              <Box sx={{ width: '100%', maxWidth: 300 }}>
                <LinearProgress 
                  variant="determinate" 
                  value={progress} 
                  sx={{ 
                    height: 8, 
                    borderRadius: 4,
                    backgroundColor: 'rgba(0,0,0,0.1)',
                    '& .MuiLinearProgress-bar': {
                      borderRadius: 4,
                      background: type === 'ai' ? 'linear-gradient(90deg, #9c27b0, #e91e63)' :
                                  type === 'automation' ? 'linear-gradient(90deg, #ff9800, #ff5722)' :
                                  type === 'system' ? 'linear-gradient(90deg, #03a9f4, #2196f3)' :
                                  'linear-gradient(90deg, #1976d2, #42a5f5)'
                    }
                  }}
                />
                <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block', textAlign: 'center' }}>
                  {progress}% complete
                </Typography>
              </Box>
            </Fade>
          )}

          {/* Loading tips */}
          {showTips && (
            <Fade in timeout={1500}>
              <Box
                sx={{
                  p: 2,
                  borderRadius: 2,
                  background: 'rgba(255,255,255,0.8)',
                  backdropFilter: 'blur(10px)',
                  border: '1px solid rgba(255,255,255,0.2)',
                  maxWidth: 400,
                  textAlign: 'center'
                }}
              >
                <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                  {randomTip}
                </Typography>
              </Box>
            </Fade>
          )}

          {/* Loading dots animation */}
          <Fade in timeout={2000}>
            <Stack direction="row" spacing={1}>
              {[0, 1, 2].map((index) => (
                <Box
                  key={index}
                  sx={{
                    width: 8,
                    height: 8,
                    borderRadius: '50%',
                    backgroundColor: type === 'ai' ? 'secondary.main' : 
                                   type === 'automation' ? 'warning.main' : 
                                   type === 'system' ? 'info.main' : 'primary.main',
                    animation: `pulse 1.5s ease-in-out infinite ${index * 0.2}s`,
                    '@keyframes pulse': {
                      '0%, 100%': { opacity: 0.3, transform: 'scale(1)' },
                      '50%': { opacity: 1, transform: 'scale(1.2)' }
                    }
                  }}
                />
              ))}
            </Stack>
          </Fade>
        </Stack>
      </Fade>
    </Box>
  );
};

// Skeleton loading component for content
export const SkeletonLoading: React.FC<{ lines?: number }> = ({ lines = 3 }) => (
  <Box sx={{ p: 2 }}>
    {Array.from({ length: lines }).map((_, index) => (
      <Skeleton
        key={index}
        variant="rectangular"
        height={60}
        sx={{ 
          mb: 2, 
          borderRadius: 2,
          animation: 'pulse 2s ease-in-out infinite',
          animationDelay: `${index * 0.1}s`
        }}
      />
    ))}
  </Box>
);

// Enhanced loading states for different components
export const CardSkeleton: React.FC = () => (
  <Box sx={{ p: 3 }}>
    <Stack spacing={2}>
      <Skeleton variant="circular" width={40} height={40} />
      <Skeleton variant="text" width="60%" height={32} />
      <Skeleton variant="text" width="40%" height={24} />
      <Skeleton variant="rectangular" height={8} sx={{ borderRadius: 1 }} />
    </Stack>
  </Box>
);

export const TableSkeleton: React.FC<{ rows?: number; columns?: number }> = ({ 
  rows = 5, 
  columns = 4 
}) => (
  <Box sx={{ p: 2 }}>
    <Stack spacing={1}>
      {/* Header skeleton */}
      <Stack direction="row" spacing={2} sx={{ mb: 2 }}>
        {Array.from({ length: columns }).map((_, index) => (
          <Skeleton key={index} variant="text" width="100%" height={40} />
        ))}
      </Stack>
      {/* Rows skeleton */}
      {Array.from({ length: rows }).map((_, rowIndex) => (
        <Stack key={rowIndex} direction="row" spacing={2} sx={{ mb: 1 }}>
          {Array.from({ length: columns }).map((_, colIndex) => (
            <Skeleton key={colIndex} variant="text" width="100%" height={32} />
          ))}
        </Stack>
      ))}
    </Stack>
  </Box>
);

export default Loading;
