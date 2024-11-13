// Dashboard.tsx

// This component renders the user's dashboard, displaying a summary of their tasks and user information.

import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './Dashboard.css';

// Interfaces to define the structure of task counts, user data, and tasks
interface TaskCount {
    completed: number;
    inProgress: number;
    pending: number;
}

interface User {
    role: 'admin' | 'user';
    name: string;
}

interface UserData {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
}

interface Task {
    taskID: number;
    userID: number;
    task_Name: string;
    task_Description: string;
    task_Status: 'Pending' | 'In Progress' | 'Completed';
    task_Priority: number;
}

// Functional component for the Dashboard
const Dashboard: React.FC = () => {
    // State to store task counts based on their status
    const [taskCount, setTaskCount] = useState<TaskCount>({ completed: 0, inProgress: 0, pending: 0 });
    // State to store user information
    const [user, setUser] = useState<User | null>(null);
    // State to handle and display errors
    const [error, setError] = useState<string | null>(null);

    // Configure Axios to include cookies with every request
    axios.defaults.withCredentials = true;

    // useEffect hook to fetch user data and task data when the component mounts
    useEffect(() => {
        // Function to fetch user data from the server
        const fetchUserData = async () => {
            try {
                const response = await axios.get<UserData>('http://localhost:5191/api/account/userdata');
                const userData = response.data;
                // Update the user state with fetched data
                setUser({
                    role: 'user', // Adjust this based on your application's logic
                    name: `${userData.firstName} ${userData.lastName}`,
                });
            } catch (error: any) {
                // Handle errors during user data fetching
                if (error.response && error.response.status === 401) {
                    setError('Unauthorized');
                } else {
                    setError('Error Fetching User Data.');
                }
                console.error('Error Fetching User Data:', error);
            }
        };

        // Function to fetch task data from the server
        const fetchTaskData = async () => {
            try {
                const response = await axios.get<Task[]>('http://localhost:5191/api/task/usertasks');
                const tasks = response.data;
                // Calculate the counts of tasks based on their status
                const taskCounts = tasks.reduce<TaskCount>(
                    (counts, task) => {
                        switch (task.task_Status) {
                            case 'Completed':
                                counts.completed += 1;
                                break;
                            case 'In Progress':
                                counts.inProgress += 1;
                                break;
                            case 'Pending':
                                counts.pending += 1;
                                break;
                        }
                        return counts;
                    },
                    { completed: 0, inProgress: 0, pending: 0 } // Initial counts
                );
                // Update the taskCount state with calculated counts
                setTaskCount(taskCounts);
            } catch (error: any) {
                // Handle errors during task data fetching
                if (error.response && error.response.status === 401) {
                    setError('Unauthorized');
                } else {
                    setError('Error fetching task data.');
                }
                console.error('Error fetching task data:', error);
            }
        };

        // Call the functions to fetch user data and task data
        fetchUserData();
        fetchTaskData();
    }, []);

    return (
        <div className="dashboard-container">
            {error ? (
                // Display an error message if there is an error
                <div className="error-message">{error}</div>
            ) : (
                <>
                    {/* Display a welcome message with the user's name */}
                    <h1>Welcome, {user?.name}</h1>
                    {/* Display a summary of tasks */}
                    <div className="task-summary">
                        <div className="task-card">
                            <h2>Completed Tasks</h2>
                            <p>{taskCount.completed}</p>
                        </div>
                        <div className="task-card">
                            <h2>In Progress Tasks</h2>
                            <p>{taskCount.inProgress}</p>
                        </div>
                        <div className="task-card">
                            <h2>Pending Tasks</h2>
                            <p>{taskCount.pending}</p>
                        </div>
                    </div>
                    {/* If the user is an admin, display the admin panel */}
                    {user?.role === 'admin' && (
                        <div className="admin-panel">
                            <h3>Admin Panel</h3>
                            <p>You Have Access To All Users' Tasks And Settings.</p>
                        </div>
                    )}
                </>
            )}
        </div>
    );
};

export default Dashboard;
