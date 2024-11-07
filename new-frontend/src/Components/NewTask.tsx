import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './NewTask.css';

interface Task {
    title: string;
    description: string;
    status: 'Completed' | 'InProgress' | 'Pending';
}

const NewTask: React.FC = () => {
    const [title, setTitle] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [status, setStatus] = useState<Task['status']>('Pending');
    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null); // For error handling

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const newTask: Task = { title, description, status };
        try {
            // Send POST request to add the new task
            const response = await axios.post('/api/tasks', newTask);

            if (response.status === 201) {
                // Navigate back to Task List on successful creation
                navigate('/tasks');
            } else {
                setError('Failed to create task. Please try again.');
            }
        } catch (error) {
            console.error('Error creating task:', error);
            setError('An error occurred. Please try again.');
        }
    };

    return (
        <div className="new-task-container">
            <h1>Create New Task</h1>
            <form onSubmit={handleSubmit} className="new-task-form">
                <input
                    type="text"
                    placeholder="Task Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    required
                />
                <textarea
                    placeholder="Task Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                />
                <select value={status} onChange={(e) => setStatus(e.target.value as Task['status'])}>
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select>
                <button type="submit">Add Task</button>
                {error && <p className="error-message">{error}</p>}
            </form>
        </div>
    );
};

export default NewTask;
