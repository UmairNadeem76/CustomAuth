// TaskList.tsx

// This component manages a list of tasks with functionality to add, edit, delete, and mark tasks as completed.
// It uses local state to manage tasks and provides a form for task creation and editing.

import React, { useState } from 'react';
import './TaskList.css';

// Define the Task interface to type the task data
interface Task {
    id: number;
    title: string;
    description: string;
    status: 'Completed' | 'InProgress' | 'Pending';
    priority: number;
}

const TaskList: React.FC = () => {
    // State to store the list of tasks
    const [tasks, setTasks] = useState<Task[]>([]);
    // State to store new task data during creation or editing
    const [newTask, setNewTask] = useState<Partial<Task>>({});
    // State to track if a task is being edited (stores the task id)
    const [isEditing, setIsEditing] = useState<number | null>(null);
    // State to handle and display errors
    const [error, setError] = useState<string | null>(null);

    // Handle input changes for the task form
    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) => {
        const { name, value } = e.target;
        // Update newTask state with the input values
        setNewTask({ ...newTask, [name]: name === 'priority' ? parseInt(value) : value });
        setError(null); // Reset error message
    };

    // Validate the task form before submission
    const validateForm = () => {
        if (!newTask.title || !newTask.description || !newTask.status || !newTask.priority) {
            setError('All Fields Are Required!');
            return false;
        }
        return true;
    };

    // Add a new task to the task list
    const addTask = () => {
        if (!validateForm()) return;
        const task: Task = {
            id: Date.now(), // Use current timestamp as unique ID
            title: newTask.title || '',
            description: newTask.description || '',
            status: newTask.status as Task['status'],
            priority: newTask.priority || 1,
        };
        // Update the tasks state with the new task and sort them by priority
        setTasks(prevTasks => sortTasks([task, ...prevTasks]));
        setNewTask({}); // Reset the newTask state
    };

    // Update an existing task in the task list
    const updateTask = () => {
        if (!validateForm()) return;
        // Update the task in the tasks array
        setTasks(prevTasks =>
            sortTasks(prevTasks.map(task => (task.id === isEditing ? { ...task, ...newTask } : task)))
        );
        setNewTask({}); // Reset the newTask state
        setIsEditing(null); // Exit editing mode
    };

    // Delete a task from the task list
    const deleteTask = (id: number) => {
        setTasks(tasks.filter(task => task.id !== id));
    };

    // Mark a task as completed and remove it from the task list
    const markAsCompleted = (id: number) => {
        setTasks(tasks.filter(task => task.id !== id));
    };

    // Start editing a task by setting the isEditing state and pre-filling the form
    const startEditing = (id: number) => {
        const taskToEdit = tasks.find(task => task.id === id);
        if (taskToEdit) {
            setNewTask(taskToEdit);
            setIsEditing(id);
            setError(null); // Reset error message
        }
    };

    // Sort tasks based on their priority (lower priority number means higher importance)
    const sortTasks = (taskList: Task[]) => {
        return taskList.sort((a, b) => a.priority - b.priority);
    };

    return (
        <div className="task-list-container">
            <h1>Task List</h1>

            {/* Form for adding a new task or editing an existing one */}
            <div className="new-task-form">
                <input
                    name="title"
                    type="text"
                    placeholder="Task Title"
                    value={newTask.title || ''}
                    onChange={handleChange}
                    required
                />
                <textarea
                    name="description"
                    placeholder="Task Description"
                    value={newTask.description || ''}
                    onChange={handleChange}
                    required
                />
                <select
                    name="status"
                    value={newTask.status || ''}
                    onChange={handleChange}
                    required
                >
                    <option value="" disabled>Select Status</option>
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select>
                <input
                    name="priority"
                    type="number"
                    placeholder="Priority (1 = Most Important)"
                    value={newTask.priority || ''}
                    onChange={handleChange}
                    required
                    min="1"
                />

                {/* Display error message if any */}
                {error && <p className="error-message">{error}</p>}

                {/* Conditional rendering of Add or Update button based on editing state */}
                {isEditing ? (
                    <button onClick={updateTask}>Update Task</button>
                ) : (
                    <button onClick={addTask}>Add Task</button>
                )}
            </div>

            {/* Display the list of tasks */}
            <div className="tasks-grid">
                {tasks.map(task => (
                    <div className="task-card" key={task.id}>
                        <h2>{task.title}</h2>
                        <p>{task.description}</p>
                        <p>Priority: {task.priority}</p>
                        <span className={`status ${task.status.toLowerCase()}`}>{task.status}</span>
                        <div className="task-actions">
                            {/* Edit, Delete, and Mark as Completed buttons */}
                            <button className="edit-button" onClick={() => startEditing(task.id)}>
                                Edit
                            </button>
                            <button className="delete-button" onClick={() => deleteTask(task.id)}>
                                Delete
                            </button>
                            <button className="completed-button" onClick={() => markAsCompleted(task.id)}>
                                Completed
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default TaskList;
