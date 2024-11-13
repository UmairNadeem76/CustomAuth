// LoginSignup.tsx

// This component handles both user login and signup functionalities.
// It uses React Hook Form for form management and Zod for schema validation.

import React, { useContext, useState } from 'react';
import { useForm, SubmitHandler, FieldErrors } from 'react-hook-form';
import { z } from 'zod';
import { useNavigate } from 'react-router-dom';
import { zodResolver } from '@hookform/resolvers/zod';
import './LoginSignup.css';
import user_icon from '../Assets/person.png';
import email_icon from '../Assets/email.png';
import password_icon from '../Assets/password.png';
import axios from 'axios';
import { UserContext } from './UserContext';

// Define the types for form data for both signup and login forms
interface SignupFormData {
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    password: string;
}

interface LoginFormData {
    email: string;
    password: string;
}

const LoginSignup: React.FC = () => {
    // State to toggle between Signup and Login forms
    const [action, setAction] = useState<'Signup' | 'Login'>("Signup");
    const navigate = useNavigate();
    const { user, setUser, fetchUserData } = useContext(UserContext);

    // Schema validation using Zod for signup form
    const signupSchema = z.object({
        firstName: z.string().min(1, "First Name Is Required"),
        lastName: z.string().min(1, "Last Name Is Required"),
        email: z.string().email("Please Enter A Valid Email Address"),
        userName: z.string().min(8, "Username Must Be At Least 8 Characters Long"),
        password: z.string().min(8, "Password Must Be At Least 8 Characters Long"),
    });

    // Schema validation using Zod for login form
    const loginSchema = z.object({
        email: z.string().email("Please Enter A Valid Email Address"),
        password: z.string().min(8, "Password Must Be At Least 8 Characters Long"),
    });

    // Switch between schemas based on the current action (Signup/Login)
    const schema = action === "Signup" ? signupSchema : loginSchema;

    // Use React Hook Form for form management
    const {
        register,
        handleSubmit,
        formState: { errors },
        reset
    } = useForm<SignupFormData | LoginFormData>({
        resolver: zodResolver(schema),
    });

    // Handle form submission for both signup and login
    const onSubmit: SubmitHandler<SignupFormData | LoginFormData> = async (data) => {
        try {
            if (action === "Signup") {
                // Make API call to register the user
                const response = await axios.post('http://localhost:5191/api/account/register', {
                    Email: (data as SignupFormData).email,
                    FirstName: (data as SignupFormData).firstName,
                    LastName: (data as SignupFormData).lastName,
                    Password: (data as SignupFormData).password,
                    UserName: (data as SignupFormData).userName,
                });

                if (response.status === 200) {
                    alert('Signup Successful');
                } else {
                    alert(`Signup Failed: ${response.data.message}`);
                }
            } else {
                // Make API call to log in the user
                const response = await axios.post(
                    'http://localhost:5191/api/account/login',
                    {
                        UserNameOrEmail: (data as LoginFormData).email,
                        Password: (data as LoginFormData).password,
                    },
                    {
                        withCredentials: true,
                    }
                );
                if (response.status === 200) {
                    alert('Login Successful');

                    // Fetch user data after successful login
                    const userData = await fetchUserData();

                    // Redirect based on user role or email
                    if (userData?.email === 'admin@gmail.com') {
                        navigate('/admindashboard');
                    } else {
                        navigate('/dashboard');
                    }
                }
                else {
                    alert(`Login Failed: ${response.data.message}`);
                }
            }
        } catch (error: any) {
            console.error('Error:', error);
            alert(error.response?.data?.message || 'An error occurred. Please try again.');
        } finally {
            reset(); // Reset the form after submission
        }
    };

    return (
        <div className='container'>
            <div className='header'>
                <div className="text">{action}</div>
                <div className="underline"></div>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="inputs">
                {action === "Signup" && (
                    <>
                        {/* First Name Input */}
                        <div className="input">
                            <img src={user_icon} alt="User Icon" />
                            <input
                                type="text"
                                placeholder="First Name"
                                {...register("firstName")}
                            />
                            {errors && 'firstName' in errors && (
                                <div className="error-message">{(errors as FieldErrors<SignupFormData>).firstName?.message}</div>
                            )}
                        </div>
                        {/* Last Name Input */}
                        <div className="input">
                            <img src={user_icon} alt="User Icon" />
                            <input
                                type="text"
                                placeholder="Last Name"
                                {...register("lastName")}
                            />
                            {errors && 'lastName' in errors && (
                                <div className="error-message">{(errors as FieldErrors<SignupFormData>).lastName?.message}</div>
                            )}
                        </div>
                        {/* Username Input */}
                        <div className="input">
                            <img src={user_icon} alt="User Icon" />
                            <input
                                type="text"
                                placeholder="Username"
                                {...register("userName")}
                            />
                            {errors && 'userName' in errors && (
                                <div className="error-message">{(errors as FieldErrors<SignupFormData>).userName?.message}</div>
                            )}
                        </div>
                    </>
                )}
                {/* Email Input */}
                <div className="input">
                    <img src={email_icon} alt="Email Icon" />
                    <input
                        type="email"
                        placeholder="Email"
                        {...register("email")}
                    />
                    {errors.email && <div className="error-message">{errors.email.message}</div>}
                </div>
                {/* Password Input */}
                <div className="input">
                    <img src={password_icon} alt="Password Icon" />
                    <input
                        type="password"
                        placeholder="Password"
                        {...register("password")}
                    />
                    {errors.password && <div className="error-message">{errors.password.message}</div>}
                </div>
                {/* Forgot Password Link for Login Action */}
                {action === "Login" && (
                    <div className="forgot-password">Forgot Password? <span>Click Here</span></div>
                )}
                {/* Toggle between Signup and Login */}
                <div className="submit-container">
                    <div
                        className={action === "Signup" ? "submit gray" : "submit"}
                        onClick={() => setAction("Signup")}
                    >
                        Signup
                    </div>
                    <div
                        className={action === "Login" ? "submit gray" : "submit"}
                        onClick={() => setAction("Login")}
                    >
                        Login
                    </div>
                </div>
                {/* Submit Button */}
                <button type="submit" className="submit-button">
                    Submit
                </button>
            </form>
        </div>
    );
};

export default LoginSignup;
