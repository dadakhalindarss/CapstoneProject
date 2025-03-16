import { useState } from "react";
import axios from "axios";

const Register = () => {
    const [formData, setFormData] = useState({
        fullName: "",
        email: "",
        password: "",
        role: "",
    });

    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [successMessage, setSuccessMessage] = useState("");

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value.trim() });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccessMessage("");
        setLoading(true);

        try {
            const response = await axios.post("http://localhost:5000/api/auth/register", formData, {
                headers: { "Content-Type": "application/json" },
            });

            if (response.status === 201) {
                setSuccessMessage("✅ Registration successful! You can now log in.");
                setFormData({ fullName: "", email: "", password: "", role: "" }); 
            } else {
                setError("Unexpected response from server. Please try again.");
            }
        } catch (err) {
            let errorMessage = "Registration failed. Please try again.";
            if (err.response?.data?.errors) {
                errorMessage = Object.values(err.response.data.errors).flat().join(", ");
            } else if (typeof err.response?.data?.message === "string") {
                errorMessage = err.response.data.message;
            }
            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ maxWidth: "400px", margin: "auto", padding: "20px", border: "1px solid #ccc", borderRadius: "8px" }}>
            <h2>Register</h2>
            <form onSubmit={handleSubmit}>
                <input type="text" name="fullName" placeholder="Full Name" value={formData.fullName} onChange={handleChange} required />
                <input type="email" name="email" placeholder="Email" value={formData.email} onChange={handleChange} required />
                <input type="password" name="password" placeholder="Password" value={formData.password} onChange={handleChange} required />
                
                <select name="role" value={formData.role} onChange={handleChange} required>
                    <option value="">Select Role</option>
                    <option value="User">User</option>
                    <option value="Manager">Manager</option>
                    <option value="Admin">Admin</option>
                </select>

                {error && <p style={{ color: "red" }}>{error}</p>}
                {successMessage && <p style={{ color: "green" }}>{successMessage}</p>}

                <button type="submit" disabled={loading || !formData.fullName || !formData.email || !formData.password || !formData.role}>
                    {loading ? "Registering..." : "Register"}
                </button>
            </form>
        </div>
    );
};

export default Register;
