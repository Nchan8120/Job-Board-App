import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import JobListingsPage from './pages/JobListingsPage';
import JobDetailPage from './pages/JobDetailPage';
import CreateJobPage from './pages/CreateJobPage';
import EditJobPage from './pages/EditJobPage';
import MyJobsPage from './pages/MyJobsPage';
import Navbar from './components/Navbar';

function PosterRoute({ children }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" />;
  if (user.role !== 'Poster') return <Navigate to="/" />;
  return children;
}


function AppRoutes() {
  return (
    <>
      <Navbar />
      <div className="container">
        <Routes>
          <Route path="/" element={<JobListingsPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/jobs/:id" element={<JobDetailPage />} />
          <Route path="/jobs/create" element={
            <PosterRoute><CreateJobPage /></PosterRoute>
          } />
          <Route path="/jobs/:id/edit" element={
            <PosterRoute><EditJobPage /></PosterRoute>
          } />
          <Route path="/my-jobs" element={
            <PosterRoute><MyJobsPage /></PosterRoute>
          } />
        </Routes>
      </div>
    </>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </AuthProvider>
  );
}