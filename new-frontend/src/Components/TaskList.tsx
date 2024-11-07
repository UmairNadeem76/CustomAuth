import React, { useState } from 'react';
import './TaskList.css';

interface Task {
    id: number;
    title: string;
    description: string;
    status: 'Completed' | 'InProgress' | 'Pending';
    priority: number;
}

const TaskList: React.FC = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [newTask, setNewTask] = useState<Partial<Task>>({});
    const [isEditing, setIsEditing] = useState<number | null>(null);
    const [error, setError] = useState<string | null>(null);

    const handleChange = (
        e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
    ) => {
        const { name, value } = e.target;
        setNewTask({ ...newTask, [name]: name === 'priority' ? parseInt(value) : value });
        setError(null);
    };

    const validateForm = () => {
        if (!newTask.title || !newTask.description || !newTask.status || !newTask.priority) {
            setError('All Fields Are Required!');
            return false;
        }
        return true;
    };

    const addTask = () => {
        if (!validateForm()) return;
        const task: Task = {
            id: Date.now(),
            title: newTask.title || '',
            description: newTask.description || '',
            status: newTask.status as Task['status'],
            priority: newTask.priority || 1,
        };
        setTasks(prevTasks => sortTasks([task, ...prevTasks]));
        setNewTask({});
    };

    const updateTask = () => {
        if (!validateForm()) return;
        setTasks(prevTasks =>
            sortTasks(prevTasks.map(task => (task.id === isEditing ? { ...task, ...newTask } : task)))
        );
        setNewTask({});
        setIsEditing(null);
    };

    const deleteTask = (id: number) => {
        setTasks(tasks.filter(task => task.id !== id));
    };

    const markAsCompleted = (id: number) => {
        setTasks(tasks.filter(task => task.id !== id));
    };

    const startEditing = (id: number) => {
        const taskToEdit = tasks.find(task => task.id === id);
        if (taskToEdit) {
            setNewTask(taskToEdit);
            setIsEditing(id);
            setError(null);
        }
    };

    const sortTasks = (taskList: Task[]) => {
        return taskList.sort((a, b) => a.priority - b.priority);
    };

    return (
        <div className="task-list-container">
            <h1>Task List</h1>

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

                {error && <p className="error-message">{error}</p>}

                {isEditing ? (
                    <button onClick={updateTask}>Update Task</button>
                ) : (
                    <button onClick={addTask}>Add Task</button>
                )}
            </div>

            <div className="tasks-grid">
                {tasks.map(task => (
                    <div className="task-card" key={task.id}>
                        <h2>{task.title}</h2>
                        <p>{task.description}</p>
                        <p>Priority: {task.priority}</p>
                        <span className={`status ${task.status.toLowerCase()}`}>{task.status}</span>
                        <div className="task-actions">
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
