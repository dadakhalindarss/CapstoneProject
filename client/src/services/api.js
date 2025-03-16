import axios from "axios";

const API_URL = "http://localhost:5000/api";

// ✅ Axios instance with base API URL
const api = axios.create({
    baseURL: API_URL,
    headers: { "Content-Type": "application/json" },
});

// ✅ Helper function to attach Authorization header
const authHeaders = (token) => ({
    headers: { Authorization: `Bearer ${token}` },
});

// ✅ Store token in local storage
export const setAuthToken = (token) => {
    if (token) {
        localStorage.setItem("authToken", token);
        api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    } else {
        localStorage.removeItem("authToken");
        delete api.defaults.headers.common["Authorization"];
    }
};

// ✅ Login User (Stores token & role)
export const loginUser = async (email, password) => {
    try {
        const response = await api.post("/auth/login", { email, password });
        const { token, role } = response.data;

        setAuthToken(token);
        return { token, role };
    } catch (error) {
        throw error.response?.data?.message || "Login failed";
    }
};

// ✅ Logout User (Clears token)
export const logoutUser = () => {
    setAuthToken(null);
};

// ✅ Get all tasks
export const getTasks = async (token) => {
    try {
        const response = await api.get("/tasks", authHeaders(token));
        return response.data;
    } catch (error) {
        console.error("Error fetching tasks:", error);
        throw error.response?.data?.message || "Failed to fetch tasks";
    }
};

// ✅ Create a new task (Only Admins & Managers)
export const createTask = async (task, token) => {
    try {
        await api.post("/tasks", task, authHeaders(token));
    } catch (error) {
        console.error("Error creating task:", error);
        throw error.response?.data?.message || "Failed to create task";
    }
};

// ✅ Update task status
export const updateTaskStatus = async (id, status, token) => {
    try {
        await api.put(`/tasks/${id}`, { status }, authHeaders(token));
    } catch (error) {
        console.error("Error updating task:", error);
        throw error.response?.data?.message || "Failed to update task";
    }
};

// ✅ Get all users
export const getUsers = async (token) => {
    try {
        const response = await api.get("/users", authHeaders(token));
        return response.data;
    } catch (error) {
        console.error("Error fetching users:", error);
        throw error.response?.data?.message || "Failed to fetch users";
    }
};

// ✅ Assign a task to a user
export const assignTask = async (taskId, userId, token) => {
    try {
        await api.put(`/tasks/${taskId}/assign/${userId}`, {}, authHeaders(token));
    } catch (error) {
        console.error("Error assigning task:", error);
        throw error.response?.data?.message || "Failed to assign task";
    }
};

// ✅ Get logged-in user's role
export const getUserRole = async (token) => {
    try {
        const response = await api.get("/auth/me", authHeaders(token));
        return response.data.role; // Returns role (Admin, Manager, User)
    } catch (error) {
        console.error("Error fetching user role:", error);
        throw error.response?.data?.message || "Failed to fetch user role";
    }
};
