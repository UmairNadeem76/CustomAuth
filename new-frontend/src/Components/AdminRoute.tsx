// AdminRoute.tsx

// This component is a protected route that only allows admin users to access certain components.
// If the user is not an admin, they are redirected to the login page.

import React, { useContext, useEffect, useState } from 'react';
import { UserContext } from './UserContext';
import { Navigate } from 'react-router-dom';

// Define the props for the AdminRoute component
interface AdminRouteProps {
    component: React.ComponentType; // The component to render if the user is an admin
}

// Functional component for the admin-protected route
const AdminRoute: React.FC<AdminRouteProps> = ({ component: Component }) => {
    // Extract user data and a function to fetch user data from the UserContext
    const { user, fetchUserData } = useContext(UserContext);
    // Local state to manage the loading status
    const [loading, setLoading] = useState(true);

    // useEffect hook to fetch user data when the component mounts or when 'user' changes
    useEffect(() => {
        if (!user) {
            // If user data is not available, fetch it and then stop loading
            fetchUserData().then(() => setLoading(false));
        } else {
            // If user data is already available, stop loading
            setLoading(false);
        }
    }, [user]);

    // If the data is still loading, display a loading message
    if (loading) {
        return <div>Loading...</div>;
    }

    // If the user is not logged in or not an admin, redirect them to the login page
    if (!user || user.email !== 'admin@gmail.com') {
        return <Navigate to="/login" />;
    }

    // If the user is an admin, render the specified component
    return <Component />;
};

export default AdminRoute;
