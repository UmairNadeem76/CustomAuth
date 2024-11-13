// NewTask.tsx

// This component allows users to create a new task by providing a title, description, and status.
// Upon successful creation, it navigates the user back to the Task List page.

import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './NewTask.css';

// Define the Task interface to type the new task data
interface Task {
    title: string;
    description: string;
    status: 'Completed' | 'InProgress' | 'Pending';
}

const NewTask: React.FC = () => {
    // State variables to manage form inputs
    const [title, setTitle] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [status, setStatus] = useState<Task['status']>('Pending');
    const navigate = useNavigate();
    const [error, setError] = useState<string | null>(null); // State to handle and display errors

    // Handle form submission
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault(); // Prevent default form submission behavior

        const newTask: Task = { title, description, status }; // Create a new task object
        try {
            // Send POST request to add the new task
            const response = await axios.post('/api/tasks', newTask);

            if (response.status === 201) {
                // Navigate back to Task List on successful creation
                navigate('/tasks');
            } else {
                // Set error message if task creation fails
                setError('Failed to create task. Please try again.');
            }
        } catch (error) {
            console.error('Error creating task:', error);
            // Set a generic error message if an exception occurs
            setError('An error occurred. Please try again.');
        }
    };

    return (
        <div className="new-task-container">
            <h1>Create New Task</h1>
            <form onSubmit={handleSubmit} className="new-task-form">
                {/* Task Title Input */}
                <input
                    type="text"
                    placeholder="Task Title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    required
                />
                {/* Task Description Input */}
                <textarea
                    placeholder="Task Description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    required
                />
                {/* Task Status Selection */}
                <select value={status} onChange={(e) => setStatus(e.target.value as Task['status'])}>
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select>
                {/* Submit Button */}
                <button type="submit">Add Task</button>
                {/* Display error message if any */}
                {error && <p className="error-message">{error}</p>}
            </form>
        </div>
    );
};

export default NewTask;
