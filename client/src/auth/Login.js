import { useContext, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

const Login = () => {
    const { login } = useContext(AuthContext);
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        try {
            const success = await login(email.trim(), password.trim());

            if (success) {
                navigate("/dashboard"); // Redirect on successful login
            } else {
                setError("Invalid email or password.");
            }
        } catch (err) {
            console.error("Login Error:", err.response?.data); //  Debugging error log

            let errorMessage = "Login failed. Please try again.";

            if (err.response?.data) {
                if (Array.isArray(err.response.data.errors)) {
                    errorMessage = err.response.data.errors.map((err) => err.msg).join(", ");
                } else if (typeof err.response.data.message === "string") {
                    errorMessage = err.response.data.message;
                }
            }

            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <h2>Login</h2>
            {error && <p style={{ color: "red" }}>{error}</p>}
            <form onSubmit={handleSubmit}>
                <input 
                    type="email" 
                    value={email} 
                    onChange={(e) => setEmail(e.target.value.trim())} 
                    placeholder="Email" 
                    required
                />
                <input 
                    type="password" 
                    value={password} 
                    onChange={(e) => setPassword(e.target.value.trim())} 
                    placeholder="Password" 
                    required
                    minLength={6}
                />
                <button type="submit" disabled={loading || !email || !password}>
                    {loading ? "Logging in..." : "Login"}
                </button>
            </form>
        </div>
    );
};

export default Login;
