//AdminDashboard.tsx
import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './AdminDashboard.css';

interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
}

interface Task {
    taskID: number;
    task_Name: string;
    task_Description: string;
    task_Status: string;
    task_Priority: number;
}

const AdminDashboard: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
    const [tasks, setTasks] = useState<Task[]>([]);

    useEffect(() => {
        // Fetch all users
        axios.get('http://localhost:5191/api/admin/users', { withCredentials: true })
            .then(response => {
                setUsers(response.data);
            })
            .catch(error => {
                console.error('Error fetching users:', error);
            });
    }, []);

    const handleUserClick = (userId: number) => {
        setSelectedUserId(userId);
        // Fetch tasks for the selected user
        axios.get(`http://localhost:5191/api/admin/users/${userId}/tasks`, { withCredentials: true })
            .then(response => {
                setTasks(response.data);
            })
            .catch(error => {
                console.error('Error fetching user tasks:', error);
            });
    };

    return (
        <div className="admin-dashboard">
            <h1>Admin Dashboard</h1>
            <h2>Users</h2>
            <ul className="user-list">
                {users.map(user => (
                    <li key={user.id}>
                        <button onClick={() => handleUserClick(user.id)}>
                            {user.firstName} {user.lastName} ({user.email})
                        </button>
                    </li>
                ))}
            </ul>

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
