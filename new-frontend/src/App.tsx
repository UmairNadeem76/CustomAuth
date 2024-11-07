// App.tsx
import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import LoginSignup from './Components/LoginSignup';
import Dashboard from './Components/Dashboard';
import Navbar from './Components/Navbar';
import Tasks from './Components/Tasks';
import AdminDashboard from './Components/AdminDashboard';
import { UserProvider, UserContext } from './Components/UserContext';
import axios from 'axios';
import { useContext } from 'react';

// ProtectedRoute Component
const ProtectedRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { user, fetchUserData } = useContext(UserContext);
  const [loading, setLoading] = React.useState<boolean>(true);

  React.useEffect(() => {
    const verifyUser = async () => {
      if (!user) {
        await fetchUserData();
      }
      setLoading(false);
    };
    verifyUser();
  }, [user, fetchUserData]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (!user) {
    return <Navigate to="/" replace />;
  }

  return children;
};

// AdminRoute Component
const AdminRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { user } = useContext(UserContext);

  if (!user) {
    return <Navigate to="/" replace />;
  }

  if (user.email !== 'admin@gmail.com') {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};

// Unauthorized Component
const UnauthorizedComponent: React.FC = () => {
  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>Unauthorized Access</h1>
      <p>You do not have permission to view this page.</p>
    </div>
  );
};

const App: React.FC = () => {
  return (
    <UserProvider>
      <Router>
        <Navbar />
        <Routes>
          <Route path="/" element={<LoginSignup />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/tasks"
            element={
              <ProtectedRoute>
                <Tasks />
              </ProtectedRoute>
            }
          />
          <Route
            path="/admindashboard"
            element={
              <AdminRoute>
                <AdminDashboard />
              </AdminRoute>
            }
          />
          <Route path="/unauthorized" element={<UnauthorizedComponent />} />
          {/* Add other routes as needed */}
        </Routes>
      </Router>
    </UserProvider>
  );
};

export default App;
