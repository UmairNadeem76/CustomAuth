// Navbar.tsx
import React, { useContext, useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import './Navbar.css';
import { UserContext } from './UserContext';

const Navbar: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { user, logout } = useContext(UserContext);

    // State to track the active button
    const [activeButton, setActiveButton] = useState<string>('');

    // Update activeButton based on the current location
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

    const handleNavigate = (path: string, buttonName: string) => {
        navigate(path);
        setActiveButton(buttonName);
    };

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
                    {user.email === 'admin@gmail.com' && (
                        <button
                            onClick={() => handleNavigate('/admindashboard', 'admindashboard')}
                            className={activeButton === 'admindashboard' ? 'active' : ''}
                        >
                            Admin Dashboard
                        </button>
                    )}
                    <button
                        onClick={() => handleNavigate('/dashboard', 'dashboard')}
                        className={activeButton === 'dashboard' ? 'active' : ''}
                    >
                        Dashboard
                    </button>
                    <button
                        onClick={() => handleNavigate('/tasks', 'tasks')}
                        className={activeButton === 'tasks' ? 'active' : ''}
                    >
                        Task List
                    </button>
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
                    <button
                        onClick={() => handleNavigate('/', 'login')}
                        className={activeButton === 'login' ? 'active' : ''}
                    >
                        Login
                    </button>
                </div>
            )}
            <div className="navbar-title">Task Management System</div>
        </nav>
    );
};

export default Navbar;
