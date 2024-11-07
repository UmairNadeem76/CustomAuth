// UserContext.tsx
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

interface UserContextType {
    user: User | null;
    setUser: (user: User | null) => void;
    fetchUserData: () => Promise<User | null>;
    logout: () => Promise<void>;
}

export const UserContext = createContext<UserContextType>({
    user: null,
    setUser: () => {},
    fetchUserData: async () => null,
    logout: async () => {},
});

export const UserProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);

    // Fetch user data from the backend
    const fetchUserData = async (): Promise<User | null> => {
        try {
            const response = await axios.get<User>('http://localhost:5191/api/account/userdata', {
                withCredentials: true,
            });
            setUser(response.data);
            return response.data;
        } catch (error) {
            console.error('Error fetching user data:', error);
            setUser(null);
            return null;
        }
    };

    // Logout function
    const logout = async () => {
        try {
            await axios.post('http://localhost:5191/api/account/logout', {}, { withCredentials: true });
            setUser(null);
            window.location.href = '/'; // Reload the page to clear any client-side state
        } catch (error) {
            console.error('Error during logout:', error);
            alert('Logout failed. Please try again.');
        }
    };

    // Optionally, fetch user data on mount
    useEffect(() => {
        fetchUserData();
    }, []);

    return (
        <UserContext.Provider value={{ user, setUser, fetchUserData, logout }}>
            {children}
        </UserContext.Provider>
    );
};
