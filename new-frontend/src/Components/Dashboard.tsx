import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './Dashboard.css';

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

const Dashboard: React.FC = () => {
    const [taskCount, setTaskCount] = useState<TaskCount>({ completed: 0, inProgress: 0, pending: 0 });
    const [user, setUser] = useState<User | null>(null);
    const [error, setError] = useState<string | null>(null);

    // Configure Axios to include cookies with requests
    axios.defaults.withCredentials = true;

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const response = await axios.get<UserData>('http://localhost:5191/api/account/userdata');
                const userData = response.data;
                setUser({
                    role: 'user', // Adjust this based on your application's logic
                    name: `${userData.firstName} ${userData.lastName}`,
                });
            } catch (error: any) {
                if (error.response && error.response.status === 401) {
                    setError('Unauthorized');
                } else {
                    setError('Error Fetching User Data.');
                }
                console.error('Error Fetching User Data:', error);
            }
        };

        const fetchTaskData = async () => {
            try {
                const response = await axios.get<Task[]>('http://localhost:5191/api/task/usertasks');
                const tasks = response.data;
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
                    { completed: 0, inProgress: 0, pending: 0 }
                );
                setTaskCount(taskCounts);
            } catch (error: any) {
                if (error.response && error.response.status === 401) {
                    setError('Unauthorized');
                } else {
                    setError('Error fetching task data.');
                }
                console.error('Error fetching task data:', error);
            }
        };

        fetchUserData();
        fetchTaskData();
    }, []);

    return (
        <div className="dashboard-container">
            {error ? (
                <div className="error-message">{error}</div>
            ) : (
                <>
                    <h1>Welcome, {user?.name}</h1>
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
