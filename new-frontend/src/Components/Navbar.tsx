// Navbar.tsx

// This component renders the navigation bar of the application.
// It displays different navigation options based on the user's authentication status and role.

import React, { useContext, useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './Navbar.css';
import { UserContext } from './UserContext';

const Navbar: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { user, logout } = useContext(UserContext);

    // State to track the active navigation button
    const [activeButton, setActiveButton] = useState<string>('');

    // Update activeButton based on the current URL path
    useEffect(() => {
        if (location.pathname === '/dashboard') {
            setActiveButton('dashboard');
        } else if (location.pathname === '/tasks') {
            setActiveButton('tasks');
        } else if (location.pathname === '/admindashboard') {
            setActiveButton('admindashboard');
        } else if (location.pathname === '/') {
            setActiveButton('login');
        } else {
            setActiveButton('');
        }
    }, [location.pathname]);

    // Handle navigation to different routes
    const handleNavigate = (path: string, buttonName: string) => {
        navigate(path);
        setActiveButton(buttonName);
    };

    // Handle user logout
    const handleLogout = async () => {
        try {
            await logout(); // Call the logout function from UserContext
            // After logout, the UserContext will set user to null and reload the page
        } catch (error) {
            console.error('Error during logout:', error);
            alert('Failed to logout. Please try again.');
        }
    };

    return (
        <nav className="navbar">
            {user && (
                <div className="nav-links">
                    {/* Show Admin Dashboard link if the user is an admin */}
                    {user.email === 'admin@gmail.com' && (
                        <button
                            onClick={() => handleNavigate('/admindashboard', 'admindashboard')}
                            className={activeButton === 'admindashboard' ? 'active' : ''}
                        >
                            Admin Dashboard
                        </button>
                    )}
                    {/* Dashboard Link */}
                    <button
                        onClick={() => handleNavigate('/dashboard', 'dashboard')}
                        className={activeButton === 'dashboard' ? 'active' : ''}
                    >
                        Dashboard
                    </button>
                    {/* Task List Link */}
                    <button
                        onClick={() => handleNavigate('/tasks', 'tasks')}
                        className={activeButton === 'tasks' ? 'active' : ''}
                    >
                        Task List
                    </button>
                    {/* Logout Button */}
                    <button
                        onClick={handleLogout}
                        className={activeButton === 'logout' ? 'active' : ''}
                    >
                        Logout
                    </button>
                </div>
            )}
            {!user && (
                <div className="nav-links">
                    {/* Login Link for unauthenticated users */}
                    <button
                        onClick={() => handleNavigate('/', 'login')}
                        className={activeButton === 'login' ? 'active' : ''}
                    >
                        Login
                    </button>
                </div>
            )}
            {/* Application Title */}
            <div className="navbar-title">Task Management System</div>
        </nav>
    );
};

export default Navbar;
