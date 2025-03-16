import React, { useState, useEffect } from "react";
import { createTask, getUsers, getUserRole } from "../services/api";

const TaskForm = ({ refreshTasks, token }) => {
    const [task, setTask] = useState({ title: "", description: "", assigneeId: "" });
    const [users, setUsers] = useState([]);
    const [userRole, setUserRole] = useState(null);

    useEffect(() => {
        fetchUsers();
        fetchUserRole();
    }, []);

    const fetchUsers = async () => {
        try {
            const data = await getUsers(token);
            setUsers(data);
        } catch (error) {
            console.error("Error fetching users:", error);
        }
    };

    const fetchUserRole = async () => {
        try {
            const role = await getUserRole(token);
            setUserRole(role);
        } catch (error) {
            console.error("Error fetching user role:", error);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!task.title || !task.assigneeId) {
            alert("Task title and assignee are required!");
            return;
        }

        try {
            await createTask(task, token, userRole);
            setTask({ title: "", description: "", assigneeId: "" });
            refreshTasks(); // Refresh task list after creation
        } catch (error) {
            alert(error.message || "Error creating task.");
        }
    };

    // ❌ If user is not Admin or Manager, hide the form
    if (!["Admin", "Manager"].includes(userRole)) {
        return <p>❌ You do not have permission to create tasks.</p>;
    }

    return (
        <form onSubmit={handleSubmit}>
            <input
                type="text"
                placeholder="Task Title"
                value={task.title}
                onChange={(e) => setTask({ ...task, title: e.target.value })}
                required
            />
            <input
                type="text"
                placeholder="Description"
                value={task.description}
                onChange={(e) => setTask({ ...task, description: e.target.value })}
            />
            <select
                value={task.assigneeId}
                onChange={(e) => setTask({ ...task, assigneeId: e.target.value })}
                required
            >
                <option value="">Select Assignee</option>
                {users.map((user) => (
                    <option key={user.id} value={user.id}>
                        {user.fullName}
                    </option>
                ))}
            </select>
            <button type="submit">Create Task</button>
        </form>
    );
};

export default TaskForm;
