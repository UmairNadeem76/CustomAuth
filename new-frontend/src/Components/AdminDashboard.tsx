// AdminDashboard.tsx

// This component renders the Admin Dashboard, displaying all users and their associated tasks.

import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './AdminDashboard.css';

// Define the User interface
interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
}

// Define the Task interface
interface Task {
    taskID: number;
    task_Name: string;
    task_Description: string;
    task_Status: string;
    task_Priority: number;
}

// Functional component for the Admin Dashboard
const AdminDashboard: React.FC = () => {
    // State to store the list of users
    const [users, setUsers] = useState<User[]>([]);
    // State to store the ID of the selected user
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
    // State to store the tasks of the selected user
    const [tasks, setTasks] = useState<Task[]>([]);

    // Fetch all users when the component mounts
    useEffect(() => {
        axios
            .get('http://localhost:5191/api/admin/users', { withCredentials: true })
            .then(response => {
                setUsers(response.data); // Update users state with fetched data
            })
            .catch(error => {
                console.error('Error fetching users:', error); // Log any errors
            });
    }, []);

    // Handle user selection and fetch their tasks
    const handleUserClick = (userId: number) => {
        setSelectedUserId(userId); // Set the selected user ID
        axios
            .get(`http://localhost:5191/api/admin/users/${userId}/tasks`, { withCredentials: true })
            .then(response => {
                setTasks(response.data); // Update tasks state with fetched data
            })
            .catch(error => {
                console.error('Error fetching user tasks:', error); // Log any errors
            });
    };

    return (
        <div className="admin-dashboard">
            <h1>Admin Dashboard</h1>
            <h2>Users</h2>
            {/* List of all users */}
            <ul className="user-list">
                {users.map(user => (
                    <li key={user.id}>
                        {/* Button to select a user and display their tasks */}
                        <button onClick={() => handleUserClick(user.id)}>
                            {user.firstName} {user.lastName} ({user.email})
                        </button>
                    </li>
                ))}
            </ul>

            {/* Display tasks if a user is selected */}
            {selectedUserId && (
                <div className="user-tasks">
                    <h2>Tasks for User {selectedUserId}</h2>
                    <ul className="task-list">
                        {tasks.map(task => (
                            <li key={task.taskID} className="task-item">
                                <h3>{task.task_Name}</h3>
                                <p>{task.task_Description}</p>
                                <p>Status: {task.task_Status}</p>
                                <p>Priority: {task.task_Priority}</p>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default AdminDashboard;
