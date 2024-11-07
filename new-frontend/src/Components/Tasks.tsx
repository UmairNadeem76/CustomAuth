import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './Tasks.css';

interface Task {
    taskID: number;
    task_Name: string;
    task_Description: string;
    task_Status: 'Pending' | 'In Progress' | 'Completed';
    task_Priority: number;
}

const Tasks: React.FC = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [newTask, setNewTask] = useState({
        task_Name: '',
        task_Description: '',
        task_Status: 'Pending',
        task_Priority: 1,
    });
    const [editTaskId, setEditTaskId] = useState<number | null>(null);
    const [editTaskData, setEditTaskData] = useState<Partial<Task>>({});
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    // Helper function to sort tasks by priority
    const sortTasks = (tasksArray: Task[]) => {
        return tasksArray.sort((a, b) => a.task_Priority - b.task_Priority);
    };

    useEffect(() => {
        axios
            .get<Task[]>('http://localhost:5191/api/task/usertasks', { withCredentials: true })
            .then((response) => {
                const sortedTasks = sortTasks(response.data);
                setTasks(sortedTasks);
            })
            .catch((error) => console.error('Error Fetching Tasks:', error));
    }, []);

    const handleCreateTask = () => {
        // Validation
        if (
            !newTask.task_Name ||
            !newTask.task_Description ||
            !newTask.task_Status ||
            !newTask.task_Priority ||
            newTask.task_Priority < 1
        ) {
            setErrorMessage(
                'All Fields Are Mandatory!'
            );
            return;
        }

        axios
            .post('http://localhost:5191/api/task/create', newTask, { withCredentials: true })
            .then(() => {
                // Fetch the updated task list
                return axios.get('http://localhost:5191/api/task/usertasks', { withCredentials: true });
            })
            .then((response) => {
                const sortedTasks = sortTasks(response.data);
                setTasks(sortedTasks);
                setNewTask({
                    task_Name: '',
                    task_Description: '',
                    task_Status: 'Pending',
                    task_Priority: 1,
                });
                setErrorMessage(null);
            })
            .catch((error) => console.error('Error Creating Task:', error));
    };

    const handleEditTask = (taskId: number) => {
        // Prepare updated task data
        const updatedTaskData = {
            ...tasks.find((task) => task.taskID === taskId),
            ...editTaskData,
        };

        // Validation
        if (
            !updatedTaskData.task_Name ||
            !updatedTaskData.task_Description ||
            !updatedTaskData.task_Status ||
            !updatedTaskData.task_Priority ||
            updatedTaskData.task_Priority < 1
        ) {
            setErrorMessage(
                'All Fields Are Mandatory!'
            );
            return;
        }

        axios
            .put(`http://localhost:5191/api/task/update/${taskId}`, editTaskData, { withCredentials: true })
            .then(() => {
                const updatedTasks = tasks.map((task) =>
                    task.taskID === taskId ? { ...task, ...editTaskData } : task
                );
                const sortedTasks = sortTasks(updatedTasks);
                setTasks(sortedTasks);
                setEditTaskId(null);
                setEditTaskData({});
                setErrorMessage(null); // Clear error message on success
            })
            .catch((error) => console.error('Error Updating Task:', error));
    };

    const handleDeleteTask = (taskId: number) => {
        axios
            .delete(`http://localhost:5191/api/task/delete/${taskId}`, { withCredentials: true })
            .then(() => {
                const updatedTasks = tasks.filter((task) => task.taskID !== taskId);
                setTasks(updatedTasks);
            })
            .catch((error) => console.error('Error Deleting Task:', error));
    };

    return (
        <div className="tasks-container">
            <h1>Tasks</h1>

            {/* Create Task Form */}
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
                {/* Error Message */}
                {errorMessage && <p className="error-message">{errorMessage}</p>}
            </div>

            {/* Task List */}
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
                                {/* Error Message */}
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
