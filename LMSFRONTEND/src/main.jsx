import React from "react";
import ReactDOM from "react-dom/client";
import "bootstrap/dist/css/bootstrap.min.css";
import { BrowserRouter as Router } from "react-router-dom";
import AppRouter from "./AppRouter";
import { LoggedUserProvider } from "./hooks/provider";

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <LoggedUserProvider>
      <Router>
        <AppRouter />
      </Router>
    </LoggedUserProvider>
  </React.StrictMode>
);
