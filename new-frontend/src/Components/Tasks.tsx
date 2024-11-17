// Tasks.tsx

import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './Tasks.css';

// Define the Task interface to type the task data
interface Task {
    taskID: number;
    task_Name: string;
    task_Description: string;
    task_Status: 'Pending' | 'In Progress' | 'Completed';
    task_Priority: number;
}

const Tasks: React.FC = () => {
    // State to store the list of tasks
    const [tasks, setTasks] = useState<Task[]>([]);
    // State to store new task data during creation
    const [newTask, setNewTask] = useState({
        task_Name: '',
        task_Description: '',
        task_Status: 'Pending',
        task_Priority: 1,
    });
    // State to track which task is being edited
    const [editTaskId, setEditTaskId] = useState<number | null>(null);
    // State to store the data of the task being edited
    const [editTaskData, setEditTaskData] = useState<Partial<Task>>({});
    // State to handle and display error messages
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    // State to track the current filter
    const [taskFilter, setTaskFilter] = useState<'All' | 'Pending' | 'In Progress' | 'Completed'>('All');

    // Helper function to sort tasks by priority (ascending order)
    const sortTasks = (tasksArray: Task[]) => {
        return tasksArray.sort((a, b) => a.task_Priority - b.task_Priority);
    };

    // Fetch tasks from the backend API when the component mounts
    const fetchTasks = (filter: 'All' | 'Pending' | 'In Progress' | 'Completed' = 'All') => {
        const endpoint =
            filter === 'All'
                ? 'http://localhost:5191/api/task/usertasks'
                : `http://localhost:5191/api/task/usertask/${filter}`;

        axios
            .get<Task[]>(endpoint, { withCredentials: true })
            .then((response) => {
                const sortedTasks = sortTasks(response.data);
                setTasks(sortedTasks); // Update tasks state with sorted tasks
            })
            .catch((error) => console.error('Error Fetching Tasks:', error));
    };

    useEffect(() => {
        fetchTasks(); // Initial fetch for all tasks
    }, []);

    // Handle the creation of a new task
    const handleCreateTask = () => {
        // Validation to ensure all fields are filled and priority is valid
        if (
            !newTask.task_Name ||
            !newTask.task_Description ||
            !newTask.task_Status ||
            !newTask.task_Priority ||
            newTask.task_Priority < 1
        ) {
            setErrorMessage('All Fields Are Mandatory!');
            return;
        }

        axios
            .post('http://localhost:5191/api/task/create', newTask, { withCredentials: true })
            .then(() => {
                // After creating the task, fetch the updated task list
                fetchTasks(taskFilter); // Fetch tasks again with the current filter
                setNewTask({
                    task_Name: '',
                    task_Description: '',
                    task_Status: 'Pending',
                    task_Priority: 1,
                });
                setErrorMessage(null); // Clear any error messages
            })
            .catch((error) => console.error('Error Creating Task:', error));
    };

    // Handle updating an existing task
    const handleEditTask = (taskId: number) => {
        const updatedTaskData = {
            ...tasks.find((task) => task.taskID === taskId),
            ...editTaskData,
        };

        if (
            !updatedTaskData.task_Name ||
            !updatedTaskData.task_Description ||
            !updatedTaskData.task_Status ||
            !updatedTaskData.task_Priority ||
            updatedTaskData.task_Priority < 1
        ) {
            setErrorMessage('All Fields Are Mandatory!');
            return;
        }

        axios
            .put(`http://localhost:5191/api/task/update/${taskId}`, editTaskData, { withCredentials: true })
            .then(() => {
                fetchTasks(taskFilter); // Fetch tasks again with the current filter
                setEditTaskId(null);
                setEditTaskData({});
                setErrorMessage(null);
            })
            .catch((error) => console.error('Error Updating Task:', error));
    };

    // Handle deleting a task
    const handleDeleteTask = (taskId: number) => {
        axios
            .delete(`http://localhost:5191/api/task/delete/${taskId}`, { withCredentials: true })
            .then(() => {
                fetchTasks(taskFilter); // Fetch tasks again with the current filter
            })
            .catch((error) => console.error('Error Deleting Task:', error));
    };

    // Handle filter change
    const handleFilterChange = (filter: 'All' | 'Pending' | 'In Progress' | 'Completed') => {
        setTaskFilter(filter); // Update the filter state
        fetchTasks(filter); // Fetch tasks based on the selected filter
    };

    return (
        <div className="tasks-container">
            <h1>Tasks</h1>

            {/* Filter buttons */}
            <div className="task-filter">
                <button
                    className={taskFilter === 'All' ? 'active' : ''}
                    onClick={() => handleFilterChange('All')}
                >
                    All
                </button>
                <button
                    className={taskFilter === 'Pending' ? 'active' : ''}
                    onClick={() => handleFilterChange('Pending')}
                >
                    Pending
                </button>
                <button
                    className={taskFilter === 'In Progress' ? 'active' : ''}
                    onClick={() => handleFilterChange('In Progress')}
                >
                    In Progress
                </button>
                <button
                    className={taskFilter === 'Completed' ? 'active' : ''}
                    onClick={() => handleFilterChange('Completed')}
                >
                    Completed
                </button>
            </div>

            {/* Form to create a new task */}
            <div className="task-form">
                <input
                    type="text"
                    placeholder="Task Name"
                    value={newTask.task_Name}
                    onChange={(e) => setNewTask({ ...newTask, task_Name: e.target.value })}
                />
                <input
                    type="text"
                    placeholder="Task Description"
                    value={newTask.task_Description}
                    onChange={(e) => setNewTask({ ...newTask, task_Description: e.target.value })}
                />
                <select
                    value={newTask.task_Status}
                    onChange={(e) =>
                        setNewTask({
                            ...newTask,
                            task_Status: e.target.value as Task['task_Status'],
                        })
                    }
                >
                    <option value="Pending">Pending</option>
                    <option value="In Progress">In Progress</option>
                    <option value="Completed">Completed</option>
                </select>
                <input
                    type="number"
                    min="1"
                    placeholder="Task Priority"
                    value={newTask.task_Priority}
                    onChange={(e) =>
                        setNewTask({
                            ...newTask,
                            task_Priority: Number(e.target.value),
                        })
                    }
                />
                <button onClick={handleCreateTask}>Create Task</button>
                {errorMessage && <p className="error-message">{errorMessage}</p>}
            </div>

            {/* Display the list of tasks */}
            <ul className="task-list">
                {tasks.map((task) => (
                    <li key={task.taskID} className="task-item">
                        {editTaskId === task.taskID ? (
                            <div>
                                <input
                                    type="text"
                                    placeholder="Task Name"
                                    value={editTaskData.task_Name || task.task_Name}
                                    onChange={(e) =>
                                        setEditTaskData({
                                            ...editTaskData,
                                            task_Name: e.target.value,
                                        })
                                    }
                                />
                                <input
                                    type="text"
                                    placeholder="Task Description"
                                    value={editTaskData.task_Description || task.task_Description}
                                    onChange={(e) =>
                                        setEditTaskData({
                                            ...editTaskData,
                                            task_Description: e.target.value,
                                        })
                                    }
                                />
                                <select
                                    value={editTaskData.task_Status || task.task_Status}
                                    onChange={(e) =>
                                        setEditTaskData({
                                            ...editTaskData,
                                            task_Status: e.target.value as Task['task_Status'],
                                        })
                                    }
                                >
                                    <option value="Pending">Pending</option>
                                    <option value="In Progress">In Progress</option>
                                    <option value="Completed">Completed</option>
                                </select>
                                <input
                                    type="number"
                                    min="1"
                                    placeholder="Task Priority"
                                    value={editTaskData.task_Priority ?? task.task_Priority}
                                    onChange={(e) =>
                                        setEditTaskData({
                                            ...editTaskData,
                                            task_Priority: Number(e.target.value),
                                        })
                                    }
                                />
                                <button onClick={() => handleEditTask(task.taskID)}>Save</button>
                                <button
                                    onClick={() => {
                                        setEditTaskId(null);
                                        setEditTaskData({});
                                        setErrorMessage(null);
                                    }}
                                >
                                    Cancel
                                </button>
                                {errorMessage && <p className="error-message">{errorMessage}</p>}
                            </div>
                        ) : (
                            <div>
                                <h3>{task.task_Name}</h3>
                                <p>{task.task_Description}</p>
                                <p>Status: {task.task_Status}</p>
                                <p>Priority: {task.task_Priority}</p>
                                <button
                                    onClick={() => {
                                        setEditTaskId(task.taskID);
                                        setEditTaskData({});
                                        setErrorMessage(null);
                                    }}
                                >
                                    Edit
                                </button>
                                <button onClick={() => handleDeleteTask(task.taskID)}>Delete</button>
                            </div>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Tasks;
