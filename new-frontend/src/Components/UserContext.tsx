// UserContext.tsx

// This file defines the UserContext, which manages user authentication state across the application.
// It provides functions to fetch user data and handle user logout.

import React, { createContext, useState, useEffect, ReactNode } from 'react';
import axios from 'axios';

// Define the User interface based on your backend response
interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    userName: string;
    // Add other user properties as needed
}

// Define the shape of the context
interface UserContextType {
    user: User | null;
    setUser: (user: User | null) => void;
    fetchUserData: () => Promise<User | null>;
    logout: () => Promise<void>;
}

// Create the UserContext with default values
export const UserContext = createContext<UserContextType>({
    user: null,
    setUser: () => { },
    fetchUserData: async () => null,
    logout: async () => { },
});

// Provider component to wrap the application and provide user context
export const UserProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null); // State to store the current user

    // Function to fetch user data from the backend
    const fetchUserData = async (): Promise<User | null> => {
        try {
            const response = await axios.get<User>('http://localhost:5191/api/account/userdata', {
                withCredentials: true, // Include cookies with the request
            });
            setUser(response.data); // Update user state with fetched data
            return response.data;
        } catch (error) {
            console.error('Error fetching user data:', error);
            setUser(null); // Reset user state if fetching fails
            return null;
        }
    };

    // Function to handle user logout
    const logout = async () => {
        try {
            // Send POST request to logout endpoint
            await axios.post('http://localhost:5191/api/account/logout', {}, { withCredentials: true });
            setUser(null); // Reset user state on successful logout
            window.location.href = '/'; // Reload the page to clear any client-side state
        } catch (error) {
            console.error('Error during logout:', error);
            alert('Logout failed. Please try again.');
        }
    };

    // Optionally, fetch user data when the component mounts
    useEffect(() => {
        fetchUserData();
    }, []);

    return (
        // Provide the user context to child components
        <UserContext.Provider value={{ user, setUser, fetchUserData, logout }}>
            {children}
        </UserContext.Provider>
    );
};
