import React, { useState } from 'react';
import { useForm, SubmitHandler, FieldErrors } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import './LoginSignup.css';
import user_icon from '../Assets/person.png';
import email_icon from '../Assets/email.png';
import password_icon from '../Assets/password.png';
import axios from 'axios';

// Define the types for form data
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
    const [action, setAction] = useState<'Signup' | 'Login'>("Signup");

    // Schema validation using zod
    const signupSchema = z.object({
        firstName: z.string().min(1, "First Name Is Required"),
        lastName: z.string().min(1, "Last Name Is Required"),
        email: z.string().email("Please Enter A Valid Email Address"),
        userName: z.string().min(8, "Username Must Be At Least 8 Characters Long"),
        password: z.string().min(8, "Password Must Be At Least 8 Characters Long"),
    });

    const loginSchema = z.object({
        email: z.string().email("Please Enter A Valid Email Address"),
        password: z.string().min(8, "Password Must Be At Least 8 Characters Long"),
    });

    // Switch between schemas based on the current action (Signup/Login)
    const schema = action === "Signup" ? signupSchema : loginSchema;

    const {
        register,
        handleSubmit,
        formState: { errors },
        reset
    } = useForm<SignupFormData | LoginFormData>({
        resolver: zodResolver(schema),
    });

    // Handle form submission
    const onSubmit: SubmitHandler<SignupFormData | LoginFormData> = async (data) => {
        try {
            if (action === "Signup") {
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
                const response = await axios.post('http://localhost:5191/api/account/login', {
                    UserNameOrEmail: (data as LoginFormData).email,
                    Password: (data as LoginFormData).password,
                }, {
                    withCredentials: true // Enable this if you are using cookies for authentication
                });

                if (response.status === 200) {
                    alert('Login Successful');
                } else {
                    alert(`Login Failed: ${response.data.message}`);
                }
            }
        } catch (error: any) {
            console.error('Error:', error);
            alert(error.response?.data?.message || 'An error occurred. Please try again.');
        } finally {
            reset();
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
                <div className="input">
                    <img src={email_icon} alt="Email Icon" />
                    <input
                        type="email"
                        placeholder="Email"
                        {...register("email")}
                    />
                    {errors.email && <div className="error-message">{errors.email.message}</div>}
                </div>
                <div className="input">
                    <img src={password_icon} alt="Password Icon" />
                    <input
                        type="password"
                        placeholder="Password"
                        {...register("password")}
                    />
                    {errors.password && <div className="error-message">{errors.password.message}</div>}
                </div>
                {action === "Signup"}
                {action === "Login" && (
                    <div className="forgot-password">Forgot Password? <span>Click Here</span></div>
                )}
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
                <button type="submit" className="submit-button">
                    Submit
                </button>
            </form>
        </div>
    );
};

export default LoginSignup;
