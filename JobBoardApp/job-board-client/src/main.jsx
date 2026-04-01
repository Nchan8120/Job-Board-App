import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';

// Clear auth state on app launch so users always start logged out
localStorage.removeItem('token');
localStorage.removeItem('username');
localStorage.removeItem('role');
localStorage.removeItem('id');

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(<App />);