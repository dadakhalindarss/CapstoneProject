import React, { useEffect, useState } from "react";
import { getTasks, updateTaskStatus, getUsers, assignTask } from "../services/api";

const TaskList = () => {
    const [tasks, setTasks] = useState([]);
    const [users, setUsers] = useState([]);
    const [assignments, setAssignments] = useState({});

    useEffect(() => {
        fetchTasks();
        fetchUsers();
    }, []);

    const fetchTasks = async () => {
        const data = await getTasks();
        setTasks(data);
    };

    const fetchUsers = async () => {
        const data = await getUsers();
        setUsers(data);
    };

    const handleStatusChange = async (id, status) => {
        await updateTaskStatus(id, status);
        fetchTasks();
    };

    const handleAssignTask = async (taskId) => {
        const userId = assignments[taskId];
        if (!userId) {
            alert("Please select a user to assign!");
            return;
        }

        await assignTask(taskId, userId);
        fetchTasks();
    };

    return (
        <div>
            <h2>Task List</h2>
            <ul>
                {tasks.map((task) => (
                    <li key={task.id}>
                        <strong>{task.title}</strong> - {task.status} <br />
                        Assigned to: {task.assignee ? task.assignee.fullName : "Unassigned"}

                        <div>
                            <select
                                onChange={(e) =>
                                    setAssignments({ ...assignments, [task.id]: e.target.value })
                                }
                            >
                                <option value="">Select User</option>
                                {users.map((user) => (
                                    <option key={user.id} value={user.id}>
                                        {user.fullName}
                                    </option>
                                ))}
                            </select>
                            <button onClick={() => handleAssignTask(task.id)}>Assign</button>
                        </div>

                        <button onClick={() => handleStatusChange(task.id, "In Progress")}>
                            Move to In Progress
                        </button>
                        <button onClick={() => handleStatusChange(task.id, "Completed")}>
                            Complete
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default TaskList;
