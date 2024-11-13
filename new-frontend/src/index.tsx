// index.tsx

// This is the entry point of the React application. It sets up the root element and renders the App component.

import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css'; // Import global CSS styles
import App from './App'; // Import the main App component
import reportWebVitals from './reportWebVitals'; // Import performance measuring tool

// Create a root element where the React app will be mounted
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement // Get the DOM element with id 'root'
);

// Render the App component inside React.StrictMode to activate additional checks and warnings
root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);

// Measure performance in the app (optional)
// You can pass a function to log results (e.g., reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
