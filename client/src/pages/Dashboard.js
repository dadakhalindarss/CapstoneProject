import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const Dashboard = () => {
    const { user, logout } = useContext(AuthContext);
    const navigate = useNavigate();
    const [tasks, setTasks] = useState([]);

    useEffect(() => {
        if (!user) {
            navigate("/login");
            return;
        }

        const fetchTasks = async () => {
            try {
                const response = await axios.get("http://localhost:5000/api/tasks", {
                    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` },
                });
                setTasks(response.data);
            } catch (error) {
                console.error("Error fetching tasks", error);
            }
        };

        fetchTasks();
    }, [user, navigate]);

    return (
        <div>
            <h2>Welcome, {user?.email}</h2>
            <button onClick={logout}>Logout</button>

            <h3>Your Tasks:</h3>
            <ul>
                {tasks.length > 0 ? tasks.map((task) => (
                    <li key={task.id}>
                        {task.title} - <strong>{task.status}</strong>
                    </li>
                )) : <p>No tasks assigned</p>}
            </ul>
        </div>
    );
};

export default Dashboard;
