import { useState } from "react";
import { Outlet, useLocation } from "react-router-dom";
import { Container, Row } from "reactstrap";
import NavigationBar from "./NavigationBar";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
function App() {
  const location = useLocation();
  const isLoginPage = location.pathname === "/";
  return (
    <Container fluid>
      <ToastContainer />
      {!isLoginPage && (
        <Row className="navigation-section border">
          <NavigationBar />
        </Row>
      )}
      <Row>
        <Outlet />
      </Row>
    </Container>
  );
}

export default App;
