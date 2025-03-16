import { createContext, useState, useEffect, useCallback } from "react";
import { jwtDecode } from "jwt-decode";
import axios from "axios";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem("token") || null);
    const [error, setError] = useState(null);

    // ✅ Wrap setLogoutTimer in useCallback to prevent unnecessary re-renders
    const setLogoutTimer = useCallback((time) => {
        setTimeout(() => {
            logout();
        }, time);
    }, []);

    useEffect(() => {
        if (token) {
            try {
                const decodedUser = jwtDecode(token);
                const isExpired = decodedUser.exp * 1000 < Date.now();

                if (!isExpired) {
                    setUser(decodedUser);
                    setLogoutTimer(decodedUser.exp * 1000 - Date.now());
                } else {
                    logout();
                }
            } catch (error) {
                console.error("Invalid token:", error);
                logout();
            }
        }
    }, [token, setLogoutTimer]); // ✅ Now includes setLogoutTimer

    const login = async (email, password) => {
        try {
            setError(null);
            const response = await axios.post("http://localhost:5000/api/auth/login", { email, password });
            const newToken = response.data.token;

            localStorage.setItem("token", newToken);
            setToken(newToken);
            setUser(jwtDecode(newToken));
            setLogoutTimer(jwtDecode(newToken).exp * 1000 - Date.now());

        } catch (error) {
            console.error("Login failed:", error.response?.data?.message || error.message);
            setError(error.response?.data?.message || "Invalid login credentials");
        }
    };

    const logout = () => {
        localStorage.removeItem("token");
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, token, error, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};
