// App.tsx

// This is the main application component that sets up routing and context providers.
// It defines protected routes for authenticated users and admin users.

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
// This component ensures that only authenticated users can access certain routes.
// If the user is not authenticated, they are redirected to the login page.
const ProtectedRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { user, fetchUserData } = useContext(UserContext);
  const [loading, setLoading] = React.useState<boolean>(true);

  React.useEffect(() => {
    const verifyUser = async () => {
      if (!user) {
        await fetchUserData(); // Fetch user data if not already available
      }
      setLoading(false); // Set loading to false after verification
    };
    verifyUser();
  }, [user, fetchUserData]);

  if (loading) {
    // Display a loading message while verifying user
    return <div>Loading...</div>;
  }

  if (!user) {
    // Redirect to login if user is not authenticated
    return <Navigate to="/" replace />;
  }

  // Render the child component if user is authenticated
  return children;
};

// AdminRoute Component
// This component ensures that only admin users can access certain routes.
// If the user is not an admin, they are redirected to an unauthorized access page.
const AdminRoute: React.FC<{ children: JSX.Element }> = ({ children }) => {
  const { user } = useContext(UserContext);

  if (!user) {
    // Redirect to login if user is not authenticated
    return <Navigate to="/" replace />;
  }

  if (user.email !== 'admin@gmail.com') {
    // Redirect to unauthorized page if user is not an admin
    return <Navigate to="/unauthorized" replace />;
  }

  // Render the child component if user is an admin
  return children;
};

// Unauthorized Component
// This component displays an unauthorized access message.
const UnauthorizedComponent: React.FC = () => {
  return (
    <div style={{ textAlign: 'center', marginTop: '50px' }}>
      <h1>Unauthorized Access</h1>
      <p>You do not have permission to view this page.</p>
    </div>
  );
};

const App: React.FC = () => {
  // Configure Axios to include cookies with every request
  axios.defaults.withCredentials = true;

  return (
    // Wrap the application with UserProvider to provide user context
    <UserProvider>
      <Router>
        {/* Render the navigation bar */}
        <Navbar />
        {/* Define application routes */}
        <Routes>
          {/* Public Route: Login and Signup */}
          <Route path="/" element={<LoginSignup />} />

          {/* Protected Route: Dashboard */}
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />

          {/* Protected Route: Task List */}
          <Route
            path="/tasks"
            element={
              <ProtectedRoute>
                <Tasks />
              </ProtectedRoute>
            }
          />

          {/* Admin Protected Route: Admin Dashboard */}
          <Route
            path="/admindashboard"
            element={
              <AdminRoute>
                <AdminDashboard />
              </AdminRoute>
            }
          />

          {/* Route for Unauthorized Access */}
          <Route path="/unauthorized" element={<UnauthorizedComponent />} />

          {/* Add other routes as needed */}
        </Routes>
      </Router>
    </UserProvider>
  );
};

export default App;
