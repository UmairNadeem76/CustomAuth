//AdminRoute.tsx
import React, { useContext, useEffect, useState } from 'react';
import { UserContext } from './UserContext';
import { Navigate } from 'react-router-dom';

interface AdminRouteProps {
    component: React.ComponentType;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ component: Component }) => {
    const { user, fetchUserData } = useContext(UserContext);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!user) {
            fetchUserData().then(() => setLoading(false));
        } else {
            setLoading(false);
        }
    }, [user]);

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!user || user.email !== 'admin@gmail.com') {
        return <Navigate to="/login" />;
    }

    return <Component />;
};

export default AdminRoute;
